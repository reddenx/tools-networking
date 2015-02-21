using SMT.Utilities.InputEvents.HardwareEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App.L33T3R1Zr
{
    public partial class Form1 : Form
    {
        private KeyboardEventListener KeyboardListener;

        public Form1(KeyboardEventListener keyboardListener)
        {
            KeyboardListener = keyboardListener;

            InitializeComponent();

            this.textBox1.Enabled = false;

            KeyboardListener.SetHook(PrependText);
        }

        private void PrependText(string msg)
        {
            this.textBox1.Text = msg + "\r\n" + this.textBox1.Text;
        }
    }
}
