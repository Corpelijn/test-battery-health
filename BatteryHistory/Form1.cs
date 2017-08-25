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
    public partial class Form1 : Form
    {
        BatteryInfoRequester requester;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GC.Collect();
            textBox1.Enabled = false;
            button1.Enabled = false;
            button3.Visible = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            requester = new BatteryInfoRequester(textBox1.Text);
            requester.Start();

            backgroundWorker1.ReportProgress(0);

            while (!requester.IsDone)
            {
                Thread.Sleep(10);
            }

            if (requester.NotFound)
                backgroundWorker1.ReportProgress(1);
            else
                backgroundWorker1.ReportProgress(100);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                label2.Text = "Data ophalen...";
                label2.Visible = true;
            }
            else if (e.ProgressPercentage == 1)
            {
                label2.Text = "Niet gevonden!";
                textBox1.Enabled = true;
                button1.Enabled = true;
                textBox1.Focus();
            }
            else
            {
                label2.Text = "Done";
                this.Hide();
                GraphView gv = new GraphView(textBox1.Text);
                gv.ShowDialog();
                this.Show();
                HistoryManager.INSTANCE.Reset(textBox1.Text);

                textBox1.Text = "";
                textBox1.Enabled = true;
                button1.Enabled = true;
                label2.Visible = false;
                button3.Visible = true;
                textBox1.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (requester != null && !requester.IsDone)
                requester.Stop();

            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Multiview mv = new Multiview();
            mv.ShowDialog();
            this.Show();

            GC.Collect();
        }
    }
}
