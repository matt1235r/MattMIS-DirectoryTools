namespace MattMIS_Directory_Manager
{
    partial class TabbedMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabbedMain));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.headerlessTabPage1 = new AeroSuite.Controls.HeaderlessTabPage();
            this.headerlessTabPage2 = new AeroSuite.Controls.HeaderlessTabPage();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.ImageList = this.imageList1;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(40, 3);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(898, 538);
            this.tabControl.TabIndex = 0;
            this.tabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl_DrawItem);
            this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
            this.tabControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControl_MouseDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "DeleteButton_Image");
            this.imageList1.Images.SetKeyName(1, "AddButton_Image");
            // 
            // headerlessTabPage1
            // 
            this.headerlessTabPage1.Location = new System.Drawing.Point(0, 0);
            this.headerlessTabPage1.Name = "headerlessTabPage1";
            this.headerlessTabPage1.Size = new System.Drawing.Size(200, 100);
            this.headerlessTabPage1.TabIndex = 0;
            // 
            // headerlessTabPage2
            // 
            this.headerlessTabPage2.Location = new System.Drawing.Point(0, 0);
            this.headerlessTabPage2.Name = "headerlessTabPage2";
            this.headerlessTabPage2.Size = new System.Drawing.Size(200, 100);
            this.headerlessTabPage2.TabIndex = 0;
            // 
            // TabbedMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(898, 538);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TabbedMain";
            this.Text = "MattMIS: Directory Manager";
            this.Load += new System.EventHandler(this.TabbedMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.ImageList imageList1;
        private AeroSuite.Controls.HeaderlessTabPage headerlessTabPage1;
        private AeroSuite.Controls.HeaderlessTabPage headerlessTabPage2;
    }
}