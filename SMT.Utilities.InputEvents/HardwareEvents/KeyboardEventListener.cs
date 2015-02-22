using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMT.Utilities.InputEvents.HardwareEvents
{
    //TODO-SM move event logic to injectable actions, listener shouldn't be doing key replacements and index tracking
    public class KeyboardEventListener : IDisposable
    {
        private HookHandlerDelegate proc;
        private IntPtr hookID = IntPtr.Zero;
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private Action<string> HandleEvent;

        private delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        public KeyboardEventListener()
        {
            proc = new HookHandlerDelegate(HookCallback);
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hookID = SetWindowsHookEx(WH_KEYBOARD_LL, proc, LoadLibrary("User32"), 0);
                //hookID = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public void SetHook(Action<string> eventHandler)
        {
            this.HandleEvent = eventHandler;
        }

        private string LoopingMessage = "never gonna give you up never gonna let you down never gonna run around and desert you never gonna make you cry never gonna say goodbye never gonna tell a lie and hurt you ";
        private int LoopingArrayIndex = 0;

        //private IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        //{


        //    if (nCode >= 0 && HandleEvent != null)
        //    {
        //        if (lParam.dwExtraInfo == (IntPtr)0 && (uint)wParam == 256 && (lParam.wVk >= 'A' && lParam.wVk <= 'Z' || lParam.wVk == (int)(Keys.Space) ))
        //        {
        //            var character = LoopingMessage[LoopingArrayIndex % LoopingMessage.Length];
        //            ++LoopingArrayIndex;

        //            byte vk = (byte)ConvertCharToVirtualKey(character);

        //            keybd_event(vk, 45, KEYEVENTF_EXTENDEDKEY, 1);
        //            keybd_event(vk, 45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 1);

        //            HandleEvent(((Keys)lParam.wVk).ToString() + " -> " + ((Keys)vk).ToString());

        //            return (System.IntPtr)1;
        //        }
        //    }


        //    return CallNextHookEx(hookID, nCode, wParam, ref lParam);
        //}

        //replacement keys
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var lParamObj = (KeyboardHardwareEventData)Marshal.PtrToStructure(lParam, typeof(KeyboardHardwareEventData));
            HandleEvent(((Keys)lParamObj.vkCode).ToString() + " v" + lParamObj.vkCode + " s" + lParamObj.scanCode + " " + wParam.ToString() + " " + lParamObj.dwExtraInfo);



            //TODO-SM uncomment properly
            //if ((uint)wParam != 256)
            //HandleEvent(((Keys)lParam.wVk).ToString() + " v" + lParam.wVk + " s" + lParam.wScan + " " + wParam.ToString() + " " + lParam.dwExtraInfo);

            //if (nCode >= 0 && HandleEvent != null && lParam.dwExtraInfo == (IntPtr)0)
            //{
            //    if (ReplaceDefinitions.Replacements.ContainsKey((Keys)lParam.wVk))
            //    {
            //        //if it's an incoming key that has a replacement, we've already handled the up portion
            //        if ((uint)wParam != 256)
            //        {
            //            return (System.IntPtr)1;
            //        }

            //        //keybd_event(90, 44, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 1); //EXAMPLE
            //        //uint flag = (uint)wParam == 256 ? KEYEVENTF_EXTENDEDKEY : KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            //        var data = ReplaceDefinitions.Replacements[((Keys)lParam.wVk)];

            //        foreach (var key in data)
            //        {
            //            if (key.shift)
            //            {
            //                keybd_event(160, 42, KEYEVENTF_EXTENDEDKEY, 1);
            //            }

            //            keybd_event(key.vk, key.sk, KEYEVENTF_EXTENDEDKEY, 1);
            //            keybd_event(key.vk, key.sk, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 1);

            //            if (key.shift)
            //            {
            //                keybd_event(160, 42, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 1);
            //            }
            //        }


            //        HandleEvent(((Keys)lParam.wVk).ToString() + " -> " + (data[0].Key).ToString());
            //        return (System.IntPtr)1;
            //    }
            //}


            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll")]
        private static extern bool keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public void Dispose()
        {
            UnhookWindowsHookEx(hookID);
        }

        private static Keys ConvertCharToVirtualKey(char ch)
        {
            short vkey = VkKeyScan(ch);
            Keys retval = (Keys)(vkey & 0xff);
            int modifiers = vkey >> 8;
            if ((modifiers & 1) != 0) retval |= Keys.Shift;
            if ((modifiers & 2) != 0) retval |= Keys.Control;
            if ((modifiers & 4) != 0) retval |= Keys.Alt;
            return retval;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);
    }

    /*
    //[StructLayout(LayoutKind.Sequential)]
    [StructLayout(LayoutKind.Explicit)]
    public struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public int wVk;
        public int wScan;
        public int dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    internal static class ReplaceDefinitions
    {
        public static Dictionary<Keys, HKeyDefinition> Standard = new Dictionary<Keys, HKeyDefinition>()
        {
            { Keys.Z, new HKeyDefinition() { vk = 90, sk = 44, shift = false } },
            { Keys.Y, new HKeyDefinition() { vk = 89, sk = 21, shift = false } },
            { Keys.X, new HKeyDefinition() { vk = 88, sk = 45, shift = false } },
            { Keys.W, new HKeyDefinition() { vk = 87, sk = 17, shift = false } },
            { Keys.V, new HKeyDefinition() { vk = 86, sk = 47, shift = false } },
            { Keys.U, new HKeyDefinition() { vk = 85, sk = 22, shift = false } },
            { Keys.T, new HKeyDefinition() { vk = 84, sk = 20, shift = false } },
            { Keys.S, new HKeyDefinition() { vk = 83, sk = 31, shift = false } },
            { Keys.R, new HKeyDefinition() { vk = 82, sk = 19, shift = false } },
            { Keys.Q, new HKeyDefinition() { vk = 81, sk = 16, shift = false } },
            { Keys.P, new HKeyDefinition() { vk = 80, sk = 25, shift = false } },
            { Keys.O, new HKeyDefinition() { vk = 79, sk = 24, shift = false } },
            { Keys.N, new HKeyDefinition() { vk = 78, sk = 49, shift = false } },
            { Keys.M, new HKeyDefinition() { vk = 77, sk = 50, shift = false } },
            { Keys.L, new HKeyDefinition() { vk = 76, sk = 38, shift = false } },
            { Keys.K, new HKeyDefinition() { vk = 75, sk = 37, shift = false } },
            { Keys.J, new HKeyDefinition() { vk = 74, sk = 36, shift = false } },
            { Keys.I, new HKeyDefinition() { vk = 73, sk = 23, shift = false } },
            { Keys.H, new HKeyDefinition() { vk = 72, sk = 35, shift = false } },
            { Keys.G, new HKeyDefinition() { vk = 71, sk = 34, shift = false } },
            { Keys.F, new HKeyDefinition() { vk = 70, sk = 33, shift = false } },
            { Keys.E, new HKeyDefinition() { vk = 69, sk = 18, shift = false } },
            { Keys.D, new HKeyDefinition() { vk = 68, sk = 32, shift = false } },
            { Keys.C, new HKeyDefinition() { vk = 67, sk = 46, shift = false } },
            { Keys.B, new HKeyDefinition() { vk = 66, sk = 48, shift = false } },
            { Keys.A, new HKeyDefinition() { vk = 65, sk = 30, shift = false } },
        };

        public static Dictionary<Keys, HKeyDefinition[]> Replacements = new Dictionary<Keys, HKeyDefinition[]>()
        {
            //{ Keys.Z, new [] { new HKeyDefinition() { vk = 89, sk = 21, shift = false } } },//z
            //{ Keys.Y, new [] { new HKeyDefinition() { vk = 90, sk = 44, shift = false } } },//y
            //{ Keys.X, new [] { new HKeyDefinition() { vk = 88, sk = 45, shift = false } } },//x
            //{ Keys.W, new [] { new HKeyDefinition() { vk = 87, sk = 17, shift = false } } },//w
            //{ Keys.V, new [] { new HKeyDefinition() { vk = 86, sk = 47, shift = false } } },//v
            //{ Keys.U, new [] { new HKeyDefinition() { vk = 85, sk = 22, shift = false } } },//u
            { Keys.T, new [] { new HKeyDefinition() { vk = 55, sk = 8, shift = false } } },//7 7
            { Keys.S, new [] { new HKeyDefinition() { vk = 52, sk = 5, shift = true } } },//$ s4
            //{ Keys.R, new [] { new HKeyDefinition() { vk = 82, sk = 19, shift = false } } },//r
            //{ Keys.Q, new [] { new HKeyDefinition() { vk = 81, sk = 16, shift = false } } },//q
            //{ Keys.P, new [] { new HKeyDefinition() { vk = 80, sk = 25, shift = false } } },//p
            { Keys.O, new [] { new HKeyDefinition() { vk = 48, sk = 11, shift = false } } },//0 0
            //{ Keys.N, new [] { new HKeyDefinition() { vk = 78, sk = 49, shift = false } } },//n
            //{ Keys.M, new [] { new HKeyDefinition() { vk = 77, sk = 50, shift = false } } },//m
            { Keys.L, new [] { new HKeyDefinition() { vk = 49, sk = 2, shift = false } } },//1 1
            //{ Keys.K, new [] { new HKeyDefinition() { vk = 75, sk = 37, shift = false } } },//k
            //{ Keys.J, new [] { new HKeyDefinition() { vk = 74, sk = 36, shift = false } } },//j
            { Keys.I, new [] { new HKeyDefinition() { vk = 49, sk = 2, shift = true } } },//! s1
            //{ Keys.H, new [] { new HKeyDefinition() { vk = 72, sk = 35, shift = false } } },//h
            { Keys.G, new [] { new HKeyDefinition() { vk = 54, sk = 7, shift = false } } },//6 6
            //{ Keys.F, new [] { new HKeyDefinition() { vk = 70, sk = 33, shift = false } } },//f
            { Keys.E, new [] { new HKeyDefinition() { vk = 51, sk = 4, shift = false } } },//3 3
            //{ Keys.D, new [] { new HKeyDefinition() { vk = 68, sk = 32, shift = false } } },//d
            //{ Keys.C, new [] { new HKeyDefinition() { vk = 67, sk = 46, shift = false } } },//c
            //{ Keys.B, new [] { new HKeyDefinition() { vk = 66, sk = 48, shift = false } } },//b
            { Keys.A, new [] { new HKeyDefinition() { vk = 50, sk = 3, shift = true } } },//@ s2
        };



        //LShiftKey v160 s42 257 0
        //LShiftKey v160 s42 256 0
        //LControlKey v162 s29 257 0
        //LControlKey v162 s29 256 0
        //LMenu v164 s56 257 0
        //LMenu v164 s56 260 0

        //D0 v48 s11 257 0
        //D9 v57 s10 257 0
        //D8 v56 s9 257 0
        //D7 v55 s8 257 0
        //D6 v54 s7 257 0
        //D5 v53 s6 257 0
        //D4 v52 s5 257 0
        //D3 v51 s4 257 0
        //D2 v50 s3 257 0
        //D1 v49 s2 257 0
    }

    internal class HKeyDefinition
    {
        public Keys Key { get { return (Keys)vk; } }
        public byte vk;
        public byte sk;
        public bool shift;
    }
}
