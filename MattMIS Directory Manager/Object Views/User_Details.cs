
using ActiveDs;
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

        public UserCard(DirectoryEntry directoryEntry, string jumpToPage = "")
        {
            InitializeComponent();
            PopulateOverview(directoryEntry);
            if (jumpToPage == "changepw") { tabControl1.SelectedTab = passwordGroupsPage; passwordMaskedTextBox.Select(); }
        }

        DirectoryEntry ADObject;
        UserPrincipal user;
        PrincipalContext pc = new PrincipalContext(ContextType.Domain, Config.Settings.Connection.ServerAddress, Config.Settings.Connection.Username, Config.Settings.Connection.Password);
        private void PopulateOverview(DirectoryEntry entry)
        {
            ADObject = entry;
            // find a user
            user = UserPrincipal.FindByIdentity(pc, entry.Properties["objectSid"].Value.ToString());
            

            groupsListView.Items.Clear();

            fullNameLabel.Text = user.DisplayName;
            fullNameTextBox.Text = user.DisplayName;
            registrationLabel.Text = Convert.ToString(entry.Properties["department"].Value ?? "");
            emailLabel.Text = Convert.ToString(entry.Properties["userPrincipalName"].Value ?? "");

            firstNameTextBox.Text = Convert.ToString(entry.Properties["givenName"].Value ?? "");
            lastNameTextBox.Text = Convert.ToString(entry.Properties["sn"].Value ?? "");

            driveComboBox.Text = Convert.ToString(entry.Properties["homeDrive"].Value ?? "");
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
                groupsListView.Items.Clear();
                foreach (GroupPrincipal group in groups)
                {
                    ListViewItem ls = new ListViewItem();
                    ls.Text = group.Name;
                    ls.SubItems.Add(group.DistinguishedName);
                    groupsListView.Items.Add(ls);
                }
            }

            //EMAIL View
            proxyListBox.Items.Clear();
            foreach (String address in (ADObject.Properties["proxyaddresses"]))
            {
                proxyListBox.Items.Add(address);

            }

            showAddressBook.Checked = !Convert.ToBoolean(ADObject.Properties["msExchHideFromAddressLists"].Value ?? false);
            emailTextBox.Text = Convert.ToString(ADObject.Properties["mail"].Value ?? "");

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
                    MessageBox.Show($"Sucessfully changed password.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to change password: \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Unable to change password: \n\nThe new password and confirm password field do not match.", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
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

        private void saveEmailSettings_Click(object sender, EventArgs e)
        {
            
        }

        private void UserCard_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ADObject.Properties["proxyaddresses"].Add(textBox2.Text);
            textBox2.Clear();
            ADObject.CommitChanges();
            PopulateOverview(ADObject);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (proxyListBox.SelectedItem == null) return;
            ADObject.Properties["proxyaddresses"].Remove(proxyListBox.SelectedItem.ToString());
            ADObject.CommitChanges();
            PopulateOverview(ADObject);
        }

        private void proxyListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (proxyListBox.SelectedItem == null) button2.Enabled = false ;
            else button2.Enabled = true;

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "") button1.Enabled = false;
            else button1.Enabled = true;
        }

        private void saveOverviewButton_Click(object sender, EventArgs e)
        {
            //OVERVIEW
            user.DisplayName = fullNameLabel.Text;
           user.DisplayName = fullNameTextBox.Text;
            ADObject.Properties["department"].Value = registrationLabel.Text;
            ADObject.Properties["userPrincipalName"].Value = emailLabel.Text;

            ADObject.Properties["givenName"].Value = firstNameTextBox.Text;
            ADObject.Properties["sn"].Value = lastNameTextBox.Text;

            ADObject.Properties["homeDrive"].Value = driveComboBox.Text;
            ADObject.Properties["homeDirectory"].Value = profilePathTextBox.Text;

            //EMAIL
            ADObject.Properties["msExchHideFromAddressLists"].Value = !showAddressBook.Checked;
            ADObject.Properties["mail"].Value = emailTextBox;
            ADObject.CommitChanges();
        }
    }
}
