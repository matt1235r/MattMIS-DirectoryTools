using System.Windows.Forms;

namespace MattMIS_Directory_Manager
{
    partial class create_Object : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.okayButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pathTextBox = new AeroSuite.Controls.CueTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.objectNameTextBox = new AeroSuite.Controls.CueTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.friendlyNameTextBox = new AeroSuite.Controls.CueTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okayButton
            // 
            this.okayButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okayButton.Location = new System.Drawing.Point(196, 183);
            this.okayButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.okayButton.Name = "okayButton";
            this.okayButton.Size = new System.Drawing.Size(105, 32);
            this.okayButton.TabIndex = 3;
            this.okayButton.Text = "Create";
            this.okayButton.UseVisualStyleBackColor = true;
            this.okayButton.Click += new System.EventHandler(this.okayButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitButton.Location = new System.Drawing.Point(10, 183);
            this.exitButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(105, 32);
            this.exitButton.TabIndex = 4;
            this.exitButton.Text = "Cancel";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pathTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.objectNameTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.friendlyNameTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(288, 165);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // pathTextBox
            // 
            this.pathTextBox.Cue = "OU=name";
            this.pathTextBox.Enabled = false;
            this.pathTextBox.Location = new System.Drawing.Point(11, 127);
            this.pathTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(269, 23);
            this.pathTextBox.TabIndex = 5;
            this.pathTextBox.DoubleClick += new System.EventHandler(this.pathTextBox_DoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 107);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Path:";
            // 
            // objectNameTextBox
            // 
            this.objectNameTextBox.Cue = "OU=name";
            this.objectNameTextBox.Location = new System.Drawing.Point(110, 61);
            this.objectNameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.objectNameTextBox.Name = "objectNameTextBox";
            this.objectNameTextBox.Size = new System.Drawing.Size(170, 23);
            this.objectNameTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Object Name:";
            // 
            // friendlyNameTextBox
            // 
            this.friendlyNameTextBox.Cue = "Enter name";
            this.friendlyNameTextBox.Location = new System.Drawing.Point(110, 22);
            this.friendlyNameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.friendlyNameTextBox.Name = "friendlyNameTextBox";
            this.friendlyNameTextBox.Size = new System.Drawing.Size(170, 23);
            this.friendlyNameTextBox.TabIndex = 1;
            this.friendlyNameTextBox.TextChanged += new System.EventHandler(this.friendlyNameTextBox_TextChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Friendly Name:";
            // 
            // create_Object
            // 
            this.AcceptButton = this.okayButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.exitButton;
            this.ClientSize = new System.Drawing.Size(322, 227);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.okayButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "create_Object";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create New Object";
            this.Load += new System.EventHandler(this.StringDialogBox_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button okayButton;
        private System.Windows.Forms.Button exitButton;
        private GroupBox groupBox1;
        private AeroSuite.Controls.CueTextBox friendlyNameTextBox;
        private Label label1;
        private AeroSuite.Controls.CueTextBox pathTextBox;
        private Label label3;
        private AeroSuite.Controls.CueTextBox objectNameTextBox;
        private Label label2;
    }
}