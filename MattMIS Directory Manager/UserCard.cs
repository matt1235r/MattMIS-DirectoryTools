
using ActiveDs;
using ComponentOwl.BetterListView;
using ComponentOwl.BetterListView.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MattMIS_Directory_Manager
{
    public partial class UserCard : Form
    {
        public UserCard()
        {
            InitializeComponent();
        }

        public UserCard(DirectoryEntry directoryEntry)
        {
            InitializeComponent();
            PopulateOverview(directoryEntry);
        }

        DirectoryEntry ADObject;
        UserPrincipal user;
        PrincipalContext pc = new PrincipalContext(ContextType.Domain, "10.120.112.100", SharedMethods.username, SharedMethods.password);

        public UserCard(string SID, string jumpToPage)
        {
            InitializeComponent();
            DirectoryEntry ADObject = new DirectoryEntry(SharedMethods.connectionString + SharedMethods.ADUserRoot, SharedMethods.username, SharedMethods.password);
            ADObject.UsePropertyCache = false;
            ADObject.AuthenticationType = AuthenticationTypes.ServerBind;

            DirectorySearcher deSearch = new DirectorySearcher(ADObject);
            deSearch.SearchScope = SearchScope.Subtree;
            deSearch.Filter = $"(&(objectCategory=person)(objectClass=User)(objectSid={SID}))";

            SearchResultCollection userResults = deSearch.FindAll();
            if (userResults.Count == 1) //Just one  Member Exists with this ID -- All good. I will check if any updates are needed and if they have been disabled.                
            {
                PopulateOverview(userResults[0].GetDirectoryEntry());
            }

            if (jumpToPage == "changepw") { tabControl1.SelectedTab = passwordGroupsPage;passwordMaskedTextBox.Select(); }
        }

       

        private void PopulateOverview(DirectoryEntry entry)
        {
            ADObject = entry;
            // find a user
            user = UserPrincipal.FindByIdentity(pc, entry.Properties["userPrincipalName"].Value.ToString());
            

            groupsListView.Items.Clear();

            fullNameLabel.Text = user.DisplayName;
            fullNameTextBox.Text = user.DisplayName;
            registrationLabel.Text = Convert.ToString(entry.Properties["department"].Value ?? "");
            emailLabel.Text = Convert.ToString(entry.Properties["userPrincipalName"].Value ?? "");

            firstNameTextBox.Text = Convert.ToString(entry.Properties["givenName"].Value ?? "");
            lastNameTextBox.Text = Convert.ToString(entry.Properties["sn"].Value ?? "");

            profileLetterTextBox.Text = Convert.ToString(entry.Properties["homeDrive"].Value ?? "");
            profilePathTextBox.Text = Convert.ToString(entry.Properties["homeDirectory"].Value ?? "");

            OUTextBox.Text = Convert.ToString(entry.Parent.Properties["distinguishedName"].Value ?? "");
            statusTextBox.Text = Convert.ToString(entry.Properties["comment"].Value ?? "").Replace("###", "");

            if(entry.Properties["lastLogonTimestamp"].Value != null)
            {
                IADsLargeInteger largeInt = (IADsLargeInteger)entry.Properties["lastLogonTimestamp"].Value;
                long datelong = (((long)largeInt.HighPart) << 32) + largeInt.LowPart;
                DateTime pwSet = DateTime.FromFileTimeUtc(datelong);
                lastLogonTextBox.Text = pwSet.ToString("dd/MM/yyyy HH:mm");
            }
            if (entry.Properties["pwdLastSet"].Value != null)
            {
                IADsLargeInteger largeInt = (IADsLargeInteger)entry.Properties["pwdLastSet"].Value;
                long datelong = (((long)largeInt.HighPart) << 32) + largeInt.LowPart;
                DateTime pwSet = DateTime.FromFileTimeUtc(datelong);
                pwChangedTextBox.Text = pwSet.ToString("dd/MM/yyyy HH:mm");
                pwChangedTextBox2.Text = pwSet.ToString("dd/MM/yyyy HH:mm");
            }

            if (user != null)
            {
                // get the user's groups
                var groups = user.GetGroups(pc);

                foreach (GroupPrincipal group in groups)
                {
                    ListViewItem ls = new ListViewItem();
                    ls.Text = group.Name;
                    ls.SubItems.Add(group.DistinguishedName);
                    groupsListView.Items.Add(ls);
                }
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            PopulateOverview(ADObject);
        }

        private void openHomeDirButton_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", $"/open, {profilePathTextBox.Text}");
        }

        private void showPasswordButton_Click(object sender, EventArgs e)
        {
            passwordMaskedTextBox.PasswordChar = '\0';
            passwordMaskedTextBox.Text = "Apples123#";
            confirmPWTextBox.Text = "";
        }

        private void setNewPasswordButton_Click(object sender, EventArgs e)
        {
            if(passwordMaskedTextBox.Text == confirmPWTextBox.Text)
            {      

                try
                {
                    string passwordToSet = passwordMaskedTextBox.Text;
                    user.SetPassword(passwordToSet);

                    if (changePWAtLogin.Checked) { user.ExpirePasswordNow(); }

                    user.Save();
                    MessageBox.Show($"Sucessfully changed users password.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to change users password: \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Unable to change users password: \n\nThe new password and confirm password field do not match.", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
 
        }

        private void removeFromGroup_Click(object sender, EventArgs e)
        {
            if(groupsListView.SelectedItems.Count != 0)
            {
                if (MessageBox.Show($"Are you sure you want to remove user: '{user.Name}' from the group: '{groupsListView.SelectedItems[0].Text}'?", "Confirm Action", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, groupsListView.SelectedItems[0].SubItems[1].Text);
                        group.Members.Remove(user);
                        group.Save();
                        groupsListView.Items.Remove(groupsListView.SelectedItems[0]);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unable to remove user from group: \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void addToGroup_Click(object sender, EventArgs e)
        {
            if (groupAddTextBox.Text != "")
            {
                if (MessageBox.Show($"Are you sure you want to add user: '{user.Name}' to the group: '{groupAddTextBox.Text}'?", "Confirm Action", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, groupAddTextBox.Text);
                        group.Members.Add(user);
                        group.Save();
                        ListViewItem li = new ListViewItem() ;
                        li.Text = group.Name;
                        li.SubItems.Add(group.DistinguishedName);
                        groupsListView.Items.Add(li);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unable to add user to group: \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void groupsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (groupsListView.SelectedItems.Count == 0)
            {
                removeFromGroup.Enabled = false;
            }
            else { removeFromGroup.Enabled = true; }
        }

        private void groupAddTextBox_TextChanged(object sender, EventArgs e)
        {
            if (addToGroup.Text.Length == 0)
            {
                addToGroup.Enabled = false;
            }
            else { addToGroup.Enabled = true; }
        }
    }
}
