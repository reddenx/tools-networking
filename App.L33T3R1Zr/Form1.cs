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
        private bool IsConverting;
        
        private bool ShiftDown;
        private bool AltDown;
        private bool CtrlDown;

        public Form1(IKeyboardEventListener keyboardListener)
        {
            KeyboardListener = keyboardListener;
            KeyboardRunner = new KeyboardEventRunner();
            IsConverting = false;

            ShiftDown = false;
            AltDown = false;
            CtrlDown = false;

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

            HandleHotKeys(e);

            if (IsConverting && CanConvert() && LeetReplacementDefinition.Replacements.ContainsKey(e.Key))
            {
                if (e.Pressed)
                {
                    foreach (var newKey in LeetReplacementDefinition.Replacements[e.Key])
                    {
                        if (newKey.shift)
                        {
                            KeyboardRunner.DoEvent(KeyboardEventArgs.Shift(true));
                        }

                        KeyboardRunner.DoEvent(new KeyboardEventArgs(newKey.vk, newKey.sk, true));
                        KeyboardRunner.DoEvent(new KeyboardEventArgs(newKey.vk, newKey.sk, false));

                        if (newKey.shift)
                        {
                            KeyboardRunner.DoEvent(KeyboardEventArgs.Shift(false));
                        }
                    }
                }
                return new KeyboardEventResult(true);
            }
            return new KeyboardEventResult(false);
        }

        private void HandleHotKeys(KeyboardEventArgs e)
        {
            if (e.Key == Keys.LControlKey)
                CtrlDown = e.Pressed;

            if (e.Key == Keys.LMenu)
                AltDown = e.Pressed;

            if (e.Key == Keys.LShiftKey)
                ShiftDown = e.Pressed;

            if (CtrlDown && AltDown && ShiftDown)
            {
                IsConverting = !IsConverting;
                textBox1.BackColor = IsConverting ? Color.Red : DefaultBackColor;
            }
        }

        private bool CanConvert()
        {
            return !(CtrlDown || AltDown || ShiftDown);
        }

        private void TestingClick(object sender, EventArgs e)
        {
        }
    }
}
