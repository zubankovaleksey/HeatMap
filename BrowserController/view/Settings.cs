using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowserController.view
{
    public partial class Settings : Form
    {
        public static bool isAbsolutCounting = true;
        public static double redBottom = 0.1;
        public static double yellowBottom = 0.01;
        public static double orangeBottom = 0.05;
        public static int sqrSize = 70;
        public Settings()
        {
            InitializeComponent();
            numericUpDown1.Value = (decimal)redBottom * 100;
            numericUpDown2.Value = (decimal)orangeBottom * 100;
            numericUpDown3.Value = (decimal)yellowBottom * 100;
            numericUpDown4.Value = sqrSize;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void setNumericsMaximums(object sender, EventArgs e)
        {
            numericUpDown2.Maximum = numericUpDown1.Value;
            numericUpDown3.Maximum = numericUpDown2.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            redBottom = (double)numericUpDown1.Value / 100;
            orangeBottom = (double)numericUpDown2.Value / 100;
            yellowBottom = (double)numericUpDown3.Value / 100;
            sqrSize = (int)numericUpDown4.Value;
            Close();
        }
        
    }
}
