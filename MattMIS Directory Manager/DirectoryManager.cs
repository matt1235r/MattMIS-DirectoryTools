using BrightIdeasSoftware;
using ComponentOwl.BetterListView;
using MattMIS_Directory_Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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



        List<DirectoryEntry> cacheList = new List<DirectoryEntry>();
        List<BetterListViewItem> preRender = new List<BetterListViewItem>();
        private void DirectoryManager_Load(object sender, EventArgs e)
        {
            searchTypeBox.SelectedIndex = 0;
            RefeshDirectoryTree();
            SendMessage(searchTextBox.Handle, EM_SETCUEBANNER, 0, "Filter by...");

            olvColumn1.ImageGetter = new ImageGetterDelegate(UserHandler.UserImageGetter);
        }

        private void RefeshDirectoryTree()
        {
            DirectoryEntry de = new DirectoryEntry(SharedMethods.connectionString + SharedMethods.ADUserRoot, SharedMethods.username, SharedMethods.password);

            de.AuthenticationType = AuthenticationTypes.Secure;

            AddSubOU(de, directoryTreeView.Nodes.Find("Active Directory", false)[0]);

            directoryTreeView.SelectedNode = directoryTreeView.Nodes.Find("SEARCHPAGE", true)[0];
            directoryTreeView.Nodes.Find("MIS", true)[0].ExpandAll();

        }

        private void AddSubOU(DirectoryEntry thisOU, TreeNode parentnode)
        {
            TreeNode newnode = new TreeNode();
            newnode.Text = Convert.ToString(thisOU.Properties["name"].Value ?? "");
            newnode.Tag = "THISOU#" + thisOU.Path;
            parentnode.Nodes.Add(newnode);
            newnode.ImageIndex = 0;
            DirectorySearcher search = new DirectorySearcher(thisOU);
            search.SearchScope = SearchScope.OneLevel;
            search.Filter = "(&(objectclass=organizationalUnit))";
            SearchResultCollection userResults = search.FindAll();

            foreach (SearchResult sub in userResults)
            {
                AddSubOU(sub.GetDirectoryEntry(), newnode);
            }
        }

        private void SetUserStatus(DirectoryEntry user, BetterListViewItem betterListViewItem)
        {
            string userComment = "";

            userComment = Convert.ToString(user.Properties["comment"].Value ?? "");

            if (userComment.StartsWith("CHECKED### by MattMIS") && IsActive(user))
            {
                userComment = userComment.Split(new[] { "###" }, StringSplitOptions.None)[2];
                userComment = $"Healthy: ({userComment.Split(' ')[0]})";
                betterListViewItem.ImageKey = "enabled.png";
            }
            else if (userComment.StartsWith("DISABLED### by MattMIS") && !IsActive(user))
            {
                userComment = userComment.Split(new[] { "###" }, StringSplitOptions.None)[2];
                userComment = "Deprovisioned";
                betterListViewItem.ForeColor = Color.Red;
                betterListViewItem.ImageKey = "disabled.png";

            }
            else if (IsActive(user))
            {
                userComment = $"Not Matched";
                betterListViewItem.ForeColor = Color.Blue;
                betterListViewItem.ImageKey = "unlinked.png";
            }
            else if (!IsActive(user))
            {
                userComment = $"Manually Disabled";
                betterListViewItem.ForeColor = Color.Red;
                betterListViewItem.ImageKey = "disabled.png";
            }



            betterListViewItem.SubItems.Add(userComment);
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
            directoryTreeView.SelectedImageIndex = e.Node.ImageIndex;
            SendBackgroundCommand(Convert.ToString(e.Node.Tag ?? ""));
        }

        private void SendBackgroundCommand(string commandValue)
        {
            if (commandValue != null)
            {
                if (backgroundWorker.IsBusy) { backgroundWorker.CancelAsync(); backgroundWorker.Abort(); backgroundCommandQueuer.Tag = commandValue; backgroundCommandQueuer.Start(); return; }
                if (commandValue.StartsWith("SEARCHALL"))
                {
                    searchTypeBox.SelectedIndex = 1;
                    mainListView.Items.Clear();
                    searchTextBox.Text = "";
                    this.ActiveControl = searchTextBox;
                    return;
                }
                else if (commandValue.StartsWith("SEARCHDIRBY"))
                {
                    backgroundWorker.RunWorkerAsync(argument: new BackgroundArguments() { ARGUMENT = $"{searchTextBox.Text}#{hideDisabledCheckBox.Checked}#{hideUnmatchedCheckBox.Checked}", OPERATION = "SEARCHDIRBY" });
                    return;
                }
                searchTypeBox.SelectedIndex = 0;
                currentFolderLocation.Text = "Reading from directory....";
                loadingSeperator.Visible = true;
                stopLoadingButton.Visible = true;
                StripProgressBar.Style = ProgressBarStyle.Marquee;
                if (commandValue.StartsWith("BYDEPARTMENT"))
                {
                    backgroundWorker.RunWorkerAsync(argument: new BackgroundArguments() { ARGUMENT = commandValue.Split('#')[1], OPERATION = "BYDEPARTMENT" });
                }
                else if (commandValue.StartsWith("BYTITLE"))
                {
                    backgroundWorker.RunWorkerAsync(argument: new BackgroundArguments() { ARGUMENT = commandValue.Split('#')[1], OPERATION = "BYTITLE" });
                }
                else if (commandValue.StartsWith("THISOU"))
                {
                    backgroundWorker.RunWorkerAsync(argument: new BackgroundArguments() { ARGUMENT = commandValue.Split('#')[1], OPERATION = "THISOU" });
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

        private void mainListView_DoubleClick(object sender, EventArgs e)
        {
            if (mainListView.SelectedItems.Count == 1)
            {
                new UserCard(mainListView.SelectedItems[0].Tag.ToString(), null).Show();
            }
        }

        private void mainListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = mainListView.SelectedItems[0];
                if (focusedItem != null && focusedItem.Bounds.BoundsOuter.Contains(e.Location))
                {
                    userMenuStrip.Tag = focusedItem;
                    viewDetailsToolStripMenuItem.Enabled = true;
                    changePasswordToolStripMenuItem.Enabled = true;
                    userMenuStrip.Show(Cursor.Position);
                }
                else
                {
                    viewDetailsToolStripMenuItem.Enabled = false;
                    changePasswordToolStripMenuItem.Enabled = false;
                    userMenuStrip.Show(Cursor.Position);
                }
            }
        }

        private void viewDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UserCard(((BetterListViewItem)userMenuStrip.Tag).Tag.ToString(), null).Show();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UserCard(((BetterListViewItem)userMenuStrip.Tag).Tag.ToString(), "changepw").Show();
        }

        private void mainListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private BetterListViewItem CreateListViewItem(DirectoryEntry directoryEntry)
        {
            BetterListViewItem betterListViewItem = new BetterListViewItem();
            betterListViewItem.Text = Convert.ToString(directoryEntry.Properties["physicalDeliveryOfficeName"].Value) ?? "";
            betterListViewItem.SubItems.Add(Convert.ToString(directoryEntry.Properties["cn"].Value) ?? "");
            betterListViewItem.SubItems.Add(Convert.ToString(directoryEntry.Properties["department"].Value ?? ""));
            betterListViewItem.SubItems.Add(Convert.ToString(directoryEntry.Properties["userPrincipalName"].Value) ?? "");

            SetUserStatus(directoryEntry, betterListViewItem);
            var sidInBytes = (byte[])directoryEntry.Properties["objectSid"].Value;
            var sid = new SecurityIdentifier(sidInBytes, 0);
            // This gives you what you want

            betterListViewItem.Tag = sid.ToString();
            betterListViewItem.SubItems.Add(Convert.ToString(directoryEntry.Properties["title"].Value) ?? "");

            return betterListViewItem;
        }

        private void LoadPreRenderListView()
        {
            currentFolderLocation.Text = "Drawing.................";
            fastObjectListView1.SetObjects(UserHandler.GetUsers());
            mainListView.BeginUpdate();
            mainListView.Items.Clear();
            foreach (var s in preRender) { mainListView.Items.Add(s); }
            mainListView.EndUpdate();
            amountOfItemsLabel.Text = $"{mainListView.Items.Count} users displayed";

            StripProgressBar.Style = ProgressBarStyle.Continuous;

            currentFolderLocation.Text = "";
            loadingSeperator.Visible = false;
            stopLoadingButton.Visible = false;

        }

        private void LoadFromCache(bool filter = false, string filterString = "")
        {
            currentFolderLocation.Text = "Drawing.................";

            mainListView.BeginUpdate();
            mainListView.Items.Clear();
            for (int i = cacheList.Count - 1; i >= 0; i--)
            {

                if (Convert.ToString(cacheList[i].Properties["cn"].Value ?? "").ToLower().Contains(filterString.ToLower()) || Convert.ToString(cacheList[i].Properties["userPrincipalName"].Value ?? "").ToLower().Contains(filterString.ToLower()) || Convert.ToString(cacheList[i].Properties["department"].Value ?? "").ToLower().Contains(filterString.ToLower()))
                {

                    mainListView.Items.Add(CreateListViewItem(cacheList[i]));

                }

            }
            mainListView.EndUpdate();
            amountOfItemsLabel.Text = $"{mainListView.Items.Count} users displayed";
            currentFolderLocation.Text = "";
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                searchButton.PerformClick();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            LoadFromCache();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            LoadFromCache();

        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {




        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            LoadFromCache();
        }

        delegate void UniversalVoidDelegate();

        /// <summary>
        /// Call form control action from different thread
        /// </summary>
        public static void ControlInvoke(Control control, Action function)
        {
            if (control.IsDisposed || control.Disposing)
                return;

            if (control.InvokeRequired)
            {
                control.Invoke(new UniversalVoidDelegate(() => ControlInvoke(control, function)));
                return;
            }
            function();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            backgroundWorker.Abort();
            backgroundWorker.CancelAsync();

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (searchTextBox.Text == "") { LoadFromCache(); }
            else if (searchTypeBox.SelectedIndex == 0)
            {
                LoadFromCache(true, searchTextBox.Text);
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
                foreach (BetterListViewItem bs in mainListView.SelectedItems)
                {

                    DirectoryEntry ADObject = new DirectoryEntry(SharedMethods.connectionString + SharedMethods.ADUserRoot, SharedMethods.username, SharedMethods.password);
                    ADObject.UsePropertyCache = false;
                    ADObject.AuthenticationType = AuthenticationTypes.ServerBind;

                    DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                    deSearch.SearchScope = SearchScope.Subtree;
                    deSearch.Filter = $"(&(objectCategory=person)(objectClass=User)(objectSid={bs.Tag}))";

                    SearchResultCollection userResults = deSearch.FindAll();
                    if (userResults.Count == 1) //Just one  Member Exists with this ID -- All good. I will check if any updates are needed and if they have been disabled.                
                    {
                        SharedMethods.DisableADAccount(userResults[0].GetDirectoryEntry());
                    }
                }
                MessageBox.Show($"Sucessfully disabled user accounts.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                refreshViewButton.PerformClick();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Unable to disable user account(s): \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void enableAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (BetterListViewItem bs in mainListView.SelectedItems)
                {
                    DirectoryEntry ADObject = new DirectoryEntry(SharedMethods.connectionString + SharedMethods.ADUserRoot, SharedMethods.username, SharedMethods.password);
                    ADObject.UsePropertyCache = false;
                    ADObject.AuthenticationType = AuthenticationTypes.ServerBind;

                    DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                    deSearch.SearchScope = SearchScope.Subtree;
                    deSearch.Filter = $"(&(objectCategory=person)(objectClass=User)(objectSid={bs.Tag}))";

                    SearchResultCollection userResults = deSearch.FindAll();
                    if (userResults.Count == 1) //Just one  Member Exists with this ID -- All good. I will check if any updates are needed and if they have been disabled.                
                    {
                        SharedMethods.EnableADAccount(userResults[0].GetDirectoryEntry());
                    }
                }
                MessageBox.Show($"Sucessfully enabled user accounts.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                refreshViewButton.PerformClick();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Unable to disable user account(s): \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (searchTextBox.Text == "" && searchTypeBox.SelectedIndex == 0) { LoadFromCache(); }
            else if (searchTypeBox.SelectedIndex == 0)
            {

                LoadFromCache(true, searchTextBox.Text);
            }
            if (searchTextBox.Text == "" && searchTypeBox.SelectedIndex == 1) { mainListView.Items.Clear(); }
        }

        private void abortableBackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundArguments arguments = (BackgroundArguments)e.Argument;
            if (arguments.OPERATION == "THISOU")
            {
                string OU = arguments.ARGUMENT;

                DirectoryEntry ADObject = new DirectoryEntry(OU, SharedMethods.username, SharedMethods.password);
                preRender.Clear();
                cacheList.Clear();
                UserHandler.Clear();
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.OneLevel;
                deSearch.SizeLimit = 3000;
                deSearch.PageSize = 3000;
                deSearch.Filter = $"(&(objectCategory=person)(objectClass=User))";
                SearchResultCollection userResults = deSearch.FindAll();

                foreach (SearchResult userResult in userResults)
                {
                    cacheList.Add(userResult.GetDirectoryEntry());
                    preRender.Add(CreateListViewItem(userResult.GetDirectoryEntry()));
                    UserHandler.AddUser(userResult.GetDirectoryEntry());

                    if (backgroundWorker.CancellationPending) { e.Cancel = true; return; }
                }
                mainListView.Tag = arguments.OPERATION + "#" + arguments.ARGUMENT;
            }
            else if (arguments.OPERATION == "BYDEPARTMENT")
            {

                DirectoryEntry ADObject = new DirectoryEntry(SharedMethods.connectionString + SharedMethods.ADUserRoot, SharedMethods.username, SharedMethods.password);
                preRender.Clear();
                cacheList.Clear();
                UserHandler.Clear();
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.Subtree;
                deSearch.SizeLimit = 3000;
                deSearch.PageSize = 3000;
                deSearch.Filter = $"(&(objectCategory=person)(objectClass=User)(department=*{arguments.ARGUMENT}*))";
                SearchResultCollection userResults = deSearch.FindAll();


                foreach (SearchResult userResult in userResults)
                {
                    cacheList.Add(userResult.GetDirectoryEntry());
                    preRender.Add(CreateListViewItem(userResult.GetDirectoryEntry()));
                    UserHandler.AddUser(userResult.GetDirectoryEntry());

                    if (backgroundWorker.CancellationPending) { e.Cancel = true; return; }

                }
                mainListView.Tag = arguments.OPERATION + "#" + arguments.ARGUMENT;

            }
            else if (arguments.OPERATION == "SEARCHDIRBY")
            {

                DirectoryEntry ADObject = new DirectoryEntry(SharedMethods.connectionString + SharedMethods.ADUserRoot, SharedMethods.username, SharedMethods.password);
                preRender.Clear();
                UserHandler.Clear();
                //ARGUMENT = $"{searchTextBox.Text}#HIDEDISABLED{hideDisabledCheckBox.Checked}#HIDEUNMATCHED{hideUnmatchedCheckBox.Checked}"
                string searchQuery = arguments.ARGUMENT.Split('#')[0];
                bool hideDisabled = Boolean.Parse(arguments.ARGUMENT.Split('#')[1]);
                bool hideUnmatched = Boolean.Parse(arguments.ARGUMENT.Split('#')[2]);
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.Subtree;
                deSearch.SizeLimit = 3000;
                deSearch.PageSize = 3000;

                if (hideDisabled) deSearch.Filter = $"(&(objectCategory=person)(objectClass=User)(|(cn=*{searchQuery}*)(userPrincipalName=*{searchQuery}*)(department=*{searchQuery}*))(!(UserAccountControl:1.2.840.113556.1.4.803:=2)))";
                else { deSearch.Filter = $"(&(objectCategory=person)(objectClass=User)(|(cn=*{searchQuery}*)(userPrincipalName=*{searchQuery}*)(department=*{searchQuery}*)))"; }

                SearchResultCollection userResults = deSearch.FindAll();

                foreach (SearchResult userResult in userResults)
                {
                    UserHandler.AddUser(userResult.GetDirectoryEntry());
                    preRender.Add(CreateListViewItem(userResult.GetDirectoryEntry()));
                    if (backgroundWorker.CancellationPending) { e.Cancel = true; return; }
                }
                mainListView.Tag = arguments.OPERATION + "#" + arguments.ARGUMENT;
            }
            else if (arguments.OPERATION == "SEARCHVIEWBY")
            {
                preRender.Clear();
                foreach (DirectoryEntry userResult in cacheList)
                {
                    if (Convert.ToString(userResult.Properties["cn"].Value ?? "").ToLower().Contains(arguments.ARGUMENT.ToLower()) || Convert.ToString(userResult.Properties["userPrincipalName"].Value ?? "").ToLower().Contains(arguments.ARGUMENT.ToLower()) || Convert.ToString(userResult.Properties["department"].Value ?? "").ToLower().Contains(arguments.ARGUMENT.ToLower()))
                    {
                        if (backgroundWorker.CancellationPending) { e.Cancel = true; return; }
                        preRender.Add(CreateListViewItem(userResult));
                    }


                }
            }

            else if (arguments.OPERATION == "BYTITLE")
            {

                DirectoryEntry ADObject = new DirectoryEntry(SharedMethods.connectionString + SharedMethods.ADUserRoot, SharedMethods.username, SharedMethods.password);
                preRender.Clear();
                cacheList.Clear();
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.Subtree;
                deSearch.SizeLimit = 3000;
                deSearch.PageSize = 3000;
                deSearch.Filter = $"(&(objectCategory=person)(objectClass=User)(title=*{arguments.ARGUMENT}*))";
                SearchResultCollection userResults = deSearch.FindAll();

                ;
                foreach (SearchResult userResult in userResults)
                {
                    cacheList.Add(userResult.GetDirectoryEntry());
                    preRender.Add(CreateListViewItem(userResult.GetDirectoryEntry()));

                    UserHandler.AddUser(userResult.GetDirectoryEntry());
                }
                mainListView.Tag = arguments.OPERATION + "#" + arguments.ARGUMENT;
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

        }

        private void directoryTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (backgroundCommandQueuer.Tag != mainListView.Tag && backgroundCommandQueuer.Tag != null)
            {
                mainListView.Items.Clear();

                backgroundCommandQueuer.Stop();
            }
            else
            {
                backgroundCommandQueuer.Stop();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SendBackgroundCommand(mainListView.Tag.ToString());
        }

        private void searchTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (searchTypeBox.SelectedIndex == 0) { hideDisabledCheckBox.Visible = false; hideUnmatchedCheckBox.Visible = false; }
            else if (searchTypeBox.SelectedIndex == 1) { hideDisabledCheckBox.Visible = true; hideUnmatchedCheckBox.Visible = true; }
        }

        private void directoryTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            MessageBox.Show("Test");
        }

        private void userMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (fastObjectListView1.SelectedIndices.Count == 0) { e.Cancel = true; }
            else if (fastObjectListView1.SelectedIndices.Count == 0) 
            { 
                UserHandler.UserModel user = fastObjectListView1.SelectedObject as UserHandler.UserModel;
                userMenuStrip.Tag = user.SID;
                viewDetailsToolStripMenuItem.Enabled = true;
                changePasswordToolStripMenuItem.Enabled = true;
            }
            else
            {
                viewDetailsToolStripMenuItem.Enabled = false;
                changePasswordToolStripMenuItem.Enabled = false;
            }

        }

        private void fastObjectListView1_FormatRow(object sender, FormatRowEventArgs e)
        {
            UserHandler.UserModel user = (UserHandler.UserModel)e.Model;
            if(user.ImageKey == "disabled.png") { e.Item.ForeColor = Color.Red; }
            else if (user.Status == "unlinked.png") { e.Item.ForeColor = Color.Blue; }
        }
    }

    public class BackgroundArguments
    {
        public string OPERATION;
        public string ARGUMENT;

    }
}



