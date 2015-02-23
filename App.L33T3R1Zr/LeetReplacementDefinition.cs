using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App.L33T3R1Zr
{
    class LeetReplacementDefinition
    {
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

        public class HKeyDefinition
        {
            public Keys Key { get { return (Keys)vk; } }
            public byte vk;
            public byte sk;
            public bool shift;
        }
    }
}
