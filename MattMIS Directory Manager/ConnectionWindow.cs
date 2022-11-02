using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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

        int connected = 0;
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
            backgroundWorker.RunWorkerAsync(argument: "TEST");
        }

        private void loadFromFile_Click(object sender, EventArgs e)
        {
            PopulateSettings();
        }

        private void PopulateSettings(string path = "")
        {
            if (path == "" && openFileDialog1.ShowDialog() == DialogResult.OK)
            {
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
                if (e.Argument.ToString() == "TEST")
                {
                    DirectoryEntry de;
                    if (machineDomainJoined.Checked)
                    {
                        if (usernameTextBox.Text == "" || passwordTextBox.Text == "")
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
                    connected = 1;
                }
                else if (e.Argument.ToString() == "TRYLOAD")
                {
                    var domain = Domain.GetCurrentDomain();
                    DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");


                    Config.Settings.ADUserRoot = rootDSE.Properties["defaultNamingContext"].Value.ToString();
                    Config.Settings.ADTreeRoot = rootDSE.Properties["defaultNamingContext"].Value.ToString();
                    Config.Settings.ServerAddress = rootDSE.Path;
                    Config.Settings.Username = rootDSE.Username;
                    Config.Settings.Password = "SKIP";
                    connected = 2;


                }
            }
            catch (Exception ex)
            {
                connected = 5;
                errorMessage = ex.Message + "\n\n" + ex.InnerException;
            }

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            if (connected == 1)
            {
                progressBar.Style = ProgressBarStyle.Continuous;

                DirectoryManager mainWindow = new DirectoryManager();
                mainWindow.Show();
                this.Hide();

            }
            else if (connected == 2)
            {
                serverAddressTextBox.Text = Config.Settings.ServerAddress;
                usernameTextBox.Text = Config.Settings.Username;
                passwordTextBox.Text = Config.Settings.Password;
                domainRootTextBox.Text = Config.Settings.ADTreeRoot;
                userRootTextBox.Text = Config.Settings.ADUserRoot;

            }
            else if (connected == 5)
            {
                progressBar.Visible = false;
                MessageBox.Show($"Unable to connect to domain controller. Please check you have entered the server address correctly.\n\nError Message: \n{errorMessage}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectButton.Enabled = true;
                connectButton.Text = "Connect";
            }

        }

        private void machineDomainJoined_CheckedChanged(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync(argument: "TRYLOAD");
        }
    }
}
