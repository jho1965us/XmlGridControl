using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WmHelp.XmlGrid;

namespace WmHelp.DataGrid
{
    public class MemberLabelCell: GridCell
    {
        public MemberNode Node { get; private set; }

        public MemberLabelCell(MemberNode node)
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
                    case MemberNodeType.Leaf:
                        return 2; // =
                    default:
                        return 3; // <>
                }
            }
        }

        public override string Text
        {
            get
            {
                return Node.Name;
            }
            set
            {
                return;
            }
        }

        public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font, Brush brush, 
            StringFormat sf, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            if (false)
                font = new Font(font, FontStyle.Italic);
            base.DrawCellText(gridView, graphics, font, brush, sf, drawInfo, rect);
        }
    }

    public class MemberValueCell : GridCell
    {
        public MemberNode Node { get; private set; }

        public MemberValueCell(MemberNode node)
        {
            Node = node;
        }

        public override string Text
        {
            get
            {
                return Node.Value.ToString();
            }
            set
            {
                throw new InvalidOperationException();
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

    public class MemberGroupCell : GridCellGroup
    {
        protected GridBuilder GridBuilder { get; private set; }
        public MemberNode Node { get; private set; }

        protected internal MemberGroupCell(GridBuilder gridBuilder, MemberNode node) : base(gridBuilder)
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
                return Node.ToString();
            }
        }

        public override void BeforeExpand()
        {
            if (Table.IsEmpty)
            {
                if (this.Items == null)
                    this.Items = GridBuilder.GetItems(Node.ChildNodes);
                GridBuilder.CreateTable(this, (GridBuilder.ItemList)this.Items);
            }
        }

        public override void CopyToClipboard()
        {
        }


        protected override void CreateTableView(GridCellGroup inner, GridColumnLabel columnHeader)
        {
            var group = (MemberColumnLabelCell)columnHeader;
            if (Items == null)
            {
                Items = group.GridBuilder.GetItems(new[] { Node });
                Items.Last.type = GridBuilder.ItemType.Table;
            }
            group.GridBuilder.CreateTableView(inner, Items.Last, @group.TableColumns);
            //_inner.Flags |= GroupFlags.Overlapped | GroupFlags.NoColumnHeader | GroupFlags.Expanded;
            //_inner.Flags |= GroupFlags.ColumnsOverlapped | GroupFlags.Expanded;
        }
    }

    public class MemberColumnLabelCell : GridColumnLabel
    {
        public GridBuilder GridBuilder { get; private set; }
        private readonly GridBuilder.TableColumn _column;
        public MemberNodeType NodeType { get; private set; }
        public String NodeName { get; private set; }

        internal MemberColumnLabelCell(GridBuilder gridBuilder, MemberNodeType type, string nodeName, GridBuilder.TableColumn column) : base(gridBuilder)
        {
            GridBuilder = gridBuilder;
            _column = column;
            NodeType = type;
            switch (type)
            {
                case MemberNodeType.Leaf:
                    Flags |= GroupFlags.NoExpand;
                    break;
                default:
                    break;
            }
            NodeName = nodeName;
        }

        public override int ImageIndex
        {
            get
            {
                switch (NodeType)
                {
                    case MemberNodeType.Leaf:
                        return 2;
                    default:
                        return 3;
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

        public override string Text
        {
            get
            {
                return NodeName;
            }
            set
            {
                base.Text = value;
            }
        }

        public override void DrawCellText(XmlGridView gridView, Graphics graphics, Font font, 
            Brush brush, StringFormat sf, XmlGridView.DrawInfo drawInfo, Rectangle rect)
        {
            font = new Font(font, FontStyle.Bold);
            base.DrawCellText(gridView, graphics, font, brush, sf, drawInfo, rect);
        }

    }

    public class MemberRowLabelCell : GridRowLabel
    {
        public Member Node { get; private set; }

        public MemberRowLabelCell(int row, Member node)
        {
            RowNum = row;
            Node = node;
        }
    }

    public class MemberNode
    {
        private object _value;
        private MemberNode parent;
        private readonly Member _member;
        private bool _hasValue;
        private MemberNode[] _childNodes;

        public MemberNode(object value, Member member)
        {
            _value = value;
            _hasValue = true;
            _member = member;
        }

        public bool HasChildNodes { get { return Member.HasChildNodes && TypeNode.GetTypeNode(Value).Members.Length > 0; } }
        public string Name { get { return Member.Name; } }
        public MemberNodeType NodeType { get { return Member.NodeType; }}

        public IList<MemberNode> ChildNodes
        {
            get
            {
                if (_childNodes == null)
                {
                    var type = TypeNode.GetTypeNode(Value);
                    _childNodes = type.Members.Select(GetValue).ToArray();
                }
                return _childNodes;
            }
        }

        private MemberNode GetValue(Member member)
        {
            return new MemberNode(member.GetValue(Value), member);
        }

        public object Value
        {
            get
            {
                if (!_hasValue)
                {
                    _value = Member.GetValue(parent.Value);
                }
                return _value;
            }
        }

        public Member Member
        {
            get { return _member; }
        }
    }

    class TypeNode
    {
        private static readonly Dictionary<Type, TypeNode> _types = new Dictionary<Type, TypeNode>();
        public readonly Member[] Members;

        public TypeNode(Member[] members)
        {
            Members = members;
        }

        public static TypeNode GetTypeNode(object node)
        {
            TypeNode typeNode;
            if (!_types.TryGetValue(node.GetType(), out typeNode))
            {
                var members = new List<Member>();
                foreach (var fieldInfo in node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic))
                {
                    members.Add(new FieldMemberNode(fieldInfo));
                }
                foreach (var propertyInfo in node.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (propertyInfo.GetIndexParameters().Length > 0)
                        continue;
                    members.Add(new PropertyMemberNode(propertyInfo));
                }
                members.Sort(Comparison);
                typeNode = new TypeNode(members.ToArray());
                
            }
            return typeNode;
        }

        static int Comparison(Member a, Member b)
        {
            return String.Compare(a.Name, b.Name, StringComparison.InvariantCulture);
        }
    }

    public enum MemberNodeType
    {
        Leaf,
        Complex,
        Collection
    }

    public abstract class Member
    {
        public abstract string Name { get; }
        public abstract Type MemberType { get; }
        public bool HasChildNodes { get { return NodeType != MemberNodeType.Leaf; } }

        public MemberNodeType NodeType
        {
            get
            {
                var type = MemberType;
                if (typeof (bool) == type ||
                    typeof (char) == type ||
                    typeof (short) == type ||
                    typeof (int) == type ||
                    typeof (long) == type ||
                    typeof (ushort) == type ||
                    typeof (uint) == type ||
                    typeof (ulong) == type ||
                    typeof (float) == type ||
                    typeof (double) == type ||
                    typeof (decimal) == type ||
                    typeof (string) == type)
                    return MemberNodeType.Leaf;
                if (typeof (ICollection).IsAssignableFrom(type))
                    return MemberNodeType.Collection;
                if (!HasOtherMembers(type))
                    return MemberNodeType.Leaf;
                return MemberNodeType.Complex;
            }
        }

        static bool HasOtherMembers(Type type, params string[] skipMembers)
        {
            foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (skipMembers.Contains(fieldInfo.Name))
                    continue;
                return true;
            }
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (propertyInfo.GetIndexParameters().Length > 0)
                    continue;
                if (skipMembers.Contains(propertyInfo.Name))
                    continue;
                return true;
            }
            return false;
        }

        public abstract object GetValue(object obj);
    }

    class FieldMemberNode : Member
    {
        private readonly FieldInfo _fieldInfo;

        public FieldMemberNode(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public override string Name
        {
            get { return _fieldInfo.Name; }
        }

        public override Type MemberType
        {
            get { return _fieldInfo.FieldType; }
        }

        public override object GetValue(object obj)
        {
            return _fieldInfo.GetValue(obj);
        }
    }

    class PropertyMemberNode : Member
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyMemberNode(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public override string Name
        {
            get { return _propertyInfo.Name; }
        }

        public override Type MemberType
        {
            get { return _propertyInfo.PropertyType; }
        }

        public override object GetValue(object obj)
        {
            return _propertyInfo.GetValue(obj);
        }
    }

    public class GridBuilder : GridBuilderBase
    {
        /// <summary>
        /// Equals if type, name, pos, count equals
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        protected internal class TableColumn
        {
            public MemberNodeType type;
            public Member member;
            public String name;
            public bool marked;
            public HashSet<MemberNode> rows; 
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
                        member == c.member;
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
                public MemberNodeType NodeType { get; set; }
                public string Name { get; set; }
                public ItemType ItemType { get; set; }
                public ContentTypeResult ContentType { get; set; }

                public readonly Dictionary<MemberNode, ItemList> rows = new Dictionary<MemberNode, ItemList>();

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

                public bool IsSimpleContentType1(MemberNode row)
                {
                    ItemList items = null;
                    foreach (MemberNode child in row.ChildNodes)
                    {
                        if (child.Name == _column.name)
                        {
                            if (items != null)
                                return false;
                            items = _column._gridBuilder.GetItems(child.ChildNodes, _options | ContentTypeOptions.Shortcut);
                            ContentType |= _column._gridBuilder.ContentType(items, _options);
                            if (ContentType.HasFlag(ContentTypeResult.Complex))
                                return false;
                        }
                    }
                    if (_options.HasFlag(ContentTypeOptions.AllowEmpty) && items == null)
                        return false;
                    return IsSimpleContentType(row, items);
                }

                private bool IsSimpleContentType(MemberNode row, ItemList items)
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
                rowItems = builder.ChildItems;
                return rowItems.ContentType;
            }

            public ContentTypeBuilder builder { get; private set; }

            public ICollection<MemberNode> GetNodes()
            {
                if (path != null)
                {
                    //var capacity = 0;
                    //foreach (var itemList in rowItems.rows.Values)
                    //{
                    //    if (itemList.Last != null)
                    //        capacity += itemList.Last.nodes.Count;
                    //}
                    //var nodes = new List<XmlNode>(capacity);
                    //foreach (var itemList in rowItems.rows.Values)
                    //{
                    //    if (itemList.Last != null)
                    //        nodes.AddRange(itemList.Last.nodes);
                    //}
                    //return nodes;
                    throw new InvalidOperationException();
                }

                {
                    var capacity = rows.Count;
                    var nodes = new List<MemberNode>(capacity);
                    foreach (var row in rows)
                    {
                        foreach (MemberNode child in row.ChildNodes)
                        {
                            if (child.NodeType == type)
                            {
                                if (member == child.Member)
                                    continue;
                                nodes.Add(child);
                            }
                        }
                    }
                    return nodes;
                }
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

            public TableColumn Add()
            {
                TableColumn res = new TableColumn(_gridBuilder);
                _list.Add(res);
                return res;
            }

            public void Add(TableColumn col)
            {
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
        public class NodeList : IReadOnlyList<MemberNode>
        {
            private readonly List<MemberNode> _nodes = new List<MemberNode>();

            public int Count
            {
                get { return _nodes.Count; }
            }

            IEnumerator<MemberNode> IEnumerable<MemberNode>.GetEnumerator()
            {
                return _nodes.GetEnumerator();
            }

            public IEnumerator GetEnumerator()
            {
                return _nodes.GetEnumerator();
            }

            public void Add(MemberNode node)
            {
                _nodes.Add(node);
            }

            public MemberNode this[int index]
            {
                get { return _nodes[index]; }
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
            private readonly List<MemberNode> _nodes;

            public Item()
            {
                _nodes = new List<MemberNode>();
            }

            public List<MemberNode> nodes
            {
                get { return _nodes; }
            }

            public override string ToString()
            {
                return string.Format("{0}[{1}], {2}", base.ToString(), _nodes.Count, type);
            }
        }

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

            public MemberNode LastNode
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
                    item = NewItem(type);
                }
                return item;
            }

            public Item NewItem(ItemType type)
            {
                var item = new Item();
                item.type = type;
                _list.Add(item);
                return item;
            }

            public void Add(ItemType type, MemberNode node)
            {
                Item item = GetItem(type);
                item.nodes.Add(node);
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


        /// <summary>
        /// TextNode, Attribute or Element containing only text
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsPairNode(MemberNode node)
        {
            if (node.NodeType == MemberNodeType.Leaf)
                return true;
            return false;
        }

        protected List<MemberNode> SelectChilds(MemberNode node)
        {
            List<MemberNode> res = new List<MemberNode>();
            foreach (MemberNode child in node.ChildNodes)
                res.Add(child);
            return res;
        }

        protected TableColumns CreateColumns(MemberNode node)
        {
            TableColumns columns = CreateTableColumns();
            if (node.HasChildNodes)
            {
                int i = 0;
                List<MemberNode> childs = SelectChilds(node);
                while (i < childs.Count)
                {
                    MemberNode cur = childs[i];
                    TableColumn col = columns.Add();
                    col.type = cur.NodeType;
                    col.name = node.Name;
                }
            }
            return columns;
        }

        protected TableColumns GroupNode(MemberNode node, TableColumns columns)
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
                    a.rows = new HashSet<MemberNode>();
                else
                    a.rows = markedB.rows;
            }
        }

        private static readonly implement_Merge columns_nodeColumns = new implement_columns_nodeColumns();
        class implement_columns_nodeColumns : implement_Merge
        {
            protected override void TakeB(TableColumn b)
            {
                b.rows = new HashSet<MemberNode>();
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


        protected NodeList GetNodeAtColumn(MemberNode node, TableColumn col)
        {
            NodeList res = new NodeList();
            if (node.HasChildNodes)
            {
                List<MemberNode> childs = SelectChilds(node);
                for (int k = 0; k < childs.Count; k++)
                {
                    MemberNode cur = childs[k];
                    if (cur.Member == col.member)
                    {
                        res.Add(cur);
                        break;
                    }
                }
            }
            return res;
        }

        public void ParseNodes(GridCellGroup cell, object node)
        {
            CreateTable(cell, GetItems(node));
        }

        public void ParseNodes(GridCellGroup cell, MemberNode[] members)
        {
            CreateTable(cell, GetItems(members));
        }

        internal ItemList GetItems(object node, ContentTypeOptions options = ContentTypeOptions.None)
        {
            var memberNode = new MemberNode(node, null);
            return GetItems(memberNode.ChildNodes, options);
        }

        internal ItemList GetItems(MemberNode[] members, ContentTypeOptions options = ContentTypeOptions.None)
        {
            ItemList items = new ItemList();
            foreach (var child in members)
            {
                //if (member is XmlSignificantWhitespace)
                //    continue;
                if (child.NodeType == MemberNodeType.Collection)
                {
                    items.NewItem(ItemType.Table);
                    items.Add(ItemType.Table, child);
                }
                else
                    items.Add(ItemType.Values, child);
            }
            return items;
        }

        [Flags]
        public enum ContentTypeResult
        {
            Unknown = 0,
            Empty = 1,
            Complex = 4,
            Leaf = 8,
            NoExpand = 16,
            Table = 32,
            List = 64,
        }

        [Flags]
        public enum ContentTypeOptions
        {
            None,
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
                    MemberNode node = items[0].nodes[s];
                    cell.Table[0, s] = new MemberLabelCell(node);
                    cell.Table[1, s] = new MemberValueCell(node);
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
                                    MemberNode node = item.nodes[s];
                                    group.Table[0, s] = new MemberLabelCell(node);
                                    group.Table[1, s] = new MemberValueCell(node);
                                }
                                cell.Table[0, k++] = group;
                            }
                            break;

                        case ItemType.Table:
                            {
                                GridCellGroup group = CreateMemberGroupCell(item.nodes[0]); // item.nodes[0] is just the first of many!
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

        internal TableColumns CreateColumnHeaderRow(MemberColumnLabelCell group, ICollection<MemberNode> nodes)
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
                var cell = new MemberColumnLabelCell(this, column.type, nodeName, column);
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
                MemberNode node = item.nodes[s];
                group.Table[0, s + 1] = new MemberRowLabelCell(s + 1, node.Member);
                for (int p = 0; p < tableColumns.Length; p++)
                {
                    NodeList nodeList = GetNodeAtColumn(node, tableColumns[p]);
                    if (nodeList.Count == 0)
                    {
                        group.Table[p + 1, s + 1] = new MemberValueCell(null);
                    }
                    else
                    {
                        MemberNode child = nodeList[0];
                        if (nodeList.Count == 1)
                        {
                            // todo HasChildNodes should be member of GridBuilder and check for significant space
                            if (!child.HasChildNodes && !IsPairNode(child))
                                group.Table[p + 1, s + 1] = new MemberLabelCell(child);
                            else if (IsPairNode(child))
                                group.Table[p + 1, s + 1] = new MemberValueCell(child);
                            else
                                group.Table[p + 1, s + 1] = CreateMemberGroupCell(child);
                        }
                        else
                        {
                            MemberGroupCell childGroup = CreateMemberGroupCell(child);
                            childGroup.Flags = GroupFlags.Overlapped | GroupFlags.Expanded;
                            group.Table[p + 1, s + 1] = childGroup;
                            ParseNodes(childGroup, nodeList);
                        }
                    }
                }
            }
        }


        private MemberGroupCell CreateMemberGroupCell(MemberNode node)
        {
            return new MemberGroupCell(this, node);
        }

        public GridCellGroup CreateGridCellGroup()
        {
            return new GridCellGroup(this);
        }
    }
}
