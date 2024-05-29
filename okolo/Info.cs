using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace okolo
{
    public partial class Info : Form
    {
        public Info()
        {
            InitializeComponent();
        }

        private void SwitchToTab(string tabName)
        {
            foreach (TabPage tab in tabControl1.TabPages)
            {
                if (tab.Text == tabName)
                {
                    tabControl1.SelectedTab = tab;
                    return;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SwitchToTab("tabPage2");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Вы на первой странице!");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SwitchToTab("Начало работы!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
