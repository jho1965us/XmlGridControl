namespace XmlGridDemo
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invalidateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.measureCellsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.xmlGridPanel = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.propertyGridPanel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.viewTabPage = new System.Windows.Forms.TabPage();
            this.viewPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.cellTabPage = new System.Windows.Forms.TabPage();
            this.cellPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.cellPropertyGridPathTextBox = new System.Windows.Forms.TextBox();
            this.loadTabPage = new System.Windows.Forms.TabPage();
            this.propertyTabPage = new System.Windows.Forms.TabPage();
            this.builderTabPage = new System.Windows.Forms.TabPage();
            this.builderPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandSubtreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseSubtreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandColumnSubtreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseColumnSubtreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandTableRowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseTableRowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.propertyGridPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.viewTabPage.SuspendLayout();
            this.cellTabPage.SuspendLayout();
            this.builderTabPage.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.invalidateToolStripMenuItem,
            this.measureCellsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(668, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(109, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Enabled = false;
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // invalidateToolStripMenuItem
            // 
            this.invalidateToolStripMenuItem.Name = "invalidateToolStripMenuItem";
            this.invalidateToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.invalidateToolStripMenuItem.Text = "Invalidate";
            this.invalidateToolStripMenuItem.Click += new System.EventHandler(this.invalidateToolStripMenuItem_Click);
            // 
            // measureCellsToolStripMenuItem
            // 
            this.measureCellsToolStripMenuItem.Name = "measureCellsToolStripMenuItem";
            this.measureCellsToolStripMenuItem.Size = new System.Drawing.Size(89, 20);
            this.measureCellsToolStripMenuItem.Text = "MeasureCells";
            this.measureCellsToolStripMenuItem.Click += new System.EventHandler(this.measureCellsToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.propertyGridPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(668, 475);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.xmlGridPanel);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(668, 290);
            this.panel2.TabIndex = 0;
            // 
            // xmlGridPanel
            // 
            this.xmlGridPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.xmlGridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlGridPanel.Location = new System.Drawing.Point(0, 20);
            this.xmlGridPanel.Name = "xmlGridPanel";
            this.xmlGridPanel.Size = new System.Drawing.Size(668, 270);
            this.xmlGridPanel.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(668, 20);
            this.textBox1.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 290);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(668, 3);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            // 
            // propertyGridPanel
            // 
            this.propertyGridPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.propertyGridPanel.Controls.Add(this.tabControl1);
            this.propertyGridPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.propertyGridPanel.Location = new System.Drawing.Point(0, 293);
            this.propertyGridPanel.Name = "propertyGridPanel";
            this.propertyGridPanel.Size = new System.Drawing.Size(668, 182);
            this.propertyGridPanel.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.viewTabPage);
            this.tabControl1.Controls.Add(this.cellTabPage);
            this.tabControl1.Controls.Add(this.loadTabPage);
            this.tabControl1.Controls.Add(this.propertyTabPage);
            this.tabControl1.Controls.Add(this.builderTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(666, 180);
            this.tabControl1.TabIndex = 0;
            // 
            // viewTabPage
            // 
            this.viewTabPage.Controls.Add(this.viewPropertyGrid);
            this.viewTabPage.Location = new System.Drawing.Point(4, 22);
            this.viewTabPage.Name = "viewTabPage";
            this.viewTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.viewTabPage.Size = new System.Drawing.Size(658, 154);
            this.viewTabPage.TabIndex = 0;
            this.viewTabPage.Text = "XmlGridView";
            this.viewTabPage.UseVisualStyleBackColor = true;
            // 
            // viewPropertyGrid
            // 
            this.viewPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.viewPropertyGrid.Name = "viewPropertyGrid";
            this.viewPropertyGrid.Size = new System.Drawing.Size(652, 148);
            this.viewPropertyGrid.TabIndex = 0;
            // 
            // cellTabPage
            // 
            this.cellTabPage.Controls.Add(this.cellPropertyGrid);
            this.cellTabPage.Controls.Add(this.cellPropertyGridPathTextBox);
            this.cellTabPage.Location = new System.Drawing.Point(4, 22);
            this.cellTabPage.Name = "cellTabPage";
            this.cellTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.cellTabPage.Size = new System.Drawing.Size(658, 154);
            this.cellTabPage.TabIndex = 1;
            this.cellTabPage.Text = "Cell";
            this.cellTabPage.UseVisualStyleBackColor = true;
            // 
            // cellPropertyGrid
            // 
            this.cellPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cellPropertyGrid.Location = new System.Drawing.Point(3, 23);
            this.cellPropertyGrid.Name = "cellPropertyGrid";
            this.cellPropertyGrid.Size = new System.Drawing.Size(652, 128);
            this.cellPropertyGrid.TabIndex = 1;
            // 
            // cellPropertyGridPathTextBox
            // 
            this.cellPropertyGridPathTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.cellPropertyGridPathTextBox.Location = new System.Drawing.Point(3, 3);
            this.cellPropertyGridPathTextBox.Name = "cellPropertyGridPathTextBox";
            this.cellPropertyGridPathTextBox.ReadOnly = true;
            this.cellPropertyGridPathTextBox.Size = new System.Drawing.Size(652, 20);
            this.cellPropertyGridPathTextBox.TabIndex = 2;
            // 
            // loadTabPage
            // 
            this.loadTabPage.Location = new System.Drawing.Point(4, 22);
            this.loadTabPage.Name = "loadTabPage";
            this.loadTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.loadTabPage.Size = new System.Drawing.Size(658, 154);
            this.loadTabPage.TabIndex = 2;
            this.loadTabPage.Text = "Load Messages";
            this.loadTabPage.UseVisualStyleBackColor = true;
            // 
            // propertyTabPage
            // 
            this.propertyTabPage.Location = new System.Drawing.Point(4, 22);
            this.propertyTabPage.Name = "propertyTabPage";
            this.propertyTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.propertyTabPage.Size = new System.Drawing.Size(658, 154);
            this.propertyTabPage.TabIndex = 3;
            this.propertyTabPage.Text = "Properties";
            this.propertyTabPage.UseVisualStyleBackColor = true;
            // 
            // builderTabPage
            // 
            this.builderTabPage.Controls.Add(this.builderPropertyGrid);
            this.builderTabPage.Location = new System.Drawing.Point(4, 22);
            this.builderTabPage.Name = "builderTabPage";
            this.builderTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.builderTabPage.Size = new System.Drawing.Size(658, 154);
            this.builderTabPage.TabIndex = 4;
            this.builderTabPage.Text = "GridBuilder";
            this.builderTabPage.UseVisualStyleBackColor = true;
            // 
            // builderPropertyGrid
            // 
            this.builderPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.builderPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.builderPropertyGrid.Name = "builderPropertyGrid";
            this.builderPropertyGrid.Size = new System.Drawing.Size(652, 148);
            this.builderPropertyGrid.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandToolStripMenuItem,
            this.collapseToolStripMenuItem,
            this.expandSubtreeToolStripMenuItem,
            this.collapseSubtreeToolStripMenuItem,
            this.expandColumnToolStripMenuItem,
            this.collapseColumnToolStripMenuItem,
            this.expandColumnSubtreeToolStripMenuItem,
            this.collapseColumnSubtreeToolStripMenuItem,
            this.expandTableRowsToolStripMenuItem,
            this.collapseTableRowsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(209, 224);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // expandToolStripMenuItem
            // 
            this.expandToolStripMenuItem.Name = "expandToolStripMenuItem";
            this.expandToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.expandToolStripMenuItem.Text = "Expand";
            this.expandToolStripMenuItem.Click += new System.EventHandler(this.expandToolStripMenuItem_Click);
            // 
            // collapseToolStripMenuItem
            // 
            this.collapseToolStripMenuItem.Name = "collapseToolStripMenuItem";
            this.collapseToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.collapseToolStripMenuItem.Text = "Collapse";
            this.collapseToolStripMenuItem.Click += new System.EventHandler(this.collapseToolStripMenuItem_Click);
            // 
            // expandSubtreeToolStripMenuItem
            // 
            this.expandSubtreeToolStripMenuItem.Name = "expandSubtreeToolStripMenuItem";
            this.expandSubtreeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.expandSubtreeToolStripMenuItem.Text = "Expand Subtree";
            this.expandSubtreeToolStripMenuItem.Click += new System.EventHandler(this.expandSubtreeToolStripMenuItem_Click);
            // 
            // collapseSubtreeToolStripMenuItem
            // 
            this.collapseSubtreeToolStripMenuItem.Name = "collapseSubtreeToolStripMenuItem";
            this.collapseSubtreeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.collapseSubtreeToolStripMenuItem.Text = "Collapse Subtree";
            this.collapseSubtreeToolStripMenuItem.Click += new System.EventHandler(this.collapseSubtreeToolStripMenuItem_Click);
            // 
            // expandColumnToolStripMenuItem
            // 
            this.expandColumnToolStripMenuItem.Name = "expandColumnToolStripMenuItem";
            this.expandColumnToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.expandColumnToolStripMenuItem.Text = "Expand Column";
            this.expandColumnToolStripMenuItem.Click += new System.EventHandler(this.expandColumnToolStripMenuItem_Click);
            // 
            // collapseColumnToolStripMenuItem
            // 
            this.collapseColumnToolStripMenuItem.Name = "collapseColumnToolStripMenuItem";
            this.collapseColumnToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.collapseColumnToolStripMenuItem.Text = "Collapse Column";
            this.collapseColumnToolStripMenuItem.Click += new System.EventHandler(this.collapseColumnToolStripMenuItem_Click);
            // 
            // expandColumnSubtreeToolStripMenuItem
            // 
            this.expandColumnSubtreeToolStripMenuItem.Name = "expandColumnSubtreeToolStripMenuItem";
            this.expandColumnSubtreeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.expandColumnSubtreeToolStripMenuItem.Text = "Expand Column Subtree";
            this.expandColumnSubtreeToolStripMenuItem.Click += new System.EventHandler(this.expandColumnSubtreeToolStripMenuItem_Click);
            // 
            // collapseColumnSubtreeToolStripMenuItem
            // 
            this.collapseColumnSubtreeToolStripMenuItem.Name = "collapseColumnSubtreeToolStripMenuItem";
            this.collapseColumnSubtreeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.collapseColumnSubtreeToolStripMenuItem.Text = "Collapse Column Subtree";
            this.collapseColumnSubtreeToolStripMenuItem.Click += new System.EventHandler(this.collapseColumnSubtreeToolStripMenuItem_Click);
            // 
            // expandTableRowsToolStripMenuItem
            // 
            this.expandTableRowsToolStripMenuItem.Name = "expandTableRowsToolStripMenuItem";
            this.expandTableRowsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.expandTableRowsToolStripMenuItem.Text = "Expand Table Rows";
            this.expandTableRowsToolStripMenuItem.Click += new System.EventHandler(this.expandTableRowsToolStripMenuItem_Click);
            // 
            // collapseTableRowsToolStripMenuItem
            // 
            this.collapseTableRowsToolStripMenuItem.Name = "collapseTableRowsToolStripMenuItem";
            this.collapseTableRowsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.collapseTableRowsToolStripMenuItem.Text = "Collapse Table Rows";
            this.collapseTableRowsToolStripMenuItem.Click += new System.EventHandler(this.collapseTableRowsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 499);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.propertyGridPanel.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.viewTabPage.ResumeLayout(false);
            this.cellTabPage.ResumeLayout(false);
            this.cellTabPage.PerformLayout();
            this.builderTabPage.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.PropertyGrid cellPropertyGrid;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel propertyGridPanel;
        private System.Windows.Forms.Panel xmlGridPanel;
        private System.Windows.Forms.TextBox cellPropertyGridPathTextBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage viewTabPage;
        private System.Windows.Forms.TabPage cellTabPage;
        private System.Windows.Forms.PropertyGrid viewPropertyGrid;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invalidateToolStripMenuItem;
        private System.Windows.Forms.TabPage loadTabPage;
        private System.Windows.Forms.TabPage propertyTabPage;
        private System.Windows.Forms.TabPage builderTabPage;
        private System.Windows.Forms.PropertyGrid builderPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem measureCellsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem expandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandSubtreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseSubtreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandColumnSubtreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseColumnSubtreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandTableRowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseTableRowsToolStripMenuItem;

    }
}

