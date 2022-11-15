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

namespace MattMIS_Directory_Manager
{
    public partial class rename_Object : Form
    {
        public rename_Object()
        {
            InitializeComponent();
        }

        DirectoryEntry directoryEntry;
        string type;
        int status = 0;
        string objPrefix = "CN=";

        public rename_Object(Handler.UserModel model)
        {
            InitializeComponent();
            
            this.directoryEntry = model.directoryEntry;
            cueTextBox2.Text = Convert.ToString(model.FullName ?? "");
            if (model.ObjectType == "organizationalUnit") objPrefix = "OU=";
            type = model.ObjectType;
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public int GetStatus()
        {
            return status;
        }


        private void okayButton_Click(object sender, EventArgs e)
        {
            this.Close();

            if (type == "user") directoryEntry.Properties["displayName"].Value = cueTextBox2.Text;
            directoryEntry.CommitChanges();
            try
            {  
                if (fullRenameCheckBox.Checked)
                {
                    directoryEntry.Rename(objPrefix + cueTextBox2.Text);
                    directoryEntry.CommitChanges();
                    status = 10;
                }
            }
            catch (Exception ex )
            {
                MessageBox.Show("Unable to rename object. \n\nError Message\n" + ex.Message);
            }
            
        }
    }
}
