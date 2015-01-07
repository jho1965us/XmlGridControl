using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Net;

using WmHelp.XmlGrid;
using System.Reflection;

namespace XmlGridDemo
{
    public partial class Form1 : Form
    {

        public XmlGridView xmlGrid;
        private string _fileName;

        public Form1()
        {            
            InitializeComponent();

            this.Text = "XmlGridControl demo";

            xmlGrid = new XmlGridView();
            xmlGrid.Dock = DockStyle.Fill;
            xmlGrid.Location = new Point(0, 100);
            xmlGrid.Name = "xmlGridView1";
            xmlGrid.Size = new Size(100, 100);
            xmlGrid.TabIndex = 0;
            xmlGrid.AutoHeightCells = true;
            xmlGrid.UseSingleColumnTrees = true;
            xmlGridPanel.Controls.Add(xmlGrid); 
            xmlGrid.FocusedCellChanged += XmlGridOnFocusedCellChanged;
            viewPropertyGrid.SelectedObject = xmlGrid;
        }

        private void XmlGridOnFocusedCellChanged(object sender, EventArgs eventArgs)
        {
            cellPropertyGrid.SelectedObject = xmlGrid.FocusedCell;
            if (xmlGrid.FocusedCell != null)
            {
                cellPropertyGridPathTextBox.Text = string.Format("{0}: {1}", xmlGrid.FocusedCell.FullText,
                    xmlGrid.FocusedCell.GetType().Name);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadXml(dialog.FileName);
                reloadToolStripMenuItem.Enabled = true;
            }
        }

        private void LoadXml(string fileName)
        {
            _fileName = fileName;
            xmlGrid.Clear();
            GridCell.LastSerialNumber = 0;
            XmlDataDocument xmldoc = new XmlDataDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.ProhibitDtd = false;
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = CredentialCache.DefaultCredentials;
            settings.XmlResolver = resolver;
            XmlReader render = XmlReader.Create(fileName, settings);
            try
            {
                try
                {
                    xmldoc.Load(render);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            finally
            {
                render.Close();
            }
            GridBuilder builder = new GridBuilder();
            builderPropertyGrid.SelectedObject = builder;
            if (xmlGrid.ShowColumnHeader)
            {
                GridCellGroup xmlgroup = builder.CreateGridCellGroup();
                xmlgroup.Flags = GroupFlags.Overlapped | GroupFlags.Expanded;
                builder.ParseNodes(xmlgroup, null, xmldoc.ChildNodes);
                builder.ParseNodes(xmlgroup, null, xmldoc.CreateNavigator());
                GridCellGroup root = builder.CreateGridCellGroup();
                root.Table.SetBounds(1, 2);
                root.Table[0, 0] = new GridHeadLabel();
                root.Table[0, 0].Text = fileName;
                root.Table[0, 1] = xmlgroup;
                xmlGrid.Cell = root;
            }
            else
            {
                GridCellGroup root = builder.CreateGridCellGroup();
                builder.ParseNodes(root, null, xmldoc.ChildNodes);
                xmlGrid.Cell = root;
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly asm = Assembly.GetAssembly(typeof(XmlGridView));
            string title = "XmlGridControl";
            MessageBox.Show(
                String.Format("{0} {1}\n", title, asm.GetName().Version) +
                "Copyright © Semyon A. Chertkov 2009\n" +
                "e-mail: semyonc@gmail.com",
                "About " + Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            GC.Collect();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadXml(_fileName);
        }

        private void invalidateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            xmlGrid.Invalidate();
        }

        private void measureCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            xmlGrid.MeasureCells();
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void collapseToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void expandSubtreeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void collapseSubtreeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void expandColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void collapseColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void expandColumnSubtreeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void collapseColumnSubtreeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void expandTableRowsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void collapseTableRowsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

        }
    }
}
