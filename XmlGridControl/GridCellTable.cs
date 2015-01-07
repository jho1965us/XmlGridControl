//  Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
//  All rights reserved.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU LESSER GENERAL PUBLIC LICENSE as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU LESSER GENERAL PUBLIC LICENSE
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using WmHelp.XmlGrid.Annotations;

namespace WmHelp.XmlGrid
{

    [Flags]
    public enum GroupFlags
    {
        Expanded = 1,
        TableView = 2,
        Overlapped = 4,
        Value = 8,
        Merged = 16,
        ColumnsOverlapped = 32,
        NoExpand = 64,
        Indent = 128,
        NoColumnHeader = 256
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GridCell //: INotifyPropertyChanged
    {
        public static int LastSerialNumber;
        public int SerialNumber { get; private set; }
        private static int _serialNumberBreakCreate;

        public GridCell()
        {
            LastSerialNumber++;
            SerialNumber = LastSerialNumber;
            if (SerialNumber == SerialNumberBreakCreate)
                Debugger.Break();
        }

        public GridCellTable Owner { get; internal set; }

        public GridCellGroup Parent
        {
            get
            {
                if (Owner != null)
                    return Owner.Parent;
                else
                    return null;
            }
        }

        public string FullText
        {
            get { return string.Join(" > ", Linage().Select(GetText)); }
        }

        private object GetText(GridCell arg)
        {
            return arg.Text;
        }

        public int Index
        {
            get
            {
                return Owner.Width * Row + Col;
            }
        }

        public int Row { get; internal set; }

        public int Col { get; internal set; }

        public object Tag { get; set; }

        public virtual bool IsGroup { get { return false; } }

        public virtual String Text
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public virtual int ExpandImageIndex
        {
            get
            {
                return -1;
            }
        }

        public virtual int ImageIndex
        {
            get
            {
                return -1;
            }
        }

        public virtual bool CanEdit
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanEditManual
        {
            get
            {
                return false;
            }
        }

        public int SerialNumberBreakCreate
        {
            get { return _serialNumberBreakCreate; }
            set { _serialNumberBreakCreate = value; }
        }

        public int IndentLevel { get; set; }

        public virtual StringFormat GetStringFormat()
        {
            StringFormat stringFormat = new StringFormat(StringFormat.GenericDefault);
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Near;
            stringFormat.Trimming = StringTrimming.Character;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap;
            return stringFormat;
        }

        public virtual int GetTextWidth(XmlGridView gridView, Graphics graphics,
            Font font, XmlGridView.DrawInfo drawInfo)
        {
            SizeF sizeF = graphics.MeasureString(Text, font);
            if (ExpandImageIndex != -1)
                sizeF.Width += drawInfo.cxImage + 1;
            if (ImageIndex != -1)
                sizeF.Width += drawInfo.cxImage + 1;
            return (int)sizeF.Width;
        }

        public virtual int GetTextHeight(XmlGridView gridView, Graphics graphics,
            Font font, XmlGridView.DrawInfo drawInfo, int Width)
        {
            return drawInfo.cyChar;
        }

        public virtual void DrawCellText(XmlGridView gridView, Graphics graphics,
            Font font, Brush brush, StringFormat sf, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            rect.Y += 2;
            //rect.Height -= 3;
            graphics.DrawString(Text, font, brush, rect, sf);
        }

        public virtual void CopyToClipboard()
        {
            DataObject data = new DataObject();
            data.SetData(typeof(string), Text);
            Clipboard.SetDataObject(data);
        }

        public IEnumerable<GridCell> Linage()
        {
            if (Parent != null)
            {
                foreach (var ancestor in Parent.Linage())
                {
                    yield return ancestor;
                }
            }
            yield return this;
        }

        public override string ToString()
        {
            var colName = "";
            if (Parent != null && Parent.TableView)
                colName = string.Format(" ({0})", Parent.Table[Col, 0].Text);
            return string.Format("{0} ({1}, #{2}, [{3}{4}, {5}])", 
                Text, base.ToString(), SerialNumber, Col, colName, Row);
        }

        [DisplayName("ToString()")]
        public string ShowToString
        {
            get { return ToString(); }
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    var handler = PropertyChanged;
        //    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        //}
    }

    public class GridCellGroup: GridCell
    {
        private static int _serialNumberBreakCreateTable;
        private GridCellTable _table;
        private GridCellGroup _inner;
        private GridBuilderBase _gridBuilder;
        private GroupFlags _implementFlags;

        [Category("Table")]
        public virtual GridCellTable Table
        {
            get
            {
                return SelfOrInner()._table;
            }
        }

        class ImplementInner : GridCellGroup
        {
            private readonly GridCellGroup _owner;

            public ImplementInner(GridCellGroup owner, GridBuilderBase gridBuilder) : base(gridBuilder)
            {
                _owner = owner;
            }

            protected override GridCellGroup SelfOrInner()
            {
                return this;
            }

            internal override GroupFlags ImplementFlags
            {
                get
                {
                    var groupFlags = FlagsMask(_gridBuilder.UnfoldInner);
                    return _implementFlags & ~groupFlags | groupFlags;
                }
                set
                {
                    _implementFlags = value;
                }
            }

            GroupFlags FlagsMask(bool unfoldInner)
            {
                if (unfoldInner) 
                    return GroupFlags.ColumnsOverlapped | GroupFlags.Expanded;
                return GroupFlags.Overlapped | GroupFlags.NoColumnHeader | GroupFlags.Expanded;
            }
        }

        //todo probably room for optimization here
        protected virtual GridCellGroup SelfOrInner()
        {
            if (Parent != null && Parent.TableView && Row > 0)
            {
                var columnHeader = Parent.Table[Col, 0];
                if (columnHeader.IsGroup)
                {
                    var group = (GridCellGroup) columnHeader;
                    if (group.Expanded)
                    {
                        if (_inner == null)
                        {
                            _inner = new ImplementInner(this, _gridBuilder);
                            CreateTableView(_inner, (GridColumnLabel)columnHeader);
                            _inner.Owner = Owner;
                        }
                        return _inner;
                    }
                }
            }
            return this;
        }

        protected virtual void CreateTableView(GridCellGroup inner, GridColumnLabel columnHeader)
        {
            throw new InvalidOperationException(); 
        }

        internal virtual GroupFlags ImplementFlags
        {
            get { return _implementFlags; }
            set { _implementFlags = value; }
        }

        public GroupFlags Flags
        {
            get { return SelfOrInner().ImplementFlags; }
            set { SelfOrInner().ImplementFlags = value; }
        }

        //public GroupFlags Flags
        //{
        //    get { return ImplementFlags; }
        //    set
        //    {
        //        if (value == ImplementFlags) return;
        //        var save = ImplementFlags;
        //        ImplementFlags = value;
        //        OnPropertyChanged();
        //        OnFlagsChanged(GroupFlags.NoExpand, save, value, "ExpandImageIndex");
        //        OnFlagsChanged(GroupFlags.Expanded, save, value, "Expanded");
        //        OnFlagsChanged(GroupFlags.Overlapped, save, value, "Overlaped");
        //        OnFlagsChanged(GroupFlags.ColumnsOverlapped, save, value, "ColumnsOverlaped");
        //        OnFlagsChanged(GroupFlags.TableView, save, value, "TableView");
        //    }
        //}

        //private void OnFlagsChanged(GroupFlags flag, GroupFlags oldValue, GroupFlags newValue, string propertyName)
        //{
        //    if (oldValue.HasFlag(flag) != newValue.HasFlag(flag))
        //        OnPropertyChanged(propertyName);
        //}

        [Category("Table")]
        public int TableWidth { get; set; }

        [Category("Table")]
        public int TableHeight { get; set; }

        [Category("Table")]
        public int TablePadding { get; set; }

        protected internal GridCellGroup(GridBuilderBase gridBuilder)
        {
            _table = new GridCellTable(this);
            _gridBuilder = gridBuilder;
        }

        public GridCell FirstChild()
        {
            if ((Flags & GroupFlags.TableView) != 0)
                return Table[0, 1];
            else
                return Table[0, 0];
        }

        public GridCell LastChild()
        {
            return Table[0, Table.Height - 1];
        }

        public override int ExpandImageIndex
        {
            get
            {
                if (Flags.HasFlag(GroupFlags.NoExpand))
                    return -1;
                return Expanded ? 0 : 1;
            }
        }

        public virtual void BeforeExpand()
        {
            return;
        }

        public virtual String Description
        {
            get
            {
                return null;
            }
        }

        public override bool IsGroup { get { return true; } }

        public bool Expanded { get { return (Flags & GroupFlags.Expanded) != 0; } }

        public bool Overlaped { get { return (Flags & GroupFlags.Overlapped) != 0; } }

        public bool NoColumnHeader { get { return (Flags & GroupFlags.NoColumnHeader) != 0; } }

        public bool ColumnsOverlaped { get { return (Flags & GroupFlags.ColumnsOverlapped) != 0; } }

        public bool TableView { get { return Flags.HasFlag(GroupFlags.TableView); } }

        public bool Indent { get { return !Overlaped && !ColumnsOverlaped && !TableView; } }

        public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font, 
            Brush brush, StringFormat format, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            StringFormat sf = new StringFormat(format);
            Font f = new Font(font, FontStyle.Bold);
            Brush textBrush = new SolidBrush(SystemColors.GrayText);
            sf.LineAlignment = StringAlignment.Center;
            rect.Height = drawInfo.cyChar;
            graphics.DrawString(Text, f, brush, rect, sf);            
            int w = (int)graphics.MeasureString(Text, f).Width + drawInfo.cxCaps / 2;            
            rect.X += w;
            rect.Width -= w;            
            if (TableView)
                graphics.DrawString(String.Format("({0})", TableViewCount()), 
                    font, textBrush, rect, sf);
            else
                if (!Expanded && !String.IsNullOrEmpty(Description))
                {
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    sf.FormatFlags = StringFormatFlags.NoWrap;
                    graphics.DrawString(Description, font, textBrush, rect, sf);
                }
        }

        protected virtual int TableViewCount()
        {
            return Table.Height - 1;
        }

        public int SerialNumberBreakCreateTable
        {
            get { return _serialNumberBreakCreateTable; }
            set { _serialNumberBreakCreateTable = value; }
        }

        [Obsolete,UsedImplicitly]
        public GridCellGroup Inner
        {
            get { return _inner; }
        }

        [Obsolete, UsedImplicitly]
        public GridCellTable SelfTable
        {
            get { return _table; }
        }

        [Obsolete, UsedImplicitly]
        public GroupFlags SelfFlags
        {
            get { return ImplementFlags; }
        }
    }
    
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GridCellTable
    {
        internal GridCell[,] _cells;

        public GridCellGroup Parent { get; private set; }

        [Category("Table")]
        public int Width { get; private set; }

        [Category("Table")]
        public int Height { get; private set; }

        //[TypeConverter(typeof(ReadOnlyListConverter))]
        [Category("Table")]
        public int[] ColumnsWidth { get; private set; }

        //[TypeConverter(typeof(ReadOnlyListConverter))]
        [Category("Table")]
        public int[] RowHeight { get; private set; }

        //[TypeConverter(typeof(ReadOnlyListConverter))]
        [Category("Table")]
        public int[] RowCount { get; private set; }

        public GridCellTable(GridCellGroup parent)
        {
            Parent = parent;
        }

        [Category("Table")]
        [Obsolete, UsedImplicitly]
        public GridCell[][] Rows
        {
            get
            {
                var rows = new List<GridCell[]>();
                for (int s = 0; s < Height; s++)
                {
                    var cells = new List<GridCell>();
                    for (int k = 0; k < Width; k++)
                    {
                        cells.Add(_cells[k, s]);
                    }
                    rows.Add(cells.ToArray());
                }
                return rows.ToArray();
            }
        }

        public void SetBounds(int width, int height)
        {
            Width = width;
            Height = height;
            ColumnsWidth = new int[width];
            RowHeight = new int[height];
            RowCount = new int[height];
            _cells = new GridCell[width, height];            
        }

        [Category("Table")]
        public bool IsEmpty
        {
            get
            {
                return _cells == null;
            }
        }

        public GridCell this[int col, int row]
        {
            get
            {
                return _cells[col, row];
            }
            set
            {
                if (value.Owner != null)
                    throw new InvalidOperationException();
                value.Owner = this;
                value.Col = col;
                value.Row = row;
                _cells[col, row] = value;
            }
        }

        public override string ToString()
        {
            if (IsEmpty)
                return string.Format("{0} {1} - Empty", base.ToString(), Parent.Text);
            return string.Format("{0} {3}[{1},{2}]", base.ToString(), Width, Height, Parent.Text);
        }
    }

