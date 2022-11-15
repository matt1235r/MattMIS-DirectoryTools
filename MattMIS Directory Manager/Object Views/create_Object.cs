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
    public partial class create_Object : Form
    {
        public create_Object()
        {
            InitializeComponent();
        }

        DirectoryEntry parent;
        string parentPath;
        string objType;
        int status = 0;

        public create_Object(Handler.TreeModel selectedNode, string type)
        {
            InitializeComponent();
            if (type != "organizationalUnit") this.Close();
            if (selectedNode.Command != "DISPLAYOU" && selectedNode.Command != "NAVIGATEOU") this.Close();

            DirectoryEntry parent = new DirectoryEntry(selectedNode.Argument, Config.Settings.Connection.Username, Config.Settings.Connection.Password);
            parentPath = parent.Properties["distinguishedName"].Value.ToString();
            this.parent = parent;
            this.objType = type;
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StringDialogBox_Load(object sender, EventArgs e)
        {
            
        }

        public string GetPath()
        {
            return pathTextBox.Text;
        }

        public string GetObjType()
        {
            return objType;
        }

        public int GetStatus()
        {
            return status;
        }

        private void okayButton_Click(object sender, EventArgs e)
        {
            this.Close();
            try
            {
                DirectoryEntry child = parent.Children.Add(objectNameTextBox.Text, objType);
                child.Properties["description"].Add(friendlyNameTextBox.Text);
                child.CommitChanges();
                
                status = 10;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create object. \n\nError Message\n" + ex.Message);
                this.Close();
            }
        }

        private void friendlyNameTextBox_TextChanged_1(object sender, EventArgs e)
        {
            objectNameTextBox.Text = "OU=" + friendlyNameTextBox.Text;
            pathTextBox.Text = objectNameTextBox.Text + "," + parentPath;
        }

        private void pathTextBox_DoubleClick(object sender, EventArgs e)
        {
            pathTextBox.Enabled = true;
        }
    }
}
