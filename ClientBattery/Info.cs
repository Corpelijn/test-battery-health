using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientBattery
{
    public partial class Info : Form
    {
        public Info()
        {
            InitializeComponent();

            Screen screen = Screen.FromPoint(Cursor.Position);
            this.Location = new Point(screen.WorkingArea.Width - this.Width, screen.WorkingArea.Height - this.Height);

            label1.Text = ApplicationStartup.Get().currentText;
            pictureBox1.Image = ApplicationStartup.Get().baseImage;
            label3.Text = ApplicationStartup.Get().warningText;
        }

        private void Info_Leave(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Info_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = ApplicationStartup.Get().currentText;
            pictureBox1.Image = ApplicationStartup.Get().baseImage;
            label3.Text = ApplicationStartup.Get().warningText;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
