using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseSampling;
using MouseControl;

namespace RSHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MouseHook.Start();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!MouseSample.Recording)
            {
                MouseSample.StartRecord();
                MouseHook.RMouseAction += MouseSample.StartNewSample;
                MouseHook.LMouseAction += MouseSample.StartNewSample;
            }
            else
            {
                MouseSample.StopRecord();
                MouseHook.RMouseAction -= MouseSample.StartNewSample;
                MouseHook.LMouseAction -= MouseSample.StartNewSample;
            }
        }
    }
}
