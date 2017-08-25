using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryHistory
{
    public partial class GraphView : Form, IPushNewInfo
    {
        private Thread _thread;
        private bool showWindow;
        private string hostname;
        private bool destroyData;

        public GraphView(string hostname)
        {
            InitializeComponent();

            this.hostname = hostname;
            destroyData = false;
        }

        public GraphView(string hostname, bool destroyData)
        {
            InitializeComponent();

            this.hostname = hostname;
            this.destroyData = destroyData;
        }

        private void GraphView_ResizeEnd(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
        }

        private void ShowWindow()
        {
            showWindow = true;
            _thread = new Thread(WaitWindow);
            _thread.Start();
        }

        private void HideWindow()
        {
            showWindow = false;
        }

        private void WaitWindow()
        {
            Wait ww = new Wait();
            ww.Show();

            while (showWindow)
            {
                Application.DoEvents();
                Thread.Sleep(20);
            }

            ww.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowWindow();

            if (maskedTextBox1.Text == "  :")
            {
                maskedTextBox1.Text = "00:00";
            }
            if (maskedTextBox2.Text == "  :")
            {
                maskedTextBox2.Text = "23:59";
            }

            if (comboBox1.Items.Count == 0 || comboBox2.Items.Count == 0)
                return;

            DateTime from = DateTime.Parse(comboBox1.SelectedItem.ToString() + " " + maskedTextBox1.Text);
            DateTime to = DateTime.Parse(comboBox2.SelectedItem.ToString() + " " + maskedTextBox2.Text);

            if (from > to)
            {
                DateTime temp = from;
                from = to;
                to = temp;
            }

            GraphRenderer renderer = new GraphRenderer(HistoryManager.INSTANCE.GetData(this.hostname, from, to), pictureBox1.Width, pictureBox1.Height);
            lock (pictureBox1)
            {
                pictureBox1.Image = renderer.GenerateImage();
            }

            HideWindow();
        }

        private void GraphView_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(HistoryManager.INSTANCE.GetDates(this.hostname));
            comboBox2.Items.AddRange(HistoryManager.INSTANCE.GetDates(this.hostname));

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = comboBox2.Items.Count - 1;

            maskedTextBox1.Text = "00:00";
            maskedTextBox2.Text = "23:59";

            HistoryManager.INSTANCE.AddListener(this, this.hostname);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            ShowWindow();

            if (maskedTextBox1.Text == "  :")
            {
                maskedTextBox1.Text = "00:00";
            }
            if (maskedTextBox2.Text == "  :")
            {
                maskedTextBox2.Text = "23:59";
            }

            DateTime from = DateTime.Parse(comboBox1.SelectedItem.ToString() + " " + maskedTextBox1.Text);
            DateTime to = DateTime.Parse(comboBox2.SelectedItem.ToString() + " " + maskedTextBox2.Text);

            if (from > to)
            {
                DateTime temp = from;
                from = to;
                to = temp;
            }

            GraphRenderer renderer = new GraphRenderer(HistoryManager.INSTANCE.GetData(hostname, from, to), 1920, 1080);
            renderer.GenerateImage().Save(saveFileDialog1.FileName);

            HideWindow();
        }

        private void GraphView_SizeChanged(object sender, EventArgs e)
        {
            //button1_Click(null, null);
        }

        public void PushInformation()
        {
            if (maskedTextBox1.Text == "  :")
            {
                return;
            }
            if (maskedTextBox2.Text == "  :")
            {
                return;
            }

            string cb1 = (string)Invoke(new Func<string>(() => comboBox1.SelectedItem.ToString()));
            string cb2 = (string)Invoke(new Func<string>(() => comboBox2.SelectedItem.ToString()));
            DateTime from, to;
            try
            {
                from = DateTime.Parse(cb1 + " " + maskedTextBox1.Text);
                to = DateTime.Parse(cb2 + " " + maskedTextBox2.Text);
            }
            catch(FormatException)
            {
                return;
            }

            if (from > to)
            {
                DateTime temp = from;
                from = to;
                to = temp;
            }

            GraphRenderer renderer = new GraphRenderer(HistoryManager.INSTANCE.GetData(hostname, from, to), pictureBox1.Width, pictureBox1.Height);
            Image img = renderer.GenerateImage();

            lock (pictureBox1)
            {
                pictureBox1.Image = img;
            }
        }

        private void GraphView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (destroyData)
            {
                HistoryManager.INSTANCE.Remove(hostname);
            }
            HistoryManager.INSTANCE.RemoveListener(this, hostname);
        }

        private void GraphView_Shown(object sender, EventArgs e)
        {
            
        }
    }
}
