using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryHistory
{
    public partial class GraphViewer : UserControl, IPushNewInfo
    {
        public GraphViewer(string hostname)
        {
            InitializeComponent();

            this.Dock = DockStyle.Top;
            label1.Text = hostname;
            HistoryManager.INSTANCE.AddHistory(hostname);
            HistoryManager.INSTANCE.AddListener(this, hostname);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            HistoryManager.INSTANCE.Remove(label1.Text);
            HistoryManager.INSTANCE.RemoveListener(this, label1.Text);
            this.Dispose();
            GC.Collect();

            base.OnHandleDestroyed(e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.AliceBlue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            HistoryManager.INSTANCE.Remove(label1.Text);
            HistoryManager.INSTANCE.RemoveListener(this, label1.Text);
            this.Dispose();
            GC.Collect();
        }

        public void PushInformation()
        {
            TimeData[] data = HistoryManager.INSTANCE.GetData(label1.Text, DateTime.Now.AddHours(-3), DateTime.Now);
            if (data.Length == 0)
                return;

            if (button1.IsDisposed)
                return;

            GraphRenderer renderer = new GraphRenderer(data, pictureBox1.Width, pictureBox1.Height);
            //pictureBox1.Image = renderer.GenerateImage();
            pictureBox1.GetType().GetProperty("Image").SetValue(pictureBox1, renderer.GenerateImage());

            Invoke(new Action(() => button1.Location = new Point(Width - button1.Width - 3, 0)));
            Invoke(new Action(() => button2.Location = new Point(Width - button2.Width - 3 - button1.Width - 3, 0)));
            Invoke(new Action(() => label1.Location = new Point(Width - button2.Width - 3 - button1.Width - 3 - label1.Width - 3, 0)));
        }
    }
}
