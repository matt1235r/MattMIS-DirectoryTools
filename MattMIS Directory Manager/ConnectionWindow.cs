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
using System.Xml;
using System.Xml.Serialization;

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

            serverAddressTextBox.Text = Config.Settings.Connection.ServerAddress;
            usernameTextBox.Text = Config.Settings.Connection.Username;
            passwordTextBox.Text = Config.Settings.Connection.Password;
            domainRootTextBox.Text = Config.Settings.Connection.ADTreeRoot;
            userRootTextBox.Text = Config.Settings.Connection.ADUserRoot;

            if (Config.Settings.Connection.SkipConnectionWizard) { connectButton.PerformClick(); }
            if (passwordTextBox.Text == "") { this.ActiveControl = passwordTextBox; }
        }

        private void ConnectionWindow_Load(object sender, EventArgs e)
        {
            Config.Settings = new Config.Model.RootModel();

            toolTip1.SetToolTip(saveOptionsButton, "Save Current Settings to File");
            toolTip1.SetToolTip(loadFromFile, "Load Settings from File");
            toolTip1.SetToolTip(connectButton, "Attempt Connection to SPecified Domain");
            if (File.Exists("Config.xml"))
            {
                PopulateSettings("Config.xml");

            }
            else if (IPGlobalProperties.GetIPGlobalProperties().DomainName != String.Empty)
            {
                machineDomainJoined.Checked = true;
            }
            else
            {
                machineDomainJoined.Enabled = false;
                this.ActiveControl = serverAddressTextBox;
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (e.Argument.ToString() == "TEST")
                {

                    if (usernameTextBox.Text == "" || usernameTextBox.Text.EndsWith("(Current User)"))
                    {
                        Config.Settings.Connection.Username = null;
                        Config.Settings.Connection.Password = null;
                    }

                    DirectoryEntry de = new DirectoryEntry("LDAP://" + Config.Settings.Connection.ServerAddress, Config.Settings.Connection.Username, Config.Settings.Connection.Password);
                    de.AuthenticationType = AuthenticationTypes.ServerBind;

                    de.Options.PageSize.ToString();
                    connected = 1;
                }
                else if (e.Argument.ToString() == "TRYLOAD")
                {
                    var domain = Domain.GetCurrentDomain();

                    DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");

                    string defaultOU = rootDSE.Properties["defaultNamingContext"].Value.ToString();

                    Config.Settings = new Config.Model.RootModel();
                    Config.Settings.Connection = new Config.Model.ConnectionModel();
                    Config.Settings.PinnedViews = new Config.Model.PinnedViews();
                    Config.Settings.PinnedViews.CustomNodes = new List<Config.Model.TreeItem>();
                    Config.Settings.Appearance = new Config.Model.AppearanceModel();
                    Config.Settings.PinnedViews.CustomNodes.Add(new Config.Model.TreeItem());
  
                    Config.Settings.Connection.ADTreeRoot = defaultOU;
                    Config.Settings.Connection.ServerAddress = domain.DomainControllers[0].Name;
                    Config.Settings.Connection.Username = $"{Environment.UserDomainName}/{Environment.UserName} (Current User)";
                    Config.Settings.Connection.Password = "Skip Logon Authentication";
                    connected = 2;


                }
            }
            catch (Exception ex)
            {
                connected = 5;
                errorMessage = ex.Message;
            }

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (connected == 1)
            {
                progressBar.Style = ProgressBarStyle.Continuous;

                TabbedMain mainWindow = new TabbedMain();
                mainWindow.Show();
                this.Hide();

            }
            else if (connected == 2)
            {
                serverAddressTextBox.Text = Config.Settings.Connection.ServerAddress;
                usernameTextBox.Text = Config.Settings.Connection.Username;
                passwordTextBox.Text = Config.Settings.Connection.Password;
                domainRootTextBox.Text = Config.Settings.Connection.ADTreeRoot;
                userRootTextBox.Text = Config.Settings.Connection.ADUserRoot;

            }
            else if (connected == 5)
            {
                progressBar.Visible = false;
                MessageBox.Show($"Unable to connect to domain controller. Please check that you have entered the server address correctly and the credentials provided are correct.\n\nError Message: \n\n{errorMessage}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectButton.Enabled = true;
                connectButton.Text = "Connect";
            }

        }

        private void machineDomainJoined_CheckedChanged(object sender, EventArgs e)
        {
            if (machineDomainJoined.Checked) backgroundWorker.RunWorkerAsync(argument: "TRYLOAD");
            else { usernameTextBox.Clear(); passwordTextBox.Clear(); }

        }
        private void extraButton_Click(object sender, EventArgs e)
        {
            this.Size = new Size(482, 492);
            extraButton.Visible = false;
        }

        private void connectButton_Click_1(object sender, EventArgs e)
        {
            Config.Settings.Connection = new Config.Model.ConnectionModel();
            Config.Settings.Connection.ServerAddress = serverAddressTextBox.Text;
            Config.Settings.Connection.Username = usernameTextBox.Text;
            Config.Settings.Connection.Password = passwordTextBox.Text;
            if (domainRootTextBox.Text != String.Empty) Config.Settings.Connection.ADTreeRoot = "/" + domainRootTextBox.Text; else Config.Settings.Connection.ADTreeRoot = null;
            if (userRootTextBox.Text != String.Empty) Config.Settings.Connection.ADUserRoot = "/" + userRootTextBox.Text; else Config.Settings.Connection.ADUserRoot = null;

            connectButton.Enabled = false;
            connectButton.Text = "Attempting connection...";
            progressBar.Visible = true;
            backgroundWorker.RunWorkerAsync(argument: "TEST");
        }

        private void loadFromFile_Click_1(object sender, EventArgs e)
        {
            PopulateSettings();
        }

        private void saveOptionsButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.OK == saveFileDialog1.ShowDialog())
                {
                    Config.Model.RootModel config = new Config.Model.RootModel();
                   
                    config.Connection.ADTreeRoot = domainRootTextBox.Text;
                    config.Connection.ADUserRoot = userRootTextBox.Text;
                    config.Connection.ServerAddress = serverAddressTextBox.Text;
                    config.Connection.Username = usernameTextBox.Text;
                    config.Connection.Password = passwordTextBox.Text;

                    XmlSerializer serializer = new XmlSerializer(typeof(Config.Model.RootModel));
                    TextWriter txtWriter = new StreamWriter(saveFileDialog1.FileName);
                    serializer.Serialize(txtWriter, config);
                    txtWriter.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to save configuration file. \n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
