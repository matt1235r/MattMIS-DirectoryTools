using BrightIdeasSoftware;
using MattMIS_Directory_Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MattMIS_Directory_Manager
{

    public partial class DirectoryManager : Form
    {
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        public DirectoryManager()
        {
            InitializeComponent();
        }

        string[] WhiteBackgroundControls = { "TextBox", "TreeView", "FastObjectListView", "ComboBox" };

        public void UpdateColorControls(Control myControl, bool set = true)
        {
            string type = myControl.GetType().Name;
            if (WhiteBackgroundControls.Contains(type)) { myControl.BackColor = SharedMethods.ColorModes.controlColor; }
            else { myControl.BackColor = SharedMethods.ColorModes.backColor; }

            if (type == "Button") { (myControl as System.Windows.Forms.Button).FlatStyle = SharedMethods.ColorModes.themeFlatStyle; }
            if (type == "ComboBox") { (myControl as System.Windows.Forms.ComboBox).FlatStyle = SharedMethods.ColorModes.themeFlatStyle; }
            if (type == "ToolStrip") { }
            if (type == "FastObjectListView")
            {
                (myControl as FastObjectListView).HeaderFormatStyle = SharedMethods.ColorModes.headerFormatStyle;
            }

            myControl.ForeColor = SharedMethods.ColorModes.foreColor;

            foreach (Control subC in myControl.Controls)
            {
                UpdateColorControls(subC);
            }
        }

        private void DirectoryManager_Load(object sender, EventArgs e)
        {
            toolStrip1.Renderer = new ToolStripCustomRenderer();
            toolStrip2.Renderer = new ToolStripCustomRenderer();
            searchTypeBox.SelectedIndex = 0;
            RefeshDirectoryTree();
            SendMessage(searchTextBox.Handle, EM_SETCUEBANNER, 0, "Filter by...");

            idColumn.IsEditable = false;

            idColumn.ImageGetter = new ImageGetterDelegate(Handler.UserImageGetter);
            idColumn.AspectToStringConverter = value => String.Empty;
            fastObjectListView1.HotItemStyle = new HotItemStyle();
            
            //HOT ITEM TRACKING
            // Set styling
            

        }

        private void RefeshDirectoryTree()
        {
            DirectoryEntry de = new DirectoryEntry("LDAP://" + Config.Settings.ServerAddress + "/" + Config.Settings.ADTreeRoot, Config.Settings.Username, Config.Settings.Password);
            de.AuthenticationType = AuthenticationTypes.ServerBind;

            directoryTreeView.Nodes.Clear();
            TreeNode adNode = new TreeNode();
            adNode.Text = "Active Directory";
            adNode.Name = "Active Directory";
            adNode.Tag = "ADROOT#";
            adNode.ImageKey = "OU.png";
            directoryTreeView.Nodes.Add(adNode);
            
            TreeNode adSearch = new TreeNode();
            adSearch.Text = "Search Entire Directory";
            adSearch.Name = "SEARCHPAGE";
            adSearch.ImageKey = "GlobalSearch";
            adSearch.Tag = "SEARCHALL";
            adNode.Nodes.Add(adSearch);

            AddSubOU(de, adNode, true);

            directoryTreeView.SelectedNode = directoryTreeView.Nodes.Find("SEARCHPAGE", true)[0];
            //directoryTreeView.Nodes.Find("MIS", true)[0].ExpandAll();

        }

        private void AddSubOU(DirectoryEntry thisOU, TreeNode parentnode, bool hideNode = false)
        {
            TreeNode newnode = new TreeNode();

            if (!hideNode)
            {
                newnode = new TreeNode();
                newnode.Text = Convert.ToString(thisOU.Properties["name"].Value ?? "");
                newnode.Name = thisOU.Path;
                newnode.Tag = "THISOU#" + thisOU.Path;
                newnode.ImageKey = "OU.png";
                parentnode.Nodes.Add(newnode);

            }
            else newnode = parentnode;

            DirectorySearcher search = new DirectorySearcher(thisOU);
            search.SearchScope = SearchScope.OneLevel;
            search.Filter = "(&(objectclass=organizationalUnit))";
            SearchResultCollection userResults = search.FindAll();

            foreach (SearchResult sub in userResults)
            {
                AddSubOU(sub.GetDirectoryEntry(), newnode);
            }
        }

        private bool IsActive(DirectoryEntry de)
        {
            if (de.NativeGuid == null) return false;

            int flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void directoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            directoryTreeView.SelectedImageKey = e.Node.ImageKey;
            searchTextBox.Clear();
            SendBackgroundCommand(Convert.ToString(e.Node.Tag ?? ""));
            e.Node.Expand();

        }

        private void SendBackgroundCommand(string commandValue)
        {
            if (commandValue != null && commandValue != "")
            {
                if (backgroundWorker.IsBusy) { backgroundWorker.CancelAsync(); backgroundWorker.Abort(); backgroundCommandQueuer.Tag = commandValue; backgroundCommandQueuer.Start(); return; }
                if (commandValue.StartsWith("SEARCHALL"))
                {
                    searchTypeBox.SelectedIndex = 1;
                    searchTextBox.Text = "";
                    this.ActiveControl = searchTextBox;
                    return;
                }
                else if (commandValue.StartsWith("SEARCHDIRBY"))
                {
                    backgroundWorker.RunWorkerAsync(argument: new BackgroundArguments() { ARGUMENT = $"{searchTextBox.Text}", HIDEDISABLED = hideDisabledCheckBox.Checked, HIDEUNMATCHED = hideUnmatchedCheckBox.Checked, OPERATION = "SEARCHDIRBY" });
                    return;
                }
                searchTypeBox.SelectedIndex = 0;
                currentFolderLocation.Text = "Reading from directory....";
                loadingSeperator.Visible = true;
                stopLoadingButton.Visible = true;
                StripProgressBar.Style = ProgressBarStyle.Marquee;
                if (commandValue.StartsWith("BYDEPARTMENT"))
                {
                    backgroundWorker.RunWorkerAsync(argument: new BackgroundArguments() { ARGUMENT = commandValue.Split('#')[1], HIDEDISABLED = hideDisabledCheckBox.Checked, HIDEUNMATCHED = hideUnmatchedCheckBox.Checked, OPERATION = "BYDEPARTMENT" });
                }
                else if (commandValue.StartsWith("BYTITLE"))
                {
                    backgroundWorker.RunWorkerAsync(argument: new BackgroundArguments() { ARGUMENT = commandValue.Split('#')[1], HIDEDISABLED = hideDisabledCheckBox.Checked, HIDEUNMATCHED = hideUnmatchedCheckBox.Checked, OPERATION = "BYTITLE" });
                }
                else if (commandValue.StartsWith("THISOU"))
                {
                    backgroundWorker.RunWorkerAsync(argument: new BackgroundArguments() { ARGUMENT = commandValue.Split('#')[1], HIDEDISABLED = hideDisabledCheckBox.Checked, HIDEUNMATCHED = hideUnmatchedCheckBox.Checked, OPERATION = "THISOU" });
                }
                else
                {
                    currentFolderLocation.Text = "";
                    loadingSeperator.Visible = false;
                    stopLoadingButton.Visible = false;
                    StripProgressBar.Style = ProgressBarStyle.Continuous;
                }

            }


        }

        private void viewDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UserCard((fastObjectListView1.SelectedObjects[0] as Handler.UserModel).directoryEntry).Show();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UserCard((fastObjectListView1.SelectedObjects[0] as Handler.UserModel).directoryEntry, "changepw").Show();
        }

        private void LoadPreRenderListView()
        {
            currentFolderLocation.Text = "Drawing.................";
            fastObjectListView1.SetObjects(Handler.GetObjects());
            amountOfItemsLabel.Text = $"{fastObjectListView1.Items.Count} items displayed";

            StripProgressBar.Style = ProgressBarStyle.Continuous;

            currentFolderLocation.Text = "";
            loadingSeperator.Visible = false;
            stopLoadingButton.Visible = false;

        }

        //private void LoadFromCache(bool filter = false, string filterString = "")
        //{
        //    currentFolderLocation.Text = "Drawing.................";

        //    mainListView.BeginUpdate();
        //    mainListView.Items.Clear();
        //    for (int i = cacheList.Count - 1; i >= 0; i--)
        //    {

        //        if (Convert.ToString(cacheList[i].Properties["cn"].Value ?? "").ToLower().Contains(filterString.ToLower()) || Convert.ToString(cacheList[i].Properties["userPrincipalName"].Value ?? "").ToLower().Contains(filterString.ToLower()) || Convert.ToString(cacheList[i].Properties["department"].Value ?? "").ToLower().Contains(filterString.ToLower()))
        //        {

        //            mainListView.Items.Add(CreateListViewItem(cacheList[i]));

        //        }

        //    }
        //    mainListView.EndUpdate();
        //    amountOfItemsLabel.Text = $"{mainListView.Items.Count} users displayed";
        //    currentFolderLocation.Text = "";
        //}

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                searchButton.PerformClick();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            LoadPreRenderListView();

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            LoadPreRenderListView();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            backgroundWorker.Abort();
            backgroundWorker.CancelAsync();

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (searchTextBox.Text == "") { LoadPreRenderListView(); }
            else if (searchTypeBox.SelectedIndex == 0)
            {
                String filterString = searchTextBox.Text.ToLower();
                this.fastObjectListView1.ModelFilter = new ModelFilter(delegate (object x)
                {
                    Handler.UserModel userModel = (Handler.UserModel)x;

                    if (userModel.FullName.ToLower().Contains(searchTextBox.Text.ToLower()) ||
                    userModel.Username.ToLower().Contains(filterString.ToLower()) ||
                    userModel.Department.ToLower().Contains(filterString))
                    { return true; }
                    else { return false; }



                });
            }
            else if (searchTypeBox.SelectedIndex == 1)
            {
                currentFolderLocation.Text = "Loading....";
                StripProgressBar.Style = ProgressBarStyle.Marquee;
                SendBackgroundCommand($"SEARCHDIRBY#{searchTextBox.Text}#{hideDisabledCheckBox.Checked}#{hideUnmatchedCheckBox.Checked}");


            }
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Handler.UserModel um in fastObjectListView1.SelectedObjects)
                {
                    SharedMethods.DisableADAccount(um.directoryEntry);
                }
                MessageBox.Show($"Sucessfully disabled account(s).", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                refreshViewButton.PerformClick();
                fastObjectListView1.SelectedObjects = null;
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Unable to disable account(s): \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void enableAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Handler.UserModel um in fastObjectListView1.SelectedObjects)
                {
                    SharedMethods.EnableADAccount(um.directoryEntry);
                }
                MessageBox.Show($"Sucessfully enabled account(s).", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                refreshViewButton.PerformClick();
                fastObjectListView1.SelectedObjects = null;
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Unable to enable account(s): \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (searchTypeBox.SelectedIndex == 0)
            {
                String filterString = searchTextBox.Text.ToLower();
                this.fastObjectListView1.ModelFilter = new ModelFilter(delegate (object x)
                {
                    Handler.UserModel userModel = (Handler.UserModel)x;

                    if (userModel.FullName.ToLower().Contains(searchTextBox.Text.ToLower()) ||
                    userModel.Username.ToLower().Contains(filterString.ToLower()) ||
                    userModel.Department.ToLower().Contains(filterString))
                    { return true; }
                    else { return false; }



                });
            }
            if (searchTextBox.Text == "" && searchTypeBox.SelectedIndex == 1) { fastObjectListView1.Items.Clear(); }
        }

        private void abortableBackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundArguments arguments = (BackgroundArguments)e.Argument;
            if (arguments.OPERATION == "THISOU")
            {
                string OU = arguments.ARGUMENT;

                DirectoryEntry ADObject = new DirectoryEntry(OU, Config.Settings.Username, Config.Settings.Password);
                Handler.Clear();
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.OneLevel;
                deSearch.SizeLimit = 3000;
                deSearch.PageSize = 3000;
                deSearch.Filter = $"(&";
                if (arguments.HIDEDISABLED) deSearch.Filter += "(!(UserAccountControl:1.2.840.113556.1.4.803:=2))";
                if (arguments.HIDEUNMATCHED) deSearch.Filter += "((comment=* by MattMIS*))";
                if (showMoreObjects.Checked) { deSearch.Filter += $"(|(objectCategory=person)(objectCategory=printQueue)(objectCategory=volume)(objectCategory=group)(objectClass=User)(objectClass=OrganizationalUnit)(objectClass=Computer)))"; }
                else { deSearch.Filter += "(objectCategory=person)(objectClass=User))"; }
                SearchResultCollection userResults = deSearch.FindAll();

                foreach (SearchResult userResult in userResults)
                {
                    Handler.AddItem(userResult.GetDirectoryEntry());

                    if (backgroundWorker.CancellationPending) { e.Cancel = true; return; }
                }
                fastObjectListView1.Tag = arguments.OPERATION + "#" + arguments.ARGUMENT;
            }
            else if (arguments.OPERATION == "BYDEPARTMENT")
            {

                DirectoryEntry ADObject = new DirectoryEntry("LDAP://" + Config.Settings.ServerAddress + "/" + Config.Settings.ADUserRoot, Config.Settings.Username, Config.Settings.Password);
                Handler.Clear();
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.Subtree;
                deSearch.SizeLimit = 3000;
                deSearch.PageSize = 3000;
                deSearch.Filter = $"(&";
                if (arguments.HIDEDISABLED) deSearch.Filter += "(!(UserAccountControl:1.2.840.113556.1.4.803:=2))";
                if (arguments.HIDEUNMATCHED) deSearch.Filter += "((comment=* by MattMIS*))";
                deSearch.Filter += $"(objectCategory=person)(objectClass=User)(department=*{arguments.ARGUMENT}*))";
                SearchResultCollection userResults = deSearch.FindAll();


                foreach (SearchResult userResult in userResults)
                {
                    Handler.AddItem(userResult.GetDirectoryEntry());

                    if (backgroundWorker.CancellationPending) { e.Cancel = true; return; }

                }
                fastObjectListView1.Tag = arguments.OPERATION + "#" + arguments.ARGUMENT;

            }
            else if (arguments.OPERATION == "SEARCHDIRBY")
            {

                DirectoryEntry ADObject = new DirectoryEntry("LDAP://" + Config.Settings.ServerAddress + "/" + Config.Settings.ADUserRoot, Config.Settings.Username, Config.Settings.Password);
                Handler.Clear();
                //ARGUMENT = $"{searchTextBox.Text}#HIDEDISABLED{hideDisabledCheckBox.Checked}#HIDEUNMATCHED{hideUnmatchedCheckBox.Checked}"
                string searchQuery = arguments.ARGUMENT.Split('#')[0];
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.Subtree;
                deSearch.SizeLimit = 3000;
                deSearch.PageSize = 3000;

                deSearch.Filter = $"(&";
                if (arguments.HIDEDISABLED) deSearch.Filter += "(!(UserAccountControl:1.2.840.113556.1.4.803:=2))";
                if (arguments.HIDEUNMATCHED) deSearch.Filter += "((comment=* by MattMIS*))";
                if (showMoreObjects.Checked) { deSearch.Filter += $"(|(objectCategory=person)(objectCategory=printQueue)(objectCategory=volume)(objectCategory=group)(objectClass=User)(objectClass=OrganizationalUnit)(objectClass=Computer))(|(cn=*{searchQuery}*)(userPrincipalName=*{searchQuery}*)(department=*{searchQuery}*)(distinguishedName=*{searchQuery}*)))"; }
                else { deSearch.Filter += "(objectCategory=person)(objectClass=User)(|(cn=*{searchQuery}*)(userPrincipalName=*{searchQuery}*)(department=*{searchQuery}*)))"; }

                SearchResultCollection userResults = deSearch.FindAll();

                foreach (SearchResult userResult in userResults)
                {
                    Handler.AddItem(userResult.GetDirectoryEntry());

                    if (backgroundWorker.CancellationPending) { e.Cancel = true; return; }
                }

                fastObjectListView1.Tag = arguments.OPERATION + "#" + arguments.ARGUMENT;
            }
            else if (arguments.OPERATION == "SEARCHVIEWBY")
            {

            }

            else if (arguments.OPERATION == "BYTITLE")
            {

                DirectoryEntry ADObject = new DirectoryEntry("LDAP://" + Config.Settings.ServerAddress + "/" + Config.Settings.ADUserRoot, Config.Settings.Username, Config.Settings.Password);
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.Subtree;
                deSearch.SizeLimit = 3000;
                Handler.Clear();
                deSearch.PageSize = 3000;
                deSearch.Filter = $"(&";
                if (arguments.HIDEDISABLED) deSearch.Filter += "(!(UserAccountControl:1.2.840.113556.1.4.803:=2))";
                if (arguments.HIDEUNMATCHED) deSearch.Filter += "((comment=* by MattMIS*))";
                deSearch.Filter += $"(objectCategory=person)(objectClass=User)(title=*{arguments.ARGUMENT}*))";
                SearchResultCollection userResults = deSearch.FindAll();


                foreach (SearchResult userResult in userResults)
                {
                    Handler.AddItem(userResult.GetDirectoryEntry());
                    if (backgroundWorker.CancellationPending) { e.Cancel = true; return; }
                }
                fastObjectListView1.Tag = arguments.OPERATION + "#" + arguments.ARGUMENT;
            }

        }

        private void abortableBackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                SendBackgroundCommand(backgroundCommandQueuer.Tag.ToString());
            }
            else
            {
                currentFolderLocation.Text = "Finished Thread...";
                LoadPreRenderListView();

                StripProgressBar.Style = ProgressBarStyle.Continuous;
            }

        }

        private void directoryTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                if (e.Node.Tag.ToString() == "SEARCHALL")
                {
                    searchTypeBox.SelectedIndex = 1;
                    searchTextBox.Text = "";
                    this.ActiveControl = searchTextBox;
                }
            }
        }

        private void hideDisabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            refreshViewButton.PerformClick();
            
        }

        private void directoryTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (backgroundCommandQueuer.Tag != fastObjectListView1.Tag && backgroundCommandQueuer.Tag != null)
            {
                fastObjectListView1.Items.Clear();

                backgroundCommandQueuer.Stop();
            }
            else
            {
                backgroundCommandQueuer.Stop();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SendBackgroundCommand(Convert.ToString(fastObjectListView1.Tag ?? ""));
        }

        private void searchTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void directoryTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void userMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (fastObjectListView1.SelectedIndices.Count == 0) { e.Cancel = true; return; }
            else if (fastObjectListView1.SelectedIndices.Count == 1)
            {
                if ((fastObjectListView1.SelectedObjects[0] as Handler.UserModel).ObjectType == "user")
                {
                    viewDetailsToolStripMenuItem.Visible = true;
                    changePasswordToolStripMenuItem.Visible = true;
                    openHomeFolderToolStripMenuItem.Visible = true;
                    userStripSeparator.Visible = true;
                    return;
                }
                else if ((fastObjectListView1.SelectedObjects[0] as Handler.UserModel).ObjectType == "computer") { }
                else
                {
                    e.Cancel = true;
                }

            }
            viewDetailsToolStripMenuItem.Visible = false;
            changePasswordToolStripMenuItem.Visible = false;
            openHomeFolderToolStripMenuItem.Visible = false;
            userStripSeparator.Visible = false;

        }

        private void fastObjectListView1_FormatRow(object sender, FormatRowEventArgs e)
        {
            Handler.UserModel user = (Handler.UserModel)e.Model;
            if (user.ImageKey == "user_disabled.ico" || user.ImageKey == "computer_disabled.ico") { e.Item.ForeColor = Color.Red; }
            else if (user.ImageKey == "user_normal.ico") { e.Item.ForeColor = Color.Blue; }

        }

        private void fastObjectListView1_DoubleClick(object sender, EventArgs e)
        {
            Handler.UserModel obj = fastObjectListView1.SelectedObjects[0] as Handler.UserModel;
            if (obj.ObjectType == "user") new UserCard(obj.directoryEntry).Show();
            else if (obj.ObjectType == "organizationalUnit")
            {
                directoryTreeView.SelectedNode = directoryTreeView.Nodes.Find(obj.Extras, true)[0];
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refreshViewButton.PerformClick();
        }

        private void hideUnmatchedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            refreshViewButton.PerformClick();
            fastObjectListView1.HeaderFormatStyle = darkHeaderFormatStyle;
        }

        private void DirectoryManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void openHomeFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Handler.UserModel userModel in fastObjectListView1.SelectedObjects)
            {
                Process.Start("explorer.exe", $"/open, {userModel.directoryEntry.Properties["homeDirectory"].Value}");
            }

        }

        private void fastObjectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void navigateUpButton_Click(object sender, EventArgs e)
        {
            directoryTreeView.SelectedNode = directoryTreeView.SelectedNode.Parent;
        }

        private void DirectoryManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                refreshViewButton.PerformClick();
            }
        }

        private void nvaigateUpButton2_Click(object sender, EventArgs e)
        {
            navigateUpButton.PerformClick();
        }

        private void refreshButton2_Click(object sender, EventArgs e)
        {
            refreshViewButton.PerformClick();
        }

        private void deleteObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void generalMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            List<String> objectTypes = new List<string>();
            for (int i = 0; i < fastObjectListView1.SelectedObjects.Count; i++) objectTypes.Add(((Handler.UserModel)fastObjectListView1.SelectedObjects[i]).ObjectType);

            //HIDE ALL FROM VIEW FIRST
            userOptionsToolStripMenuItem.Visible = false;
            computerOptionsToolStripMenuItem.Visible = false;


            //CHECK WHAT ELEMENTS ARE IN
            if (objectTypes.Where(x => x == "user").Count() == fastObjectListView1.SelectedObjects.Count) userOptionsToolStripMenuItem.Visible = true;

            //CHECK WHAT ELEMENTS ARE IN
            if (objectTypes.Where(x => x == "computer").Count() == fastObjectListView1.SelectedObjects.Count) computerOptionsToolStripMenuItem.Visible = true;

        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes != MessageBox.Show($"Are you sure you wish to delete {fastObjectListView1.SelectedObjects.Count} object(s)?\n\nThese items will not be able to be retrieved.", "Delete Object(s)", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)) return;
                foreach (Handler.UserModel um in fastObjectListView1.SelectedObjects)
                {
                    um.directoryEntry.DeleteTree();
                }
                refreshViewButton.PerformClick();
                fastObjectListView1.SelectedObjects = null;
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Unable to delete objects(s): \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void commandPromptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Handler.UserModel us = (Handler.UserModel)fastObjectListView1.SelectedObjects[0];
                Process.Start("PSEXEC.EXE", $"\\\\{us.FullName} -u {Config.Settings.Username} -p {Config.Settings.Password} -i cmd.exe -e");
            }
            catch (Exception)
            {

                MessageBox.Show("Error starting PSEXEC. Please make sure you have downloaded PSEXEC and it is in the same folder as this program.", "ERROR");
            }
        }

        private void darkModeButton_Click_1(object sender, EventArgs e)
        {
            SharedMethods.ColorModes.darkMode = darkModeButton.Checked;
            if (SharedMethods.ColorModes.darkMode)
            {
                SharedMethods.ColorModes.backColor = Color.FromArgb(64, 64, 64);
                SharedMethods.ColorModes.foreColor = Color.White;
                SharedMethods.ColorModes.controlColor = Color.FromArgb(64, 64, 64);
                
                SharedMethods.ColorModes.themeFlatStyle = FlatStyle.Flat;
                SharedMethods.ColorModes.itemHoverColor = Color.Gray;
                SharedMethods.ColorModes.itemSelectedColor = Color.DarkGray;
            }
            else
            {
                SharedMethods.ColorModes.backColor = SystemColors.Control;
                SharedMethods.ColorModes.foreColor = Color.Black;
                SharedMethods.ColorModes.controlColor = SystemColors.Window;
                fastObjectListView1.HeaderFormatStyle = null;
                SharedMethods.ColorModes.themeFlatStyle = FlatStyle.Standard;
                SharedMethods.ColorModes.itemHoverColor = Color.FromArgb(224, 224, 224);
                SharedMethods.ColorModes.itemHoverColor = Color.FromKnownColor(KnownColor.GradientInactiveCaption);
                SharedMethods.ColorModes.itemSelectedColor = Color.FromKnownColor(KnownColor.GradientActiveCaption);
            }
            if (darkModeButton.Checked) darkModeButton2.Checked = true;
            else darkModeButton2.Checked = false;
            
            fastObjectListView1.RebuildColumns();
            fastObjectListView1.Refresh();
            fastObjectListView1.HotItemStyle.BackColor = SharedMethods.ColorModes.itemHoverColor;
            fastObjectListView1.SelectedBackColor = SharedMethods.ColorModes.itemSelectedColor;
            
            UpdateColorControls(this);
        }

        private void darkModeButton2_Click(object sender, EventArgs e)
        {
            darkModeButton.PerformClick();
        }

        private void connectionInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Domain: {Config.Settings.ADTreeRoot}\nServer: {Config.Settings.ServerAddress}\nConnected As: {Config.Settings.Username ?? Environment.UserDomainName + "/" + Environment.UserName}", "Connection Information");
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ConnectionWindow().Show();
            this.Close();
        }

        private void refreshToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            RefeshDirectoryTree();
        }
    }

    public class BackgroundArguments
    {
        public string OPERATION;
        public string ARGUMENT;
        public bool HIDEDISABLED;
        public bool HIDEUNMATCHED;

    }
}



