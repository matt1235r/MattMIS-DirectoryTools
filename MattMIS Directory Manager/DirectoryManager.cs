using BrightIdeasSoftware;
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

        private void DirectoryManager_Load(object sender, EventArgs e)
        {
            searchTypeBox.SelectedIndex = 0;
            Config.LoadConfig();
            RefeshDirectoryTree();
            SendMessage(searchTextBox.Handle, EM_SETCUEBANNER, 0, "Filter by...");

            idColumn.ImageGetter = new ImageGetterDelegate(UserHandler.UserImageGetter);
            
        }

        private void RefeshDirectoryTree()
        {
            DirectoryEntry de = new DirectoryEntry("LDAP://" + Config.Settings.ServerAddress + "/" + Config.Settings.ADUserRoot, Config.Settings.Username, Config.Settings.Password);

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
            newnode.ImageIndex = 1;
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
            directoryTreeView.SelectedImageIndex = e.Node.ImageIndex;
            searchTextBox.Clear();
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
                    searchTextBox.Text = "";
                    this.ActiveControl = searchTextBox;
                    return;
                }
                else if (commandValue.StartsWith("SEARCHDIRBY"))
                {
                    backgroundWorker.RunWorkerAsync(argument: new BackgroundArguments() { ARGUMENT = $"{searchTextBox.Text}", HIDEDISABLED=hideDisabledCheckBox.Checked, HIDEUNMATCHED = hideUnmatchedCheckBox.Checked, OPERATION = "SEARCHDIRBY" });
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
            new UserCard(userMenuStrip.Tag.ToString(), null).Show();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UserCard(userMenuStrip.Tag.ToString(), "changepw").Show();
        }

        private void LoadPreRenderListView()
        {
            currentFolderLocation.Text = "Drawing.................";
            fastObjectListView1.SetObjects(UserHandler.GetUsers());
            amountOfItemsLabel.Text = $"{fastObjectListView1.Items.Count} users displayed";

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
                this.fastObjectListView1.ModelFilter = new ModelFilter(delegate (object x) {
                    UserHandler.UserModel userModel = (UserHandler.UserModel)x;
                    
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
                foreach (UserHandler.UserModel um in fastObjectListView1.SelectedObjects)
                {
                    SharedMethods.DisableADAccount(um.directoryEntry);                   
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
                foreach (UserHandler.UserModel um in fastObjectListView1.SelectedObjects)
                {
                    SharedMethods.EnableADAccount(um.directoryEntry);
                }
                MessageBox.Show($"Sucessfully enabled user accounts.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                refreshViewButton.PerformClick();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Unable to enable user account(s): \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (searchTypeBox.SelectedIndex == 0)
            {
                String filterString = searchTextBox.Text.ToLower();
                this.fastObjectListView1.ModelFilter = new ModelFilter(delegate (object x) {
                    UserHandler.UserModel userModel = (UserHandler.UserModel)x;

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
                UserHandler.Clear();
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.OneLevel;
                deSearch.SizeLimit = 3000;
                deSearch.PageSize = 3000;
                deSearch.Filter = $"(&";
                if (arguments.HIDEDISABLED) deSearch.Filter += "(!(UserAccountControl:1.2.840.113556.1.4.803:=2))";
                if (arguments.HIDEUNMATCHED) deSearch.Filter += "(!(comment=* by MattMIS*))";
                deSearch.Filter += $"(objectCategory=person)(objectClass=User))";
                SearchResultCollection userResults = deSearch.FindAll();

                foreach (SearchResult userResult in userResults)
                {
                    UserHandler.AddUser(userResult.GetDirectoryEntry());

                    if (backgroundWorker.CancellationPending) { e.Cancel = true; return; }
                }
                fastObjectListView1.Tag = arguments.OPERATION + "#" + arguments.ARGUMENT;
            }
            else if (arguments.OPERATION == "BYDEPARTMENT")
            {

                DirectoryEntry ADObject = new DirectoryEntry("LDAP://" + Config.Settings.ServerAddress + "/" + Config.Settings.ADUserRoot, Config.Settings.Username, Config.Settings.Password);
                UserHandler.Clear();
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.Subtree;
                deSearch.SizeLimit = 3000;
                deSearch.PageSize = 3000;
                deSearch.Filter = $"(&";
                if (arguments.HIDEDISABLED) deSearch.Filter += "(!(UserAccountControl:1.2.840.113556.1.4.803:=2))";
                if (arguments.HIDEUNMATCHED) deSearch.Filter += "(!(comment=* by MattMIS*))";
                deSearch.Filter += $"(objectCategory=person)(objectClass=User)(department=*{arguments.ARGUMENT}*))";
                SearchResultCollection userResults = deSearch.FindAll();


                foreach (SearchResult userResult in userResults)
                {
                    UserHandler.AddUser(userResult.GetDirectoryEntry());

                    if (backgroundWorker.CancellationPending) { e.Cancel = true; return; }

                }
                fastObjectListView1.Tag = arguments.OPERATION + "#" + arguments.ARGUMENT;

            }
            else if (arguments.OPERATION == "SEARCHDIRBY")
            {
                
                DirectoryEntry ADObject = new DirectoryEntry("LDAP://" + Config.Settings.ServerAddress + "/" + Config.Settings.ADUserRoot, Config.Settings.Username, Config.Settings.Password);
                UserHandler.Clear();
                //ARGUMENT = $"{searchTextBox.Text}#HIDEDISABLED{hideDisabledCheckBox.Checked}#HIDEUNMATCHED{hideUnmatchedCheckBox.Checked}"
                string searchQuery = arguments.ARGUMENT.Split('#')[0];
                DirectorySearcher deSearch = new DirectorySearcher(ADObject);
                deSearch.SearchScope = SearchScope.Subtree;
                deSearch.SizeLimit = 3000;
                deSearch.PageSize = 3000;

                deSearch.Filter = $"(&";
                if (arguments.HIDEDISABLED) deSearch.Filter += "(!(UserAccountControl:1.2.840.113556.1.4.803:=2))";
                if (arguments.HIDEUNMATCHED) deSearch.Filter += "(!(comment=* by MattMIS*))";
                deSearch.Filter += $"(objectCategory=person)(objectClass=User)(|(cn=*{searchQuery}*)(userPrincipalName=*{searchQuery}*)(department=*{searchQuery}*)))"; 

                SearchResultCollection userResults = deSearch.FindAll();

                foreach (SearchResult userResult in userResults)
                {
                    UserHandler.AddUser(userResult.GetDirectoryEntry());

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
                UserHandler.Clear();
                deSearch.PageSize = 3000;
                deSearch.Filter = $"(&";
                if (arguments.HIDEDISABLED) deSearch.Filter += "(!(UserAccountControl:1.2.840.113556.1.4.803:=2))";
                if (arguments.HIDEUNMATCHED) deSearch.Filter += "(!(comment=* by MattMIS*))";
                deSearch.Filter += $"(objectCategory=person)(objectClass=User)(title=*{arguments.ARGUMENT}*))";
                SearchResultCollection userResults = deSearch.FindAll();

                
                foreach (SearchResult userResult in userResults)
                {
                    UserHandler.AddUser(userResult.GetDirectoryEntry());
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
            SendBackgroundCommand(fastObjectListView1.Tag.ToString());
        }

        private void searchTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void directoryTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            
        }

        private void userMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (fastObjectListView1.SelectedIndices.Count == 0) { e.Cancel = true; }
            else if (fastObjectListView1.SelectedIndices.Count == 1)
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
            if (user.ImageKey == "disabled.png") { e.Item.ForeColor = Color.Red; }
            else if (user.Status == "unlinked.png") { e.Item.ForeColor = Color.Blue; }
        }

        private void fastObjectListView1_DoubleClick(object sender, EventArgs e)
        {
            new UserCard(((UserHandler.UserModel)fastObjectListView1.SelectedObjects[0]).directoryEntry).Show();
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



