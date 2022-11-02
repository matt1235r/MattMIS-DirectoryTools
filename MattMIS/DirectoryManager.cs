using ComponentOwl.BetterListView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MattMIS_Sync
{
    public partial class DirectoryManager : Form
    {
        public DirectoryManager()
        {
            InitializeComponent();
        }

        private void DirectoryManager_Load(object sender, EventArgs e)
        {
            LoadDirectoryObject("LDAP://OU=2022,OU=Pupils,OU=User Accounts,DC=tcs,DC=local");
        }

        private void LoadDirectoryObject(string OU)
        {
            DirectoryEntry ADObject = new DirectoryEntry("LDAP://DC=tcs,DC=local", "tcs\\a", "DLDN/1164-la", AuthenticationTypes.Secure);
            MessageBox.Show(ADObject.Name.ToString());


            DirectorySearcher deSearch = new DirectorySearcher(ADObject);
            deSearch.SizeLimit = 3000;

            deSearch.SearchScope = SearchScope.Subtree;
            deSearch.Filter = $"(&(objectCategory=person)(objectClass=User))";
            SearchResultCollection userResults = deSearch.FindAll();
            
           
            foreach (DirectoryEntry userResult in userResults)
            {
                BetterListViewItem betterListViewItem = new BetterListViewItem();
                betterListViewItem.Text = userResult.Name;
                mainListView.Items.Add(betterListViewItem);
                
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDirectoryObject("LDAP://OU=2022,OU=Pupils,OU=User Accounts,DC=tcs,DC=local");
        }
    }
}
