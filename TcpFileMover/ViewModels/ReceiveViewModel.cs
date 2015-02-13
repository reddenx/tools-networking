using Microsoft.Win32;
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

        private TcpFileNetworkState ReceiverState;

        public Visibility ProgressVisibility
        {
            get
            {
                switch (ReceiverState)
                {
                    case TcpFileNetworkState.Receiving:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }

        public ReceiveViewModel()
        {
            Events = new List<string>();
            Receiver = new TcpFileReceiver(HandleNetworkEvent, HandleReceiveStateChange, HandleTransferUpdate, GetNewFileInfo);
            Port = 37015;
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

        private void HandleReceiveStateChange(TcpFileNetworkState newState)
        {
            ReceiverState = newState;
            base.OnPropertyChanged(() => this.ProgressVisibility);
        }

        private void HandleTransferUpdate(TransferState transferState)
        {
            TransferRate = transferState.BytesPerSecond.ToString();
            TransferProgress = transferState.Progress;
        }
    }
}
