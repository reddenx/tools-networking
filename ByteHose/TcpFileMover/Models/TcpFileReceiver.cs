using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpFileMover.Models
{
    public class TcpFileReceiver : TcpFileTransferBase
    {
        private Func<string, FileInfo> HandleFileReceived;

        public TcpFileReceiver(Action<string> messageHandler, Action<TcpFileNetworkState> statusChangeHandler, Action<TransferState> transferInfoUpdateHandler, Func<string, FileInfo> handleFileReceived)
            : base(messageHandler, statusChangeHandler, transferInfoUpdateHandler)
        {
            _currentState = TcpFileNetworkState.Ready;
            HandleFileReceived = handleFileReceived;
        }

        public void ReceiveOneFile(int port)
        {
            if (CurrentState == TcpFileNetworkState.Ready)
            {
                CurrentState = TcpFileNetworkState.Receiving;

                NetworkingPipeThread = new Thread(new ParameterizedThreadStart(ProcessAndReceiveFile));
                NetworkingPipeThread.IsBackground = true;
                NetworkingPipeThread.Start(port);
            }
        }

        private void ProcessAndReceiveFile(object portBlob)
        {
            try
            {
                var port = (int)portBlob;
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                MessageHandler("Waiting for connection");

                var client = listener.AcceptTcpClient();
                listener.Stop();
                MessageHandler("Client connected");

                using (var netStream = client.GetStream())
                {
                    var handShakeInfo = Handshake(netStream);
                    using (var fileStream = handShakeInfo.FileStream)
                    {
                        var fileSize = handShakeInfo.FileSize;
                        TransferStreamUntilExhausted(netStream, fileStream, fileSize);
                    }
                }
            }
            catch (Exception e)
            {
                MessageHandler("Error:" + e.Message);
            }

            CurrentState = TcpFileNetworkState.Ready;
        }

        /* Handshake overview
         * HELLO ->
         * <- NAME
         * [FILENAME] ->
         * <- SIZE
         * [FILESIZE] ->
         * <- READY
         * [FILEDATA] ->
         * 
         *   -CLOSE-
         */
        private HandshakeResponse Handshake(NetworkStream netStream)
        {
            MessageHandler("Handshaking");

            var firstMessage = GetResponse(netStream);
            if (firstMessage != "HELLO")
            {
                throw new IOException("Invalid protocol response:" + firstMessage);
            }

            SendMessage("NAME", netStream);
            var fileName = GetResponse(netStream);
            var fileInfo = HandleFileReceived(fileName);
            if (fileInfo == null || fileInfo.Exists)
            {
                SendMessage("NOGO", netStream);
                throw new IOException("cannot overwrite a file");
            }

            SendMessage("SIZE", netStream);
            var fileSizeMsg = GetResponse(netStream);
            long fileSize;
            if (!long.TryParse(fileSizeMsg, out fileSize))
            {
                SendMessage("NOGO", netStream);
                throw new IOException("cannot overwrite a file");
            }

            var fileStream = fileInfo.Create();
            SendMessage("READY", netStream);

            return new HandshakeResponse()
            {
                FileSize = fileSize,
                FileStream = fileStream,
            };
        }

        private class HandshakeResponse
        {
            public FileStream FileStream { get; set; }
            public long FileSize { get; set; }
        }
    }
}
