using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MattMIS_Directory_Manager
{
    public partial class StringDialogBox : Form
    {
        public StringDialogBox()
        {
            InitializeComponent();
        }

        public StringDialogBox(string message, string goButton, bool multiLine = false)
        {
            InitializeComponent();
            richTextBox1.Text = message;
            okayButton.Text = goButton;
            textBox2.Visible = multiLine;
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StringDialogBox_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
        }

        public string GetValue()
        {
            return textBox1.Text;
        }

        public string GetValue2()
        {
            return textBox2.Text;
        }

        private void okayButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
