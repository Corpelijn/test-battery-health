using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryHistory
{
    public partial class Multiview : Form
    {
        public Multiview()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.Cells.Add(new DataGridViewTextBoxCell { Value = "LAP25828" });
            row.Cells.Add(new DataGridViewButtonCell { Value = "View" });
            row.Cells.Add(new DataGridViewButtonCell { Value = "Open" });
            dataGridView1.Rows.Add(row);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                string laptop = (string)senderGrid.Rows[e.RowIndex].Cells[0].Value;
                GraphViewer viewer = new GraphViewer(laptop);
                viewer.Parent = panel2;
                viewer.BringToFront();
            }
        }
    }
}
