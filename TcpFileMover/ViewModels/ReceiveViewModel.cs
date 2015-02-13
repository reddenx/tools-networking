using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpFileMover.Models;

namespace TcpFileMover.ViewModels
{
    public class ReceiveViewModel : BaseViewModel
    {
        private List<string> Events;
        public string EventText
        {
            get { return string.Join("\r\n", Events.ToArray()); }
            set { }
        }

        private int Port;
        public string PortString
        {
            get { return Port.ToString(); }
            set
            {
                int.TryParse(value, out Port);
                base.OnPropertyChanged(() => this.PortString);
            }
        }

        private TcpFileReceiver Receiver;

        public ReceiveViewModel()
        {
            Events = new List<string>();
            Receiver = new TcpFileReceiver(HandleNetworkEvent, (s) => { }, GetNewFileInfo);
        }

        private void HandleNetworkEvent(string message)
        {
            Events.Add(message);
            base.OnPropertyChanged(() => this.EventText);
        }

        public void ReceiveFile()
        {
            Receiver.ReceiveOneFile(Port);
        }

        private FileInfo GetNewFileInfo(string incomingFileName)
        {
            var initialFileInfo = new FileInfo(incomingFileName);

            var saver = new SaveFileDialog();
            saver.FileName = initialFileInfo.Name;
            saver.DefaultExt = initialFileInfo.Extension;
            var result = saver.ShowDialog();
            if (result.HasValue && result.Value)
            {
                return new FileInfo(saver.FileName);
            }
            else
            {
                return null;
            }
        }
    }
}
