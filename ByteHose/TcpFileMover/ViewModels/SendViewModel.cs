using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private float _transferProgress;
        public float TransferProgress
        {
            get { return _transferProgress; }
            set
            {
                _transferProgress = value;
                base.OnPropertyChanged(() => this.TransferProgress);
            }
        }

        public float _transferRate;
        public string TransferRate
        {
            get 
            {
                var suffix = "Bytes/s";
                var modifiedRate = _transferRate;

                if (modifiedRate > 1024)
                {
                    suffix = "kBytes/s";
                    modifiedRate /= 1024;
                }
                if (modifiedRate > 1024)
                {
                    suffix = "MBytes/s";
                    modifiedRate /= 1024;
                }

                return string.Format("{0} {1}", modifiedRate, suffix);
            }
            set
            {
                if (float.TryParse(value, out _transferRate))
                {
                    base.OnPropertyChanged(() => this.TransferRate);
                }
            }
        }

        private TcpFileNetworkState SenderState;

        public Visibility ProgressVisibility 
        {
            get
            {
                switch (SenderState)
                {
                    case TcpFileNetworkState.Sending:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }

        private TcpFileSender Sender;

        public SendViewModel()
        {
            Events = new List<string>();
            Sender = new TcpFileSender(HandleNetworkEvent, HandleSendStateChange, HandleTransferUpdate);
            Port = 37015;
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
            SenderState = newState;
            base.OnPropertyChanged(() => this.ProgressVisibility);
        }

        private void HandleTransferUpdate(TransferState transferState)
        {
            TransferRate = transferState.BytesPerSecond.ToString();
            TransferProgress = transferState.Progress;
        }
    }
}
