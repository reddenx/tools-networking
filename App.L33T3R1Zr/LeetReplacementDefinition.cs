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
        

        public class HKeyDefinition
        {
            public Keys Key { get { return (Keys)vk; } }
            public byte vk;
            public byte sk;
            public bool shift;

            public HKeyDefinition Shift()
            {
                return new HKeyDefinition() { vk = this.vk, sk = this.sk, shift = true };
            }

            public HKeyDefinition UnShift()
            {
                return new HKeyDefinition() { vk = this.vk, sk = this.sk, shift = false };
            }
        }

        public static Dictionary<char, HKeyDefinition> HKeyDefinitions = new Dictionary<char, HKeyDefinition>()
        {
            { 'z', new HKeyDefinition() { vk = 89, sk = 21} },
            { 'y', new HKeyDefinition() { vk = 90, sk = 44} },
            { 'x', new HKeyDefinition() { vk = 88, sk = 45} },
            { 'w', new HKeyDefinition() { vk = 87, sk = 17} },
            { 'v', new HKeyDefinition() { vk = 86, sk = 47} },
            { 'u', new HKeyDefinition() { vk = 85, sk = 22} },
            { 't', new HKeyDefinition() { vk = 84, sk = 20} },
            { 's', new HKeyDefinition() { vk = 83, sk = 31} },
            { 'r', new HKeyDefinition() { vk = 82, sk = 19} },
            { 'q', new HKeyDefinition() { vk = 81, sk = 16} },
            { 'p', new HKeyDefinition() { vk = 80, sk = 25} },
            { 'o', new HKeyDefinition() { vk = 79, sk = 24} },
            { 'n', new HKeyDefinition() { vk = 78, sk = 49} },
            { 'm', new HKeyDefinition() { vk = 77, sk = 50} },
            { 'l', new HKeyDefinition() { vk = 76, sk = 38} },
            { 'k', new HKeyDefinition() { vk = 75, sk = 37} },
            { 'j', new HKeyDefinition() { vk = 74, sk = 36} },
            { 'i', new HKeyDefinition() { vk = 73, sk = 23} },
            { 'h', new HKeyDefinition() { vk = 72, sk = 35} },
            { 'g', new HKeyDefinition() { vk = 71, sk = 34} },
            { 'f', new HKeyDefinition() { vk = 70, sk = 33} },
            { 'e', new HKeyDefinition() { vk = 69, sk = 18} },
            { 'd', new HKeyDefinition() { vk = 68, sk = 32} },
            { 'c', new HKeyDefinition() { vk = 67, sk = 46} },
            { 'b', new HKeyDefinition() { vk = 66, sk = 48} },
            { 'a', new HKeyDefinition() { vk = 65, sk = 30} },

            { '1', new HKeyDefinition() { vk = 49, sk = 2 } },
            { '2', new HKeyDefinition() { vk = 50, sk = 3 } },
            { '3', new HKeyDefinition() { vk = 51, sk = 4 } },
            { '4', new HKeyDefinition() { vk = 52, sk = 5 } },
            { '5', new HKeyDefinition() { vk = 53, sk = 6 } },
            { '6', new HKeyDefinition() { vk = 54, sk = 7 } },
            { '7', new HKeyDefinition() { vk = 55, sk = 8 } },
            { '8', new HKeyDefinition() { vk = 56, sk = 9 } },
            { '9', new HKeyDefinition() { vk = 57, sk = 10} },
            { '0', new HKeyDefinition() { vk = 48, sk = 1 } },

            {  '-', new HKeyDefinition() { vk = 189, sk = 12 } },
            {  '=', new HKeyDefinition() { vk = 187, sk = 13 } },
            {  '[', new HKeyDefinition() { vk = 212, sk = 26 } },
            {  ']', new HKeyDefinition() { vk = 221, sk = 27 } },
            { '\\', new HKeyDefinition() { vk = 220, sk = 43 } },
            {  ';', new HKeyDefinition() { vk = 186, sk = 39 } },
            { '\'', new HKeyDefinition() { vk = 222, sk = 40 } },
            {  ',', new HKeyDefinition() { vk = 188, sk = 51 } },
            {  '.', new HKeyDefinition() { vk = 190, sk = 52 } },
            {  '/', new HKeyDefinition() { vk = 191, sk = 53 } },

            { '`', new HKeyDefinition() { vk = 192, sk = 41 } },

            //{ 'ctrl',  new HKeyDefinition() { vk = 162, sk = 29 } },
            //{ 'alt',   new HKeyDefinition() { vk = 164, sk = 56 } },
            //{ 'shift', new HKeyDefinition() { vk = 160, sk = 42 } },
        };

        public static Dictionary<Keys, HKeyDefinition[]> Replacements = new Dictionary<Keys, HKeyDefinition[]>()
        {
            { Keys.A, new [] { HKeyDefinitions['4'] } },
            { Keys.E, new [] { HKeyDefinitions['3'] } },
            { Keys.I, new [] { HKeyDefinitions['1'] } },
            { Keys.O, new [] { HKeyDefinitions['0'] } },
            { Keys.U, new [] { HKeyDefinitions['9'].Shift(), HKeyDefinitions['-'].Shift(), HKeyDefinitions['0'].Shift() } },
            { Keys.T, new [] { HKeyDefinitions['7'] } },
        };
    }
}
