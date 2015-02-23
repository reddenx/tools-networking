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
using SMT.Utilities.InputEvents.Interfaces;

namespace App.L33T3R1Zr
{
    public partial class Form1 : Form
    {
        private IKeyboardEventListener KeyboardListener;
        private IKeyboardEventRunner KeyboardRunner;

        public Form1(IKeyboardEventListener keyboardListener)
        {
            KeyboardListener = keyboardListener;
            KeyboardRunner = new KeyboardEventRunner();

            InitializeComponent();

            this.textBox1.Enabled = false;
        }

        private void PrependText(string msg)
        {
            this.textBox1.Text = msg + "\r\n" + this.textBox1.Text;
        }

        private void StartClick(object sender, EventArgs e)
        {
            KeyboardListener.StartListening(HandleEvent);
        }

        private void StopClick(object sender, EventArgs e)
        {
            KeyboardListener.StopListening();
        }

        private KeyboardEventResult HandleEvent(KeyboardEventArgs e)
        {
            PrependText(string.Format("{0} {1} {2}",
                " v" + e.VirtualKey,
                " s" + e.ScanKey.ToString(),
                (e.Pressed ? " D" : " U")));
            if (LeetReplacementDefinition.Replacements.ContainsKey(e.Key))
            {
                if (e.Pressed)
                {
                    var newKey = LeetReplacementDefinition.Replacements[e.Key];
                    if (newKey[0].shift)
                    {
                        KeyboardRunner.DoEvent(KeyboardEventArgs.Shift(true));
                    }

                    KeyboardRunner.DoEvent(new KeyboardEventArgs(newKey[0].vk, newKey[0].sk, true));
                    KeyboardRunner.DoEvent(new KeyboardEventArgs(newKey[0].vk, newKey[0].sk, false));

                    if (newKey[0].shift)
                    {
                        KeyboardRunner.DoEvent(KeyboardEventArgs.Shift(false));
                    }
                }
                return new KeyboardEventResult(true);
            }
            return new KeyboardEventResult(false);
        }

        private void TestingClick(object sender, EventArgs e)
        {
        }
    }
}
