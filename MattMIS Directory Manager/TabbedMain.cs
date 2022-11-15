using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MattMIS_Directory_Manager
{
    public partial class TabbedMain : Form
    {
        public TabbedMain()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int TCM_SETMINTABWIDTH = 0x1300 + 49;
        private void tabControl_HandleCreated(object sender, EventArgs e)
        {
            SendMessage(this.tabControl.Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)16);
        }


        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = this.tabControl.TabPages[e.Index];
            var tabRect = this.tabControl.GetTabRect(e.Index);


                // e.Graphics.FillRectangle(blueBrush, tabRect);
                //tabRect.Inflate(-2, -2);
                if (e.Index == this.tabControl.TabCount - 1)
            {
                var addImage = imageList1.Images["AddButton_Image"];
                e.Graphics.DrawImage(addImage,
                    tabRect.Left + (tabRect.Width - addImage.Width) / 2,
                    tabRect.Top + (tabRect.Height - addImage.Height) / 2);
            }
            else
            {
                var closeImage = imageList1.Images["DeleteButton_Image"];
                e.Graphics.DrawImage(closeImage,
                    (tabRect.Right - closeImage.Width),
                    tabRect.Top + (tabRect.Height - closeImage.Height) / 2);

                
                TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
                    tabRect, tabPage.ForeColor, TextFormatFlags.Left);
            }
        }

        private void tabControl_MouseDown(object sender, MouseEventArgs e)
        {
            var lastIndex = this.tabControl.TabCount - 1;
            if (this.tabControl.GetTabRect(lastIndex).Contains(e.Location))
            {
                TabPage tb = new TabPage();
                tb.Text = "Home - Directory Manager";
                DirectoryManager dm = new DirectoryManager(tb);
                
                tb.Controls.Add(dm);
                dm.Dock = DockStyle.Fill;

                this.tabControl.TabPages.Insert(lastIndex, tb);
                this.tabControl.SelectedIndex = lastIndex;
                
            }
            else
            {
                for (var i = 0; i < this.tabControl.TabPages.Count; i++)
                {
                    var tabRect = this.tabControl.GetTabRect(i);
                    tabRect.Inflate(-2, -2);
                    var closeImage = imageList1.Images["DeleteButton_Image"];
                    var imageRect = new Rectangle(
                        (tabRect.Right - closeImage.Width),
                        tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                        closeImage.Width,
                        closeImage.Height);
                    if (imageRect.Contains(e.Location))
                    {
                        this.tabControl.TabPages.RemoveAt(i);
                        if (tabControl.TabPages.Count <= 1) Application.Exit();
                        break;
                        
                    }
                }
            }
        }

        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == this.tabControl.TabCount - 1)
                e.Cancel = true;
        }

        private void TabbedMain_Load(object sender, EventArgs e)
        {           
            this.tabControl.Padding = new Point(12, 4);
            this.tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            TabPage tb = new TabPage();
            tb.Text = "Home - Directory Manager";
            DirectoryManager dm = new DirectoryManager(tb);

            tb.Controls.Add(dm);
            dm.Dock = DockStyle.Fill;

            this.tabControl.TabPages.Add(tb);
            this.tabControl.TabPages.Add("");
        }

        private void headerlessTabControl1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
