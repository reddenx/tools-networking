using SMT.Utilities.InputEvents.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.PhoneRemoveBase.Models
{
    class MessageEnterpreter
    {
        private readonly IKeyboardEventRunner Runner;

        public MessageEnterpreter(IKeyboardEventRunner runner)
        {
            Runner = runner;
        }

        public void HandleMessage(string message)
        {
            message.Split(':');
        }
    }

    static class MessageCommands
    {
        public const string LeftClick = "LCLICK";
        public const string RightClick = "RCLICK";
        public const string LeftDoubleClick = "LDBLLCICK";
        public const string MoveMouse = "MOVE";
        public const string StartLeftDrag = "LDRAG+";
        public const string StopLeftDrag = "LDRAG-";
    }

    static class HardwareCommands
    {
        
    }


    /* message protocol
     * 
     * [COMMAND:DATA,DATA,DATA,DATA...]
     * can only be one command per packet
     * can only be 256 char long
     * 
     * commands include
     * LCLICK
     * RCLICK
     * LDBLCLICK
     * RDBLCLICK
     * 
     */
}
