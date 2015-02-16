using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpFileMover.Models
{
    abstract public class TcpFileTransferBase
    {
        protected Action<string> MessageHandler;
        protected Action<TcpFileNetworkState> StatusChangeHandler;
        protected Action<TransferState> TransferInfoUpdateHandler;

        protected Thread NetworkingPipeThread;
        protected Thread SpeedUpdateThread;

        protected TcpFileNetworkState _currentState;
        protected TcpFileNetworkState CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                StatusChangeHandler(_currentState);
            }
        }

        protected int TotalMovedBytes;

        protected TcpFileTransferBase(Action<string> messageHandler, Action<TcpFileNetworkState> statusChangeHandler, Action<TransferState> transferInfoUpdateHandler)
        {
            MessageHandler = messageHandler;
            StatusChangeHandler = statusChangeHandler;
            TransferInfoUpdateHandler = transferInfoUpdateHandler;
            CurrentState = TcpFileNetworkState.Ready;
        }

        protected void TransferStreamUntilExhausted(Stream inputStream, Stream outputStream, long totalLength)
        {
            SpeedUpdateThread = new Thread(new ParameterizedThreadStart(SpeedLoop));
            SpeedUpdateThread.IsBackground = true;
            SpeedUpdateThread.Start(totalLength);

            MessageHandler("Transferring data");

            TotalMovedBytes = 0;
            var fileBytes = new byte[1024];
            var bytesRead = 0;
            
            while ((bytesRead = inputStream.Read(fileBytes, 0, 1024)) > 0)
            {
                outputStream.Write(fileBytes, 0, bytesRead);
                TotalMovedBytes += bytesRead;
            }

            MessageHandler("Transfer complete");
        }

        private void SpeedLoop(object fileSizeBlob)
        {
            var fileSize = (long)fileSizeBlob;
            int lastTotalBytesCheck = 0;
            DateTime lastCheckIn = DateTime.Now;

            while (CurrentState == TcpFileNetworkState.Receiving || CurrentState == TcpFileNetworkState.Sending)
            {
                var totalBytes = TotalMovedBytes;
                var sentBytes = totalBytes - lastTotalBytesCheck;
                lastTotalBytesCheck = totalBytes;

                var now = DateTime.Now;
                var elapsedTime = now - lastCheckIn;
                lastCheckIn = now;

                TransferInfoUpdateHandler(new TransferState()
                {
                    BytesPerSecond = (float)sentBytes / (float)elapsedTime.TotalSeconds,
                    Progress = (float)lastTotalBytesCheck / (float)fileSize,
                });

                Thread.Sleep(1000);
            }
        }

        protected string GetResponse(Stream stream)
        {
            var responseBytes = new byte[1024];
            var responseLength = stream.Read(responseBytes, 0, 1024);
            return ASCIIEncoding.ASCII.GetString(responseBytes, 0, responseLength);
        }

        protected void SendMessage(string message, Stream stream)
        {
            var msgBytes = ASCIIEncoding.ASCII.GetBytes(message);
            stream.Write(msgBytes, 0, msgBytes.Length);
        }
    }
}
