using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseSampler;

namespace RSHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MouseSample.LoadSamples();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MouseSample.MoveTo(new Point(800, 800));
        }
    }
}
