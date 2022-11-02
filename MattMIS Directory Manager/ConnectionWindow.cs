using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MattMIS_Directory_Manager
{
    public partial class ConnectionWindow : Form
    {
        public ConnectionWindow()
        {
            InitializeComponent();
        }

        bool connected = false;
        string errorMessage = "";
        private void connectButton_Click(object sender, EventArgs e)
        {
            Config.Settings.ServerAddress = serverAddressTextBox.Text;
            Config.Settings.Username = usernameTextBox.Text;
            Config.Settings.Password = passwordTextBox.Text;
            Config.Settings.ADTreeRoot = domainRootTextBox.Text;
            Config.Settings.ADUserRoot = userRootTextBox.Text;

            connectButton.Enabled = false;
            connectButton.Text = "Attempting connection...";
            progressBar.Visible = true;
            backgroundWorker.RunWorkerAsync();
        }

        private void loadFromFile_Click(object sender, EventArgs e)
        {
            PopulateSettings();
        }

        private void PopulateSettings(string path = "")
        {           
            if(path == "" && openFileDialog1.ShowDialog() == DialogResult.OK){
                path = openFileDialog1.FileName;
            }
            if (Config.LoadConfig(path) == 1) return;

            serverAddressTextBox.Text = Config.Settings.ServerAddress;
            usernameTextBox.Text = Config.Settings.Username;
            passwordTextBox.Text = Config.Settings.Password;
            domainRootTextBox.Text = Config.Settings.ADTreeRoot;
            userRootTextBox.Text = Config.Settings.ADUserRoot;

            if (Config.Settings.SkipConnectionWizard) { connectButton.PerformClick(); }
            if (passwordTextBox.Text == "") { this.ActiveControl = passwordTextBox; }
        }

        private void ConnectionWindow_Load(object sender, EventArgs e)
        {
            if (File.Exists("Config.xml"))
            {
                PopulateSettings("Config.xml");
                
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DirectoryEntry de;
                if (machineDomainJoined.Checked)
                {
                    if(usernameTextBox.Text == "" || passwordTextBox.Text == "")
                    {
                        de = new DirectoryEntry("LDAP://" + Config.Settings.ADUserRoot);
                    }
                    de = new DirectoryEntry("LDAP://" + Config.Settings.ADUserRoot, Config.Settings.Username, Config.Settings.Password);
                }
                else
                {
                    de = new DirectoryEntry("LDAP://" + Config.Settings.ServerAddress + "/" + Config.Settings.ADUserRoot, Config.Settings.Username, Config.Settings.Password);
                    de.AuthenticationType = AuthenticationTypes.ServerBind;
                }
               

                string value = de.Properties["Name"].Value.ToString();
                connected = true;
            }
            catch (Exception ex)
            {
                connected = false;
                errorMessage = ex.Message;
            }
            
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (connected)
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                
                DirectoryManager mainWindow = new DirectoryManager();
                mainWindow.Show();
                this.Hide();

            }
            else
            {
                progressBar.Visible = false;
                MessageBox.Show(errorMessage);
                connectButton.Enabled = true;
                connectButton.Text = "Connect";
            }
        }
    }
}
