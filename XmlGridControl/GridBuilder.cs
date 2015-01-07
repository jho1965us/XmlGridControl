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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Xml.XPath;

namespace WmHelp.XmlGrid
{
    public class XmlLabelCell: GridCell
    {
        public XmlNode Node { get; private set; }

        public XmlLabelCell(XmlNode node)
        {
            Node = node;
        }

        public override int ImageIndex
        {
            get
            {
                if (Node == null)
                    return -1;
                switch (Node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        return 2; // =
                    case XmlNodeType.Element:
                        return 3; // <>
                    case XmlNodeType.Text:
                        return 4; // Abc
                    case XmlNodeType.CDATA:
                        return 5; // [C..
                    case XmlNodeType.Comment:
                        return 6; // <!--
                    case XmlNodeType.DocumentType:
                        return 7; // Doc
                    default:
                        return -1;
                }
            }
        }

        public override string Text
        {
            get
            {
                if (Node != null)
                    switch (Node.NodeType)
                    {
                        case XmlNodeType.Comment:
                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                            return Node.Value;
                        
                        default:
                            return Node.Name;
                    }
                else
                    return null;
            }
            set
            {
                return;
            }
        }

        public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font, Brush brush, 
            StringFormat sf, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            if (Node.NodeType != XmlNodeType.Attribute && Node.NodeType != XmlNodeType.Element)
                font = new Font(font, FontStyle.Italic);
            base.DrawCellText(gridView, graphics, font, brush, sf, drawInfo, rect);
        }
    }

    public class XmlValueCell : GridCell
    {
        public XmlNode Node { get; private set; }

        public XmlValueCell(XmlNode node)
        {
            Node = node;
        }

        public override string Text
        {
            get
            {
                if (Node != null)
                {
                    if (Node.NodeType == XmlNodeType.Element)
                        return Node.InnerText;
                    else
                        return Node.Value;
                }
                else
                    return null;
            }
            set
            {
                return;
            }
        }

        public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font, Brush brush, 
            StringFormat sf, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            if (gridView.AutoHeightCells)
                sf.FormatFlags = sf.FormatFlags & ~StringFormatFlags.NoWrap;
            base.DrawCellText(gridView, graphics, font, brush, sf, drawInfo, rect);
        }

        public override int GetTextHeight(XmlGridView gridView, Graphics graphics, 
            Font font, XmlGridView.DrawInfo drawInfo, int Width)
        {
            if (String.IsNullOrEmpty(Text))
                return drawInfo.cyChar;
            else
            {
                StringFormat sf = GetStringFormat();
                sf.FormatFlags = 0;
                SizeF sz = graphics.MeasureString(Text, font, Width, sf);
                int height = Math.Max((int)sz.Height, drawInfo.cyChar);
                if (height > drawInfo.cyChar)
                    height += 4;
                return height;
            }
        }
    }

    public class XmlDeclarationCell : GridCell
    {
        public XmlNode Node { get; private set; }

        public XmlDeclarationCell(XmlNode node)
        {
            Node = node;
        }

        public override string Text
        {
            get
            {
                if (Node.NodeType == XmlNodeType.DocumentType)
                {
                    XmlDocumentType docType = (XmlDocumentType)Node;
                    if (docType.PublicId != null || docType.SystemId != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        if (docType.PublicId != null)
                            sb.AppendFormat("PUBLIC \"{0}\"", docType.PublicId);
                        if (docType.SystemId != null)
                        {
                            if (sb.Length > 0)
                                sb.Append(' ');
                            sb.AppendFormat("SYSTEM \"{0}\"", docType.SystemId);
                        }
                        return sb.ToString();
                    }
                }
                return Node.Value;
            }
            set
            {
                return;
            }
        }
    }

    public class XmlGroupCell : GridCellGroup
    {
        protected GridBuilder GridBuilder { get; private set; }
        public XmlNode Node { get; private set; }

        protected internal XmlGroupCell(GridBuilder gridBuilder, XmlNode node) : base(gridBuilder)
        {
            GridBuilder = gridBuilder;
            Node = node;
        }

        public GridBuilder.ItemList Items { get; set; }

        public override string Text
        {
            get
            {
                return Node.Name;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (Node != null && Node is XmlElement && ((XmlElement)Node).HasAttributes)
                    for (int i = 0; i < Node.Attributes.Count; i++)
                    {
                        if (sb.Length > 150)
                        {
                            sb.Append("..");
                            break;
                        }
                        if (i > 0)
                            sb.Append(" ");
                        XmlAttribute attr = (XmlAttribute)Node.Attributes.Item(i);
                        sb.AppendFormat("{0}={1}", attr.Name, attr.Value);
                    }
                return sb.ToString();
            }
        }

        public override void BeforeExpand()
        {
            if (Table.IsEmpty)
            {
                if (this.Items == null)
                    this.Items = GridBuilder.GetItems(this.Node.Attributes, this.Node.ChildNodes);
                GridBuilder.CreateTable(this, (GridBuilder.ItemList)this.Items);
            }
        }

        public override void CopyToClipboard()
        {
            CopyToClipboard(Node);
        }

        public virtual void CopyToClipboard(XmlNode Node)
        {
            DataFormats.Format fmt = DataFormats.GetFormat("EXML Fragment");
            DataObject data = new DataObject();
            MemoryStream stream = new MemoryStream();
            StreamWriter sw = new StreamWriter(stream, Encoding.Unicode);
            sw.Write("<doc>");
            if (TableView)
            {
                for (int s = 1; s < Table.Height; s++)
                {
                    XmlRowLabelCell cell = (XmlRowLabelCell)Table[0, s];
                    sw.Write(cell.Node.OuterXml);
                }
            }
            else
                sw.Write(Node.OuterXml);
            sw.Write("</doc>");
            sw.Flush();
            stream.WriteByte(0);
            stream.WriteByte(0);
            data.SetData(fmt.Name, false, stream);
            if (TableView)
            {                
                stream = new MemoryStream();
                sw = new StreamWriter(stream, Encoding.Default);
                String separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                for (int s = 0; s < Table.Height; s++)
                {
                    if (s > 0)
                        sw.WriteLine();
                    for (int k = 1; k < Table.Width; k++)
                    {
                        if (k > 1)
                            sw.Write(separator);
                        if (Table[k, s] != null && Table[k, s].Text != null)
                        {
                            String text = Table[k, s].Text;
                            if (text.Contains(separator) || text.Contains("\""))
                                text = String.Format("\"{0}\"", text.Replace("\"", "\"\""));
                            sw.Write(text);
                        }
                    }                    
                }
                sw.Flush();
                stream.WriteByte(0);
                stream.WriteByte(0);
                data.SetData(DataFormats.CommaSeparatedValue, false, stream);
            }
            data.SetText(Text);
            Clipboard.SetDataObject(data);
        }

        protected override void CreateTableView(GridCellGroup inner, GridColumnLabel columnHeader)
        {
            var group = (XmlColumnLabelCell)columnHeader;
            if (Items == null)
            {
                Items = group.GridBuilder.GetItems(null, new[] { Node });
                Items.Last.type = GridBuilder.ItemType.Table;
            }
            group.GridBuilder.CreateTableView(inner, Items.Last, @group.TableColumns);
            //_inner.Flags |= GroupFlags.Overlapped | GroupFlags.NoColumnHeader | GroupFlags.Expanded;
            //_inner.Flags |= GroupFlags.ColumnsOverlapped | GroupFlags.Expanded;
        }
    }

    public class XmlColumnLabelCell : GridColumnLabel
    {
        public GridBuilder GridBuilder { get; private set; }
        private readonly GridBuilder.TableColumn _column;
        public XmlNodeType NodeType { get; private set; }
        public String NodeName { get; private set; }
        public int NodePos { get; private set; }

        //public XmlColumnLabelCell(GridBuilder gridBuilder, Type type, string nodeName, int nodePos, ICollection<XmlNode> nodes)
        internal XmlColumnLabelCell(GridBuilder gridBuilder, XmlNodeType type, string nodeName, int nodePos, GridBuilder.TableColumn column) : base(gridBuilder)
        {
            GridBuilder = gridBuilder;
            _column = column;
            //if (typeof(XmlAttribute).IsAssignableFrom(type))
            //    NodeType = XmlNodeType.Attribute;
            //else if (typeof(XmlElement).IsAssignableFrom(type))
            //    NodeType = XmlNodeType.Element;
            //else if (typeof(XmlText).IsAssignableFrom(type))
            //    NodeType = XmlNodeType.Text;
            //else if (typeof(XmlCDataSection).IsAssignableFrom(type))
            //    NodeType = XmlNodeType.CDATA;
            //else if (typeof(XmlProcessingInstruction).IsAssignableFrom(type))
            //    NodeType = XmlNodeType.ProcessingInstruction;
            //else
            //    NodeType = XmlNodeType.None;
            switch (type)
            {
                case XmlNodeType.Element:
                    NodeType = type;
                    break;
                case XmlNodeType.Attribute:
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.ProcessingInstruction:
                    NodeType = type;
                    Flags |= GroupFlags.NoExpand;
                    break;
                default:
                    NodeType = XmlNodeType.None;
                    Flags |= GroupFlags.NoExpand;
                    break;
            }
            NodeName = nodeName;
            NodePos = nodePos;
        }

        public override int ImageIndex
        {
            get
            {
                switch (NodeType)
                {
                    case XmlNodeType.Attribute:
                        return 2;
                    case XmlNodeType.Element:
                        return 3;
                    case XmlNodeType.Text:
                        return 4;
                    case XmlNodeType.CDATA:
                        return 5;
                    default:
                        return -1;
                }
            }
        }

        internal GridBuilder.TableColumns TableColumns { get; private set; }

        public override void BeforeExpand()
        {
            if (Table.IsEmpty)
            {
                TableColumns = GridBuilder.CreateColumnHeaderRow(this, _column.GetNodes());
            }
        }

        public XmlNode GetNodeAtColumn(XmlNode node)
        {
            int nodePos = NodePos;
            if (NodeType == XmlNodeType.Attribute)
            {
                if (node.Attributes != null)
                    return node.Attributes.GetNamedItem(NodeName);
            }
            else
                if (node.HasChildNodes)
                    for (int k = 0; k < node.ChildNodes.Count; k++)
                    {
                        XmlNode child = node.ChildNodes.Item(k);
                        if (child.NodeType == NodeType && 
                            ((NodeType != XmlNodeType.Element || NodeName.Equals(child.Name))))
                            if (nodePos == 0)
                                return child;
                            else
                                nodePos--;
                    }
            return null;
        }

        public override string Text
        {
            get
            {
                switch (NodeType)
                {
                    case XmlNodeType.Text:
                        return "#text";
                    case XmlNodeType.CDATA:
                        return "#CDATA";
                    default:
                        return NodeName;
                }
            }
            set
            {
                base.Text = value;
            }
        }

        public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font, 
            Brush brush, StringFormat sf, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            if (NodeType != XmlNodeType.Element && NodeType != XmlNodeType.Attribute)
                font = new Font(font, FontStyle.Italic);
            else
                font = new Font(font, FontStyle.Bold);
            base.DrawCellText(gridView, graphics, font, brush, sf, drawInfo, rect);
        }

    }

    public class XmlRowLabelCell : GridRowLabel
    {
        public XmlNode Node { get; private set; }

        public XmlRowLabelCell(int row, XmlNode node)
        {
            RowNum = row;
            Node = node;
        }
    }

    public class GridBuilderBase
    {
        public bool UnfoldInner { get; set; }
    }

    public class GridBuilder : GridBuilderBase
    {
        /// <summary>
        /// Equals if type, name, pos, count equals
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        protected internal class TableColumn
        {
            public XmlNodeType type;
            public String name;
            public int pos;
            public int count;
            public bool marked;
            public HashSet<XmlNode> rows; 
            private readonly GridBuilder _gridBuilder;

            public TableColumn(GridBuilder gridBuilder)
            {
                _gridBuilder = gridBuilder;
            }

            public override bool Equals(object obj)
            {
                if (obj is TableColumn)
                {
                    TableColumn c = (TableColumn)obj;
                    return type == c.type && name == c.name &&
                        pos == c.pos && count == c.count;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public class ChildItems
            {
                public XmlNodeType NodeType { get; set; }
                public string Name { get; set; }
                public ItemType ItemType { get; set; }
                public ContentTypeResult ContentType { get; set; }

                public readonly Dictionary<XmlNode, ItemList> rows = new Dictionary<XmlNode, ItemList>();

                public bool CombineTypes(ItemType itemType)
                {
                    if (ItemType == itemType)
                        return true;
                    if (ItemType == ItemType.None)
                    {
                        ItemType = itemType;
                        return true;
                    }
                    if (ItemType == ItemType.Table)
                        return itemType == ItemType.List;
                    if (ItemType == ItemType.List && itemType == ItemType.Table)
                    {
                        ItemType = ItemType.Table;
                        return true;
                    }
                    return false;
                }
            }


            public List<string> path;
            private ChildItems rowItems;

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public class ContentTypeBuilder
            {
                private readonly TableColumn _column;
                private readonly ContentTypeOptions _options;

                public ContentTypeBuilder(TableColumn column, ContentTypeOptions options)
                {
                    _column = column;
                    _options = options;
                }

                public List<ChildItems> savedChildItems;
                public ChildItems ChildItems { get; set; }

                public bool IsSimpleContentType1(XmlNode row)
                {
                    ItemList items = null;
                    //todo maybe use GetNodeAtColumn
                    foreach (XmlNode child in row.ChildNodes)
                    {
                        if (child.Name == _column.name)
                        {
                            if (items != null)
                                return false; // if we have more than one the path would need to identify index(es) ie include '[...]'
                            items = _column._gridBuilder.GetItems(child.Attributes, child.ChildNodes, _options | ContentTypeOptions.Shortcut);
                            ContentType |= _column._gridBuilder.ContentType(items, _options);
                            if (ContentType.HasFlag(ContentTypeResult.Complex))
                                return false;
                        }
                    }
                    if (_options.HasFlag(ContentTypeOptions.AllowEmpty) && items == null) //todo is this correct?
                        return false;
                    return IsSimpleContentType(row, items);
                }

                private bool IsSimpleContentType(XmlNode row, ItemList items)
                {
                    ContentType |= _column._gridBuilder.ContentType(items, _options);
                    if (ContentType.HasFlag(ContentTypeResult.Complex))
                        return false;
                    if (ChildItems == null)
                        ChildItems = new ChildItems();
                    if (items.LastNode != null)
                    {
                        if (ChildItems.ItemType == ItemType.None)
                        {
                            ChildItems.NodeType = items.LastNode.NodeType;
                            ChildItems.Name = items.LastNode.Name;
                        }
                        else
                        {
                            if (ChildItems.NodeType != items.LastNode.NodeType ||
                                ChildItems.Name != items.LastNode.Name)
                                return false;
                        }
                    }
                    if (items.Last != null && !ChildItems.CombineTypes(items.Last.type))
                        return false;
                    ChildItems.rows.Add(row, items);
                    return true;
                }

                public ContentTypeResult ContentType { get; set; }

                internal bool IsSimpleContentType2(KeyValuePair<XmlNode, ItemList> pair)
                {
                    //if (pair.Value[0].type == ItemType.Table)
                    //{
                    //    return true;
                    //}
                    if (pair.Value.Last == null)
                        return true;
                    if (pair.Value.Last.type != ItemType.List)
                        return false;
                    var node = pair.Value.LastNode;
                    var items = _column._gridBuilder.GetItems(node.Attributes, node.ChildNodes, _options | ContentTypeOptions.Shortcut);
                    return IsSimpleContentType(pair.Key, items);
                }

                public bool IsComplexContentType<T>(IEnumerable<T> collection, Func<T, bool> isSimpleContent)
                {
                    var isSimple = collection.All(isSimpleContent);
                    if (ChildItems != null)
                    {
                        if (ChildItems.ItemType == ItemType.List)
                            ContentType |= ContentTypeResult.List;
                        else if (ChildItems.ItemType == ItemType.Table)
                            ContentType |= ContentTypeResult.Table;
                        else
                            ContentType |= ContentTypeResult.NoExpand;
                        ChildItems.ContentType = ContentType;
                    }
                    if (isSimple)
                        return false;
                    ContentType |= ContentTypeResult.Complex;
                    return true;
                }
            }

            public ContentTypeResult ContentType(ContentTypeOptions options)
            {
                builder = new ContentTypeBuilder(this, options);
                if (builder.IsComplexContentType(rows, builder.IsSimpleContentType1))
                    return builder.ContentType;
                if (builder.ContentType.HasFlag(ContentTypeResult.Leaf))
                    return builder.ContentType;
                if (builder.ContentType == (ContentTypeResult.Empty | ContentTypeResult.NoExpand))
                    return builder.ContentType;
                path = new List<string> { name };
                rowItems = builder.ChildItems;
                builder.savedChildItems = new List<ChildItems>();
                if (builder.ChildItems.ItemType == ItemType.Table)
                    path.Add(builder.ChildItems.Name);
                while (builder.ChildItems.ItemType == ItemType.List)
                {
                    builder.savedChildItems.Add(builder.ChildItems);
                    builder.ChildItems = null;
                    builder.ContentType = ContentTypeResult.Unknown;
                    if (builder.IsComplexContentType(rowItems.rows, builder.IsSimpleContentType2))
                        break;
                    path.Add(builder.ChildItems.Name);
                    rowItems = builder.ChildItems;
                }
                return rowItems.ContentType;
            }

            public ContentTypeBuilder builder { get; private set; }

            public ICollection<XmlNode> GetNodes()
            {
                if (path != null)
                {
                    var capacity = 0;
                    foreach (var itemList in rowItems.rows.Values)
                    {
                        if (itemList.Last != null)
                            capacity += itemList.Last.nodes.Count;
                    }
                    var nodes = new List<XmlNode>(capacity);
                    foreach (var itemList in rowItems.rows.Values)
                    {
                        if (itemList.Last != null)
                            nodes.AddRange(itemList.Last.nodes);
                    }
                    return nodes;
                }

                if (type == XmlNodeType.Attribute)
                {
                    var attributes = new List<XmlNode>(rows.Count);
                    foreach (var row in rows)
                    {
                        var attribute = row.Attributes[name];
                        if (attribute != null)
                            attributes.Add(attribute);
                    }
                    return attributes;
                }
                else
                {
                    var capacity = rows.Count;
                    if (type == XmlNodeType.Element)
                    {
                        foreach (var row in rows)
                            capacity += row.ChildNodes.Count;
                    }
                    var nodes = new List<XmlNode>(capacity);
                    foreach (var row in rows)
                    {
                        foreach (XmlNode child in row.ChildNodes)
                        {
                            if (child.NodeType == type)
                            {
                                if (type == XmlNodeType.Element && child.Name != name)
                                    continue;
                                nodes.Add(child);
                            }
                        }
                    }
                    return nodes;
                }
            }

            public bool AllTextOnly()
            {
                return false; //savedChildItems.Last().All(IsTextOnly);
            }

            private bool IsTextOnly(ItemList items)
            {
                return items.LastNode.NodeType == XmlNodeType.Text;
            }
        }

        TableColumns CreateTableColumns()
        {
            return new TableColumns(this);
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        protected internal class TableColumns : IEnumerable<TableColumn>
        {
            private readonly GridBuilder _gridBuilder;
            private List<TableColumn> _list;

            internal TableColumns(GridBuilder gridBuilder)
            {
                _gridBuilder = gridBuilder;
                _list = new List<TableColumn>();
            }

            public int Length
            {
                get
                {
                    return _list.Count;
                }
            }

            public TableColumn this[int index]
            {
                get
                {
                    return _list[index];
                }
            }

            public TableColumn Last()
            {
                if (Length > 0)
                    return _list[Length - 1];
                else
                    return null;
            }

            public TableColumn Add()
            {
                TableColumn res = new TableColumn(_gridBuilder);
                _list.Add(res);
                return res;
            }

            public void Add(TableColumn col)
            {
                if (Length > 0)
                {
                    TableColumn last = Last();
                    if (last.type == col.type && last.name == col.name &&
                        last.pos == col.pos)
                        last.count++;
                    else
                        _list.Add(col);
                }
                else
                    _list.Add(col);
            }

            #region IEnumerable<TableColumn> Members

            public IEnumerator<TableColumn> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            #endregion
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class NodeList : XmlNodeList, IReadOnlyList<XmlNode>
        {
            private readonly List<XmlNode> _nodes = new List<XmlNode>();

            public override int Count
            {
                get { return _nodes.Count; }
            }

            IEnumerator<XmlNode> IEnumerable<XmlNode>.GetEnumerator()
            {
                return _nodes.GetEnumerator();
            }

            public override IEnumerator GetEnumerator()
            {
                return _nodes.GetEnumerator();
            }

            public override XmlNode Item(int index)
            {
                return _nodes[index];
            }

            public void Add(XmlNode node)
            {
                _nodes.Add(node);
            }
        }

        public enum ItemType
        {
            None,
            Values,
            List,
            Table
        };

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Item
        {
            public ItemType type { get; set; }
            private readonly List<XmlNode> _nodes;

            public Item()
            {
                _nodes = new List<XmlNode>();
            }

            public List<XmlNode> nodes
            {
                get { return _nodes; }
            }

            public override string ToString()
            {
                return string.Format("{0}[{1}], {2}", base.ToString(), _nodes.Count, type);
            }
        }

        //[TypeConverter(typeof(ReadOnlyListConverter))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ItemList : IReadOnlyList<Item>
        {
            private readonly List<Item> _list;

            [Obsolete]
            public Item[] List { get { return _list.ToArray(); } }

            public ItemList()
            {
                _list = new List<Item>();
            }

            public int Length
            {
                get
                {
                    return _list.Count;
                }
            }

            public Item this[int index]
            {
                get
                {
                    return _list[index];
                }
            }

            public Item Last
            {
                get
                {
                    if (Length > 0)
                        return _list[Length - 1];
                    else
                        return null;
                }
            }

            public XmlNode LastNode
            {
                get
                {
                    Item item = Last;
                    if (item != null && item.nodes.Count > 0)
                        return item.nodes[item.nodes.Count - 1];
                    else
                        return null;
                }
            }

            private Item GetItem(ItemType type)
            {
                Item item = Last;
                if (item == null || type != item.type)
                {
                    item = new Item();
                    item.type = type;
                    _list.Add(item);
                }
                return item;
            }

            public void Add(ItemType type, XmlNode node)
            {
                Item item = GetItem(type);
                item.nodes.Add(node);
            }

            public void Add(ItemType type, XmlNodeList nodes)
            {
                Item item = GetItem(type);
                for (int k = 0; k < nodes.Count; k++)
                    item.nodes.Add(nodes.Item(k));
            }

            public void Add(ItemType type, XmlAttributeCollection nodes)
            {
                Item item = GetItem(type);
                for (int k = 0; k < nodes.Count; k++)
                    item.nodes.Add(nodes.Item(k));
            }

            public void Add(ItemType type, XmlNamedNodeMap attrs)
            {
                Item item = GetItem(type);
                for (int k = 0; k < attrs.Count; k++)
                    item.nodes.Add(attrs.Item(k));
            }

            /// <summary>
            /// Split <code>Item.Last()</code> in to two entries containing <code>Item.Last().Select(NotLast)</code> and <code>Item.Last().Last()</code>
            /// </summary>
            public void Fork()
            {
                Item item = Last;
                if (item.nodes.Count > 1)
                {
                    ItemType type = item.type;
                    XmlNode node = LastNode;
                    item.nodes.RemoveAt(item.nodes.Count -1);
                    item = new Item();
                    _list.Add(item);
                    item.type = type;
                    item.nodes.Add(node);
                }
            }

            public int CountCells()
            {
                int res = 0;
                for (int i = 0; i < Length; i++)
                    if (this[i].type == ItemType.List)
                        res += this[i].nodes.Count;
                    else
                        res++;
                return res;
            }

            public IEnumerator<Item> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable) _list).GetEnumerator();
            }

            public int Count
            {
                get { return _list.Count; }
            }
        }

        public GridBuilder()
        {
            
        }

        /// <summary>
        /// TextNode, Attribute or Element containing only text
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsPairNode(XmlNode node)
        {
            if (node is XmlText || node is XmlAttribute)
                return true;
            else
                if (node is XmlElement)
                {
                    XmlElement elem = (XmlElement)node;
                    if (!elem.HasAttributes && (!elem.HasChildNodes ||
                            (elem.ChildNodes.Count == 1 && elem.FirstChild is XmlText)))
                        return true;
                }
            return false;
        }

        /// <summary>
        /// Elements with same name can be grouped
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        protected bool CanGroupNodes(XmlNode node1, XmlNode node2)
        {
            if (node1 != null && node1 is XmlElement &&
                node2 != null && node2 is XmlElement)
            {
                XmlElement elem1 = (XmlElement)node1;
                XmlElement elem2 = (XmlElement)node2;
                if (elem1.Name == elem2.Name)
                    return true;
            }
            return false;
        }

        protected string GetNodeName(XmlNode node)
        {
            if (node is XmlText && node.ParentNode.ChildNodes.Count == 1)
                return node.ParentNode.Name;
            else
                return node.Name;
        }

        protected List<XmlNode> SelectChilds(XmlNode node)
        {
            List<XmlNode> res = new List<XmlNode>();
            foreach (XmlNode child in node.ChildNodes)
                if (!(child is XmlSignificantWhitespace))
                    res.Add(child);
            return res;
        }

        protected TableColumns CreateColumns(XmlNode node)
        {
            TableColumns columns = CreateTableColumns();
            if (node is XmlElement)
            {
                XmlElement elem = (XmlElement)node;
                if (elem.HasAttributes)
                    foreach (XmlAttribute attr in elem.Attributes)
                    {
                        TableColumn col = columns.Add();
                        col.type = XmlNodeType.Attribute;
                        col.name = attr.Name;
                    }
            }
            if (node.HasChildNodes)
            {
                int i = 0;
                List<XmlNode> childs = SelectChilds(node);
                while (i < childs.Count)
                {
                    XmlNode cur = childs[i];
                    TableColumn col = columns.Add();
                    col.type = cur.NodeType;
                    col.name = GetNodeName(cur);
                    col.pos = 0;
                    col.count = 1;
                    int k;
                    for (k = 0; k < i; k++)
                    {
                        cur = childs[k];
                        if (cur.NodeType == col.type && (!(cur is XmlElement) || GetNodeName(cur) == col.name))
                            col.pos++;
                    }
                    k = i + 1;
                    while (k < childs.Count)
                    {
                        cur = childs[k];
                        if (cur.NodeType == col.type && (!(cur is XmlElement) || GetNodeName(cur) == col.name))
                            col.count++;
                        else
                            break;
                        k++;
                    }
                    i = k;
                }
            }
            return columns;
        }

        //protected TableColumns GroupNode(XmlNode node, TableColumns columns, TableRows tableRows)
        protected TableColumns GroupNode(XmlNode node, TableColumns columns)
        {
            TableColumns nodeColumns = CreateColumns(node);
            //tableRows.Add(node, nodeColumns);
            TableColumns res = CreateTableColumns();
            if (columns.Length <= nodeColumns.Length)
            {
                nodeColumns_columns.GroupNode(nodeColumns, columns, res);
            }
            else
            {
                columns_nodeColumns.GroupNode(columns, nodeColumns, res);
            }
            foreach (var tableColumn in nodeColumns)
            {
                tableColumn.rows.Add(node);
            }
            return res;
        }

        private static readonly implement_Merge nodeColumns_columns = new implement_nodeColumns_columns();
        class implement_nodeColumns_columns : implement_Merge
        {
            protected override void TakeB(TableColumn b)
            {
            }

            protected override void TakeA(TableColumn a, TableColumn markedB)
            {
                if (markedB == null)
                    a.rows = new HashSet<XmlNode>();
                else
                    a.rows = markedB.rows;
            }
        }

        private static readonly implement_Merge columns_nodeColumns = new implement_columns_nodeColumns();
        class implement_columns_nodeColumns : implement_Merge
        {
            protected override void TakeB(TableColumn b)
            {
                b.rows = new HashSet<XmlNode>();
            }

            protected override void TakeA(TableColumn a, TableColumn markedB)
            {
                if (markedB != null)
                    markedB.rows = a.rows;
            }
        }

        abstract class implement_Merge
        {
            /// <summary>
            /// Merge two TableColumns
            /// </summary>
            /// <param name="a">the larger TableColumns</param>
            /// <param name="b">the smaller TableColumns</param>
            /// <param name="res"></param>
            public void GroupNode(TableColumns a, TableColumns b, TableColumns res)
            {
                int kUnmarked = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    TableColumn markedB = null;
                    for (int k = kUnmarked; k < b.Length; k++)
                        if (!b[k].marked && b[k].Equals(a[i]))
                        {
                            // if (option.KeepOrder)
                            {
                                for (int s = kUnmarked; s < k - 1; s++)
                                    if (!b[s].marked)
                                    {
                                        res.Add(b[s]);
                                        TakeB(b[s]);
                                        b[s].marked = true;
                                    }
                                kUnmarked = k + 1;
                            }
                            // else b[k].index = i;
                            b[k].marked = true;
                            markedB = b[k];
                            break;
                        }
                    TakeA(a[i], markedB);
                    res.Add(a[i]);
                }
                // todo insert in best order if (option.KeepOrder)
                for (int i = kUnmarked; i < b.Length; i++)
                {
                    if (!b[i].marked)
                    {
                        res.Add(b[i]);
                        TakeB(b[i]);
                    }
                }
            }

            protected abstract void TakeB(TableColumn b);
            protected abstract void TakeA(TableColumn a, TableColumn markedB);
        }


        protected NodeList GetNodeAtColumn(XmlNode node, TableColumn col)
        {
            NodeList res = new NodeList();
            //if (col.type == typeof(XmlAttribute))
            if (col.type == XmlNodeType.Attribute)
            {
                XmlElement elem = (XmlElement)node;
                if (elem.HasAttributes)
                {
                    XmlNode attr = elem.Attributes.GetNamedItem(col.name);
                    if (attr != null)
                        res.Add(attr);
                }
            }
            else
                if (node.HasChildNodes)
                {
                    int pos = col.pos;
                    int count = col.count;
                    List<XmlNode> childs = SelectChilds(node);
                    for (int k = 0; k < childs.Count; k++)
                    {
                        XmlNode cur = childs[k];
                        if (cur.NodeType == col.type && (!(cur is XmlElement) || cur.Name == col.name))
                            if (pos == 0)
                            {
                                int s = k;
                                while (count > 0 && s < childs.Count)
                                {
                                    cur = childs[s];
                                    if (cur.NodeType == col.type && (!(cur is XmlElement) || cur.Name == col.name))
                                    {
                                        res.Add(cur);
                                        count--;
                                    }
                                    else
                                        break;
                                    s++;
                                }
                            }
                            else
                                pos--;
                    }
                }
            return res;
        }

        public void ParseNodes(GridCellGroup cell, XmlNamedNodeMap attrs, XmlNodeList nodes)
        {
            CreateTable(cell, GetItems(attrs, nodes));
        }

        public void ParseNodes(GridCellGroup cell, XmlNode node)
        {
            CreateTable(cell, GetItems(node.Attributes, node.ChildNodes));
        }

        //internal ItemList GetItems(XmlNode node, bool shortcut = false)
        //{
        //    return GetItems(node.Attributes, node.ChildNodes);
        //}

        //internal ItemList GetItems(XmlNamedNodeMap attrs, XmlNodeList nodes, bool shortcut = false)
        internal ItemList GetItems(XmlNamedNodeMap attrs, IEnumerable nodes, ContentTypeOptions options = ContentTypeOptions.None)
        {
            ItemList items = new ItemList();
            bool shortcut = options.HasFlag(ContentTypeOptions.Shortcut);
            if (attrs != null && attrs.Count > 0)
            {
                if (shortcut)
                    return null;
                items.Add(ItemType.Values, attrs);
            }
            foreach (XmlNode child in nodes)
            {
                if (child is XmlSignificantWhitespace)
                    continue;
                if (CanGroupNodes(items.LastNode, child))
                {
                    if (items.Last.type != ItemType.Table)
                    {
                        items.Fork();
                        items.Last.type = ItemType.Table;
                    }
                    items.Add(ItemType.Table, child);
                }
                else if ((child.NodeType != XmlNodeType.Text && IsPairNode(child)) ||
                         child.NodeType == XmlNodeType.XmlDeclaration ||
                         child.NodeType == XmlNodeType.DocumentType ||
                         child.NodeType == XmlNodeType.ProcessingInstruction)
                    items.Add(ItemType.Values, child);
                else
                    items.Add(ItemType.List, child);
                if (shortcut)
                {
                    if (ContentType(items, options).HasFlag(ContentTypeResult.Complex)) 
                        return null;
                }
            }
            return items;
        }

        [Flags]
        public enum ContentTypeResult
        {
            Unknown = 0,
            Empty = 1,
            SingleChild = 2,
            Complex = 4,
            Leaf = 8,
            NoExpand = 16,
            Table = 32,
            List = 64,
        }

        [Flags]
        public enum ContentTypeOptions
        {
            None = 0,
            AllowText = 1,
            AllowEmpty = 2,
            Shortcut = 4
        }

        internal ContentTypeResult ContentType(ItemList items, ContentTypeOptions options = ContentTypeOptions.None)
        {
            if (items == null)
                return ContentTypeResult.Complex;
            if (items.Length == 0)
            {
                if (options.HasFlag(ContentTypeOptions.AllowEmpty))
                    return ContentTypeResult.Empty;
                return ContentTypeResult.Complex;
            }
            if (items.Length != 1)
                return ContentTypeResult.Complex;
            var last = items.Last;
            if (last.type != ItemType.Table && last.nodes.Count > 1)
                return ContentTypeResult.Complex;
            if (items.LastNode.NodeType == XmlNodeType.Element)
                return ContentTypeResult.SingleChild;
            return ContentTypeResult.Leaf;
        }

        internal void CreateTable(GridCellGroup cell, ItemList items)
        {
            // save items for debugging purposes
            cell.Tag = items;
            if (cell.SerialNumberBreakCreateTable == cell.SerialNumber)
                Debugger.Break();

            if (items.Length == 1 && items[0].type == ItemType.Values)
            {
                cell.Table.SetBounds(2, items[0].nodes.Count);
                for (int s = 0; s < items[0].nodes.Count; s++)
                {
                    XmlNode node = items[0].nodes[s];
                    cell.Table[0, s] = new XmlLabelCell(node);
                    cell.Table[1, s] = new XmlValueCell(node);
                }
            }
            else
            {
                int k = 0;
                cell.Table.SetBounds(1, items.CountCells());
                for (int i = 0; i < items.Length; i++)
                {
                    Item item = items[i];
                    switch (item.type)
                    {
                        case ItemType.Values:
                            {
                                GridCellGroup group = CreateGridCellGroup();
                                group.Flags = GroupFlags.Expanded | GroupFlags.Overlapped;
                                group.Table.SetBounds(2, item.nodes.Count);
                                for (int s = 0; s < item.nodes.Count; s++)
                                {
                                    XmlNode node = item.nodes[s];
                                    group.Table[0, s] = new XmlLabelCell(node);
                                    if (node.NodeType == XmlNodeType.XmlDeclaration ||
                                        node.NodeType == XmlNodeType.DocumentType)
                                        group.Table[1, s] = new XmlDeclarationCell(node);
                                    else
                                        group.Table[1, s] = new XmlValueCell(node);
                                }
                                cell.Table[0, k++] = group;
                            }
                            break;

                        case ItemType.List:
                            for (int s = 0; s < item.nodes.Count; s++)
                            {
                                var node = item.nodes[s];
                                if (node.NodeType == XmlNodeType.Element)
                                {
                                    //var pathCell = TryCreateXmlPathCell(node);
                                    //if (pathCell != null)
                                    //    cell.Table[0, k++] = pathCell;
                                    //else
                                    //    cell.Table[0, k++] = CreateXmlGroupCell(node);
                                    //cell.Table[0, k++] = CreateXmlGroupCell(node);
                                    cell.Table[0, k++] = CreateXmlPathOrGroupCell(node);
                                }
                                else
                                    cell.Table[0, k++] = new XmlLabelCell(node);
                            }
                            break;

                        case ItemType.Table:
                            {
                                GridCellGroup group = CreateXmlGroupCell(item.nodes[0]); // item.nodes[0] is just the first of many!
                                CreateTableView(group, item);
                                cell.Table[0, k++] = group;
                            }
                            break;

                        default:
                            throw new InvalidOperationException("Unknown item.type: " + item.type);
                    }
                }
            }
        }

        internal TableColumns CreateColumnHeaderRow(XmlColumnLabelCell group, ICollection<XmlNode> nodes)
        {
            //return CreateTableHeaderRow(group, nodes, false);
            group.Flags = group.Flags | GroupFlags.TableView | GroupFlags.ColumnsOverlapped;
            TableColumns tableColumns = CreateTableColumns();
            foreach (var t in nodes)
                tableColumns = GroupNode(t, tableColumns);
            group.Table.SetBounds(tableColumns.Length + 1, 1);
            CreateTableHeaderRow(group, tableColumns);
            return tableColumns;
        }

        //internal TableColumns CreateTableHeaderRow(GridCellGroup group, ICollection<XmlNode> nodes, bool hasValues)
        //{
        //    group.Flags = group.Flags | GroupFlags.TableView;
        //    TableColumns tableColumns = CreateTableColumns();
        //    var tableRows = new TableRows();
        //    foreach (var t in nodes)
        //        tableColumns = GroupNode(t, tableColumns, tableRows);
        //    var height = 1;
        //    if (hasValues)
        //        height += nodes.Count;
        //    CreateTableHeaderRow(group, tableColumns, height);
        //    return tableColumns;
        //}

        private void CreateTableHeaderRow(GridCellGroup group, TableColumns tableColumns)
        {
            group.Table[0, 0] = new GridRowLabel();
            for (int s = 0; s < tableColumns.Length; s++)
            {
                var column = tableColumns[s];
                var nodeName = column.name;
                var contentType = column.ContentType(ContentTypeOptions.AllowEmpty);
                if (column.path != null)
                {
                    nodeName = string.Format("./{0}", string.Join("/", column.path));
                }
                var cell = new XmlColumnLabelCell(this, column.type, nodeName, column.pos, column);
                //if (contentType.HasFlag(ContentTypeResult.NoExpand))
                if ((contentType & ~ContentTypeResult.Empty) == (ContentTypeResult.Leaf | ContentTypeResult.List))
                    cell.Flags |= GroupFlags.NoExpand;
                group.Table[s + 1, 0] = cell;
                cell.Tag = column;
            }
        }

        internal void CreateTableView(GridCellGroup group, Item item)
        {
            TableColumns tableColumns = CreateTableColumns();
            foreach (var t in item.nodes)
                tableColumns = GroupNode(t, tableColumns);
            CreateTableView(group, item, tableColumns);
        }

        internal void CreateTableView(GridCellGroup group, Item item, TableColumns tableColumns)
        {
            group.Flags = group.Flags | GroupFlags.TableView;
            group.Table.SetBounds(tableColumns.Length + 1, item.nodes.Count + 1);
            CreateTableHeaderRow(group, tableColumns);
            for (int s = 0; s < item.nodes.Count; s++)
            {
                XmlNode node = item.nodes[s];
                group.Table[0, s + 1] = new XmlRowLabelCell(s + 1, node);
                for (int p = 0; p < tableColumns.Length; p++)
                {
                    NodeList nodeList = GetNodeAtColumn(node, tableColumns[p]);
                    if (nodeList.Count == 0)
                    {
                        group.Table[p + 1, s + 1] = new XmlValueCell(null);
                    }
                    else
                    {
                        XmlNode child = nodeList[0];
                        if (nodeList.Count == 1)
                        {
                            // todo HasChildNodes should be member of GridBuilder and check for significant space
                            if (child.NodeType == XmlNodeType.Element && !child.HasChildNodes)
                                group.Table[p + 1, s + 1] = new XmlLabelCell(child);
                            else if (child.NodeType != XmlNodeType.Element || IsPairNode(child))
                                group.Table[p + 1, s + 1] = new XmlValueCell(child);
                            else
                                group.Table[p + 1, s + 1] = CreateXmlPathOrGroupCell(child);
                        }
                        else
                        {
                            XmlGroupCell childGroup = CreateXmlGroupCell(child);
                            childGroup.Flags = GroupFlags.Overlapped | GroupFlags.Expanded;
                            group.Table[p + 1, s + 1] = childGroup;
                            ParseNodes(childGroup, null, nodeList);
                        }
                    }
                }
            }
        }

        private XmlGroupCell CreateXmlPathOrGroupCell(XmlNode node)
        {
            var items = GetItems(node.Attributes, node.ChildNodes, ContentTypeOptions.Shortcut);
            if (ContentType(items).HasFlag(ContentTypeResult.Complex))
                return CreateXmlGroupCell(node);
            var path = new List<XmlNode> {node};
            if (items == null)
                items = GetItems(node.Attributes, node.ChildNodes);
            while (items.Last.type == ItemType.List)
            {
                var n = items.LastNode;
                path.Add(n);
                var i = GetItems(n.Attributes, n.ChildNodes, ContentTypeOptions.Shortcut);
                if (ContentType(i).HasFlag(ContentTypeResult.Complex))
                    break;
                items = i ?? GetItems(n.Attributes, n.ChildNodes);
            }
            XmlPathCell pathCell;
            if (items.Last.type == ItemType.Table)
                pathCell = CreateXmlPathCell(path.ToArray(), items.LastNode.Name, items.Last.nodes.Count);
            else
                pathCell = CreateXmlPathCell(path.ToArray());
            pathCell.Items = items;
            return pathCell;
        }

        private XmlGroupCell CreateXmlGroupCell(XmlNode node)
        {
            return new XmlGroupCell(this, node);
        }

        private XmlPathCell CreateXmlPathCell(XmlNode[] path, string name = null, int? count = null)
        {
            if (name == null)
                return new XmlPathCell(this, path, null, path.Last(), count);
            return new XmlPathCell(this, path, name, null, null);
        }

        public GridCellGroup CreateGridCellGroup()
        {
            return new GridCellGroup(this);
        }

        public void ParseNodes(GridCellGroup xmlgroup, XmlNamedNodeMap attrs, XPathNavigator createNavigator)
        {
            throw new NotImplementedException();
        }
    }

    class XmlPathCell : XmlGroupCell
    {
        private XmlNode[] _path;
        private readonly string _name;
        private readonly int? _count;

        protected internal XmlPathCell(GridBuilder gridBuilder, XmlNode[] path, string name, XmlNode node, int? count)
            : base(gridBuilder, node)
        {
            _path = path;
            _name = name;
            _count = count;
        }

        public override string Text
        {
            get
            {
                var text = "./" + string.Join("/", _path.Select(GetName).ToArray());
                if (_name != null)
                    text += "/" + _name;
                return text;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public override void BeforeExpand()
        {
            if (Table.IsEmpty)
            {
                GridBuilder.CreateTableView(this, Items[0]);
            }
        }

        protected override int TableViewCount()
        {
            return _count ?? base.TableViewCount();
        }

        private string GetName(XmlNode node)
        {
            return node.Name;
        }

        public override void CopyToClipboard()
        {
            base.CopyToClipboard(_path[0]);
        }
    }

    class ReadOnlyListConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof (string))
            {
                int? count = null;
                var type = value.GetType();
                var collection = value as ICollection;
                if (collection != null)
                {
                    count = collection.Count;
                }
                else
                {
                    if (type.IsGenericType)
                    {
                        var genericTypeDefinition = type.GetGenericTypeDefinition();
                        if (typeof (IReadOnlyCollection<>).IsAssignableFrom(genericTypeDefinition))
                        {
                            count = (int)type.GetProperty("Count").GetValue(value);
                        }
                    }
                }
                if (count.HasValue)
                    return string.Format("{0} [{1}]", type, count);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var list = value as IReadOnlyList<object>;
            if (list != null)
            {
                var objects = list.ToArray();
                var typeConverter = TypeDescriptor.GetConverter(objects);
                return typeConverter.GetProperties(context, objects, attributes);
            }
            return base.GetProperties(context, value, attributes);
        }
    }
}
