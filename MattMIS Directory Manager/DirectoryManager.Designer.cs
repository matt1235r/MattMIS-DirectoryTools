namespace MattMIS_Directory_Manager
{
    partial class DirectoryManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectoryManager));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Search Entire Directory");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("7 Fisher");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("7 More");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("11 Fisher");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Registration Groups", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Year 7");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Year 8");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Year 9");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Year 10");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Year 11");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Year 12");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Year 13 ");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Year Groups", new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12});
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("All Pupils", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode13});
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("All Staff");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("The Campion School (SIMS)", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode14,
            treeNode15});
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Active Directory");
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.directoryTreeView = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.userMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.changePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.disableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.hideUnmatchedCheckBox = new System.Windows.Forms.CheckBox();
            this.fastObjectListView1 = new BrightIdeasSoftware.FastObjectListView();
            this.idColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.fullNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.departmentColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.usernameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.statusColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.hideDisabledCheckBox = new System.Windows.Forms.CheckBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.amountOfItemsLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.refreshViewButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.StripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.currentFolderLocation = new System.Windows.Forms.ToolStripLabel();
            this.stopLoadingButton = new System.Windows.Forms.ToolStripButton();
            this.loadingSeperator = new System.Windows.Forms.ToolStripSeparator();
            this.label2 = new System.Windows.Forms.Label();
            this.searchTypeBox = new System.Windows.Forms.ComboBox();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.backgroundCommandQueuer = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker = new MattMIS_Directory_Manager.AbortableBackgroundWorker();
            this.toolStrip1.SuspendLayout();
            this.userMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fastObjectListView1)).BeginInit();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "201556.png");
            this.imageList1.Images.SetKeyName(1, "unlinked.png");
            this.imageList1.Images.SetKeyName(2, "sims.jpg");
            this.imageList1.Images.SetKeyName(3, "sims-logo.png");
            this.imageList1.Images.SetKeyName(4, "149071.png");
            this.imageList1.Images.SetKeyName(5, "disabled.png");
            this.imageList1.Images.SetKeyName(6, "enabled.png");
            this.imageList1.Images.SetKeyName(7, "161-1616455_search-search-icon-grey.png");
            this.imageList1.Images.SetKeyName(8, "azure-active-directory-aad-icon-488x512-3d71nrtk.png");
            this.imageList1.Images.SetKeyName(9, "224641.png");
            // 
            // directoryTreeView
            // 
            this.directoryTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directoryTreeView.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directoryTreeView.HideSelection = false;
            this.directoryTreeView.ImageIndex = 0;
            this.directoryTreeView.ImageList = this.imageList1;
            this.directoryTreeView.Location = new System.Drawing.Point(0, 0);
            this.directoryTreeView.Name = "directoryTreeView";
            treeNode1.Checked = true;
            treeNode1.ImageIndex = 7;
            treeNode1.Name = "SEARCHPAGE";
            treeNode1.Tag = "SEARCHALL";
            treeNode1.Text = "Search Entire Directory";
            treeNode2.ImageIndex = 1;
            treeNode2.Name = "Node3";
            treeNode2.Tag = "BYDEPARTMENT#7 Fisher";
            treeNode2.Text = "7 Fisher";
            treeNode3.ImageIndex = 1;
            treeNode3.Name = "Node5";
            treeNode3.Text = "7 More";
            treeNode4.ImageIndex = 1;
            treeNode4.Name = "Node1";
            treeNode4.Tag = "BYDEPARTMENT#11 Fisher";
            treeNode4.Text = "11 Fisher";
            treeNode5.ImageIndex = 1;
            treeNode5.Name = "Node2";
            treeNode5.Text = "Registration Groups";
            treeNode6.ImageIndex = 1;
            treeNode6.Name = "Year 7";
            treeNode6.Tag = "BYDEPARTMENT#7 ";
            treeNode6.Text = "Year 7";
            treeNode7.ImageIndex = 1;
            treeNode7.Name = "Node1";
            treeNode7.Tag = "BYDEPARTMENT#8 ";
            treeNode7.Text = "Year 8";
            treeNode8.ImageIndex = 1;
            treeNode8.Name = "Node1";
            treeNode8.Tag = "BYDEPARTMENT#9 ";
            treeNode8.Text = "Year 9";
            treeNode9.ImageIndex = 1;
            treeNode9.Name = "Node4";
            treeNode9.Tag = "BYDEPARTMENT#10 ";
            treeNode9.Text = "Year 10";
            treeNode10.ImageIndex = 1;
            treeNode10.Name = "Node5";
            treeNode10.Tag = "BYDEPARTMENT#11 ";
            treeNode10.Text = "Year 11";
            treeNode11.ImageIndex = 1;
            treeNode11.Name = "Node6";
            treeNode11.Tag = "BYDEPARTMENT#12";
            treeNode11.Text = "Year 12";
            treeNode12.ImageIndex = 1;
            treeNode12.Name = "Node7";
            treeNode12.Tag = "BYDEPARTMENT#13";
            treeNode12.Text = "Year 13 ";
            treeNode13.ImageIndex = 1;
            treeNode13.Name = "Node0";
            treeNode13.Text = "Year Groups";
            treeNode14.ImageIndex = 1;
            treeNode14.Name = "Node1";
            treeNode14.Tag = "BYTITLE#Student";
            treeNode14.Text = "All Pupils";
            treeNode15.ImageIndex = 1;
            treeNode15.Name = "Node0";
            treeNode15.Tag = "BYTITLE#Staff";
            treeNode15.Text = "All Staff";
            treeNode16.ImageIndex = 2;
            treeNode16.Name = "MIS";
            treeNode16.Text = "The Campion School (SIMS)";
            treeNode17.ImageIndex = 8;
            treeNode17.Name = "Active Directory";
            treeNode17.Tag = "ADROOT#";
            treeNode17.Text = "Active Directory";
            this.directoryTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode16,
            treeNode17});
            this.directoryTreeView.SelectedImageIndex = 0;
            this.directoryTreeView.Size = new System.Drawing.Size(300, 526);
            this.directoryTreeView.TabIndex = 1;
            this.directoryTreeView.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.directoryTreeView_BeforeCollapse);
            this.directoryTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.directoryTreeView_BeforeSelect);
            this.directoryTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.directoryTreeView_AfterSelect);
            this.directoryTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.directoryTreeView_NodeMouseClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1,
            this.toolStripSeparator3,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1253, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(32, 22);
            this.toolStripSplitButton1.Text = "Open";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Refresh View";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // userMenuStrip
            // 
            this.userMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewDetailsToolStripMenuItem,
            this.toolStripSeparator1,
            this.changePasswordToolStripMenuItem,
            this.toolStripSeparator2,
            this.disableToolStripMenuItem,
            this.enableAccountToolStripMenuItem});
            this.userMenuStrip.Name = "userMenuStrip";
            this.userMenuStrip.Size = new System.Drawing.Size(169, 104);
            this.userMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.userMenuStrip_Opening);
            // 
            // viewDetailsToolStripMenuItem
            // 
            this.viewDetailsToolStripMenuItem.Name = "viewDetailsToolStripMenuItem";
            this.viewDetailsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.viewDetailsToolStripMenuItem.Text = "View Details";
            this.viewDetailsToolStripMenuItem.Click += new System.EventHandler(this.viewDetailsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // changePasswordToolStripMenuItem
            // 
            this.changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
            this.changePasswordToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.changePasswordToolStripMenuItem.Text = "Change Password";
            this.changePasswordToolStripMenuItem.Click += new System.EventHandler(this.changePasswordToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(165, 6);
            // 
            // disableToolStripMenuItem
            // 
            this.disableToolStripMenuItem.Name = "disableToolStripMenuItem";
            this.disableToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.disableToolStripMenuItem.Text = "Disable Account";
            this.disableToolStripMenuItem.Click += new System.EventHandler(this.disableToolStripMenuItem_Click);
            // 
            // enableAccountToolStripMenuItem
            // 
            this.enableAccountToolStripMenuItem.Name = "enableAccountToolStripMenuItem";
            this.enableAccountToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.enableAccountToolStripMenuItem.Text = "Enable Account";
            this.enableAccountToolStripMenuItem.Click += new System.EventHandler(this.enableAccountToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.directoryTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.hideUnmatchedCheckBox);
            this.splitContainer1.Panel2.Controls.Add(this.fastObjectListView1);
            this.splitContainer1.Panel2.Controls.Add(this.hideDisabledCheckBox);
            this.splitContainer1.Panel2.Controls.Add(this.searchButton);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip2);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.searchTypeBox);
            this.splitContainer1.Panel2.Controls.Add(this.searchTextBox);
            this.splitContainer1.Size = new System.Drawing.Size(1253, 526);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 3;
            // 
            // hideUnmatchedCheckBox
            // 
            this.hideUnmatchedCheckBox.AutoSize = true;
            this.hideUnmatchedCheckBox.Location = new System.Drawing.Point(153, 44);
            this.hideUnmatchedCheckBox.Name = "hideUnmatchedCheckBox";
            this.hideUnmatchedCheckBox.Size = new System.Drawing.Size(154, 17);
            this.hideUnmatchedCheckBox.TabIndex = 11;
            this.hideUnmatchedCheckBox.Text = "Hide unmatched accounts.";
            this.hideUnmatchedCheckBox.UseVisualStyleBackColor = true;
            // 
            // fastObjectListView1
            // 
            this.fastObjectListView1.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.fastObjectListView1.AllColumns.Add(this.idColumn);
            this.fastObjectListView1.AllColumns.Add(this.fullNameColumn);
            this.fastObjectListView1.AllColumns.Add(this.departmentColumn);
            this.fastObjectListView1.AllColumns.Add(this.usernameColumn);
            this.fastObjectListView1.AllColumns.Add(this.statusColumn);
            this.fastObjectListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fastObjectListView1.CellEditUseWholeCell = false;
            this.fastObjectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.idColumn,
            this.fullNameColumn,
            this.departmentColumn,
            this.usernameColumn,
            this.statusColumn});
            this.fastObjectListView1.ContextMenuStrip = this.userMenuStrip;
            this.fastObjectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.fastObjectListView1.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fastObjectListView1.FullRowSelect = true;
            this.fastObjectListView1.HideSelection = false;
            this.fastObjectListView1.Location = new System.Drawing.Point(7, 67);
            this.fastObjectListView1.Name = "fastObjectListView1";
            this.fastObjectListView1.RowHeight = 30;
            this.fastObjectListView1.SelectedBackColor = System.Drawing.Color.LightBlue;
            this.fastObjectListView1.SelectedForeColor = System.Drawing.Color.Black;
            this.fastObjectListView1.ShowGroups = false;
            this.fastObjectListView1.Size = new System.Drawing.Size(930, 431);
            this.fastObjectListView1.SmallImageList = this.imageList1;
            this.fastObjectListView1.TabIndex = 2;
            this.fastObjectListView1.UseCompatibleStateImageBehavior = false;
            this.fastObjectListView1.UseFiltering = true;
            this.fastObjectListView1.UseHotItem = true;
            this.fastObjectListView1.View = System.Windows.Forms.View.Details;
            this.fastObjectListView1.VirtualMode = true;
            this.fastObjectListView1.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.fastObjectListView1_FormatRow);
            this.fastObjectListView1.DoubleClick += new System.EventHandler(this.fastObjectListView1_DoubleClick);
            // 
            // idColumn
            // 
            this.idColumn.AspectName = "ID";
            this.idColumn.Text = "ID";
            this.idColumn.Width = 74;
            // 
            // fullNameColumn
            // 
            this.fullNameColumn.AspectName = "FullName";
            this.fullNameColumn.Text = "Full Name";
            this.fullNameColumn.Width = 231;
            // 
            // departmentColumn
            // 
            this.departmentColumn.AspectName = "Department";
            this.departmentColumn.Text = "Department";
            this.departmentColumn.Width = 221;
            // 
            // usernameColumn
            // 
            this.usernameColumn.AspectName = "Username";
            this.usernameColumn.Text = "Username";
            this.usernameColumn.Width = 315;
            // 
            // statusColumn
            // 
            this.statusColumn.AspectName = "Status";
            this.statusColumn.Text = "Status";
            this.statusColumn.Width = 249;
            // 
            // hideDisabledCheckBox
            // 
            this.hideDisabledCheckBox.AutoSize = true;
            this.hideDisabledCheckBox.Location = new System.Drawing.Point(7, 44);
            this.hideDisabledCheckBox.Name = "hideDisabledCheckBox";
            this.hideDisabledCheckBox.Size = new System.Drawing.Size(140, 17);
            this.hideDisabledCheckBox.TabIndex = 9;
            this.hideDisabledCheckBox.Text = "Hide disabled accounts.";
            this.hideDisabledCheckBox.UseVisualStyleBackColor = true;
            this.hideDisabledCheckBox.CheckedChanged += new System.EventHandler(this.hideDisabledCheckBox_CheckedChanged);
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.BackColor = System.Drawing.SystemColors.Control;
            this.searchButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("searchButton.BackgroundImage")));
            this.searchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.searchButton.ForeColor = System.Drawing.SystemColors.Control;
            this.searchButton.Location = new System.Drawing.Point(907, 5);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(33, 33);
            this.searchButton.TabIndex = 8;
            this.searchButton.UseVisualStyleBackColor = false;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(240, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Filter:";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.amountOfItemsLabel,
            this.toolStripSeparator4,
            this.toolStripButton2,
            this.refreshViewButton,
            this.toolStripSeparator6,
            this.StripProgressBar,
            this.toolStripSeparator7,
            this.currentFolderLocation,
            this.stopLoadingButton,
            this.loadingSeperator});
            this.toolStrip2.Location = new System.Drawing.Point(0, 501);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(949, 25);
            this.toolStrip2.TabIndex = 6;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // amountOfItemsLabel
            // 
            this.amountOfItemsLabel.Name = "amountOfItemsLabel";
            this.amountOfItemsLabel.Size = new System.Drawing.Size(153, 22);
            this.amountOfItemsLabel.Text = "MattMIS Directory Manager";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Soft Refresh";
            this.toolStripButton2.Visible = false;
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // refreshViewButton
            // 
            this.refreshViewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshViewButton.Image = ((System.Drawing.Image)(resources.GetObject("refreshViewButton.Image")));
            this.refreshViewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshViewButton.Name = "refreshViewButton";
            this.refreshViewButton.Size = new System.Drawing.Size(23, 22);
            this.refreshViewButton.Text = "Refresh View";
            this.refreshViewButton.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // StripProgressBar
            // 
            this.StripProgressBar.Name = "StripProgressBar";
            this.StripProgressBar.Size = new System.Drawing.Size(100, 22);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // currentFolderLocation
            // 
            this.currentFolderLocation.Name = "currentFolderLocation";
            this.currentFolderLocation.Size = new System.Drawing.Size(68, 22);
            this.currentFolderLocation.Text = "Alpha Build";
            // 
            // stopLoadingButton
            // 
            this.stopLoadingButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopLoadingButton.Image = ((System.Drawing.Image)(resources.GetObject("stopLoadingButton.Image")));
            this.stopLoadingButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopLoadingButton.Name = "stopLoadingButton";
            this.stopLoadingButton.Size = new System.Drawing.Size(23, 22);
            this.stopLoadingButton.Text = "Stop Loading";
            this.stopLoadingButton.Visible = false;
            this.stopLoadingButton.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // loadingSeperator
            // 
            this.loadingSeperator.Name = "loadingSeperator";
            this.loadingSeperator.Size = new System.Drawing.Size(6, 25);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Search:";
            // 
            // searchTypeBox
            // 
            this.searchTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.searchTypeBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchTypeBox.FormattingEnabled = true;
            this.searchTypeBox.Items.AddRange(new object[] {
            "Current View",
            "Whole Directory"});
            this.searchTypeBox.Location = new System.Drawing.Point(68, 10);
            this.searchTypeBox.Name = "searchTypeBox";
            this.searchTypeBox.Size = new System.Drawing.Size(166, 28);
            this.searchTypeBox.TabIndex = 4;
            this.searchTypeBox.SelectedIndexChanged += new System.EventHandler(this.searchTypeBox_SelectedIndexChanged);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchTextBox.Location = new System.Drawing.Point(294, 10);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(608, 26);
            this.searchTextBox.TabIndex = 1;
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            this.searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchTextBox_KeyDown);
            // 
            // backgroundCommandQueuer
            // 
            this.backgroundCommandQueuer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.abortableBackgroundWorker1_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.abortableBackgroundWorker1_RunWorkerCompleted);
            // 
            // DirectoryManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1253, 551);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DirectoryManager";
            this.Text = "MattMIS Directory Manager";
            this.Load += new System.EventHandler(this.DirectoryManager_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.userMenuStrip.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fastObjectListView1)).EndInit();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TreeView directoryTreeView;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip userMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem viewDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem changePasswordToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem disableToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox searchTypeBox;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel amountOfItemsLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripLabel currentFolderLocation;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripProgressBar StripProgressBar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton stopLoadingButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.CheckBox hideUnmatchedCheckBox;
        private System.Windows.Forms.CheckBox hideDisabledCheckBox;
        private System.Windows.Forms.ToolStripSeparator loadingSeperator;
        private System.Windows.Forms.ToolStripMenuItem enableAccountToolStripMenuItem;
        private AbortableBackgroundWorker backgroundWorker;
        private System.Windows.Forms.Timer backgroundCommandQueuer;
        private System.Windows.Forms.ToolStripButton refreshViewButton;
        private BrightIdeasSoftware.FastObjectListView fastObjectListView1;
        private BrightIdeasSoftware.OLVColumn idColumn;
        private BrightIdeasSoftware.OLVColumn fullNameColumn;
        private BrightIdeasSoftware.OLVColumn departmentColumn;
        private BrightIdeasSoftware.OLVColumn usernameColumn;
        private BrightIdeasSoftware.OLVColumn statusColumn;
    }
}