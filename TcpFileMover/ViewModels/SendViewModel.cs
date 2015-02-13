using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpFileMover.Models;

namespace TcpFileMover.ViewModels
{
    public class SendViewModel : BaseViewModel
    {
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                base.OnPropertyChanged(() => this.FilePath);
            }
        }

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

        public string DestinationString { get; set; }

        private TcpFileSender Sender;

        public SendViewModel()
        {
            Events = new List<string>();
            Sender = new TcpFileSender(HandleNetworkEvent, HandleSendStateChange);
        }

        private void HandleNetworkEvent(string message)
        {
            Events.Add(message);
            base.OnPropertyChanged(() => this.EventText);
        }

        public void SendFile()
        {
            var fileInfo = new FileInfo(FilePath);
            Sender.SendFile(fileInfo, DestinationString, Port);
        }

        private void HandleSendStateChange(TcpFileNetworkState newState)
        {
            //do nothing?
        }
    }
}