    public class GridColumnLabel: GridCellGroup
    {
        public GridColumnLabel(GridBuilderBase gridBuilder) : base(gridBuilder)
        {
        }

        public override int ImageIndex
        {
            get
            {
                return 3;
            }
        }

        public override string Text
        {
            get
            {
                return "label";
            }
            set
            {
                throw new InvalidOperationException();
            }
        }
    }

    public class GridRowLabel: GridCell
    {
        public GridRowLabel()
        {
        }

        public int RowNum { get; protected set; }

        public override string Text
        {
            get
            {
                if (RowNum > 0)
                    return RowNum.ToString();
                else
                    return "";
            }
            set
            {
            }
        }
    }

    public class GridHeadLabel : GridCell
    {
        private string _text;

        public override StringFormat GetStringFormat()
        {
            StringFormat sf = base.GetStringFormat();
            sf.Trimming = StringTrimming.EllipsisCharacter;
            return sf;
        }

        //public override Size MeasureText(XmlGridView gridView, Graphics graphics, 
        //    Font font, XmlGridView.DrawInfo drawInfo, int Width)
        //{
        //    Size sz = base.MeasureText(gridView, graphics, font, drawInfo, Width);
        //    sz.Width += drawInfo.cxChar;
        //    return sz;
        //}

        public override int GetTextWidth(XmlGridView gridView, Graphics graphics, 
            Font font, XmlGridView.DrawInfo drawInfo)
        {
            return base.GetTextWidth(gridView, graphics, font, drawInfo) + drawInfo.cxChar;
        }

        public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font,
            Brush brush, StringFormat sf, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            rect.X += drawInfo.cxChar;
            rect.Width -= drawInfo.cxChar;
            base.DrawCellText(gridView, graphics, font, 
                brush, sf, drawInfo, rect);
        }

        public override string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }
    }
}