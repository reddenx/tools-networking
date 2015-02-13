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
    public class TcpFileReceiver
    {
        private Action<string> MessageHandler;
        private Action<TcpFileNetworkState> StatusChangeHandler;
        private Func<string, FileInfo> HandleFileReceived;

        private Thread NetworkingAndReceivingThread;

        private TcpFileNetworkState _currentState;
        private TcpFileNetworkState CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                StatusChangeHandler(_currentState);
            }
        }


        public TcpFileReceiver(Action<string> messageHandler, Action<TcpFileNetworkState> statusChangeHandler, Func<string, FileInfo> handleFileReceived)
        {
            _currentState = TcpFileNetworkState.Ready;
            MessageHandler = messageHandler;
            StatusChangeHandler = statusChangeHandler;
            HandleFileReceived = handleFileReceived;
        }

        public void ReceiveOneFile(int port)
        {
            if (CurrentState == TcpFileNetworkState.Ready)
            {
                CurrentState = TcpFileNetworkState.Receiving;

                NetworkingAndReceivingThread = new Thread(new ParameterizedThreadStart(ProcessAndReceiveFile));
                NetworkingAndReceivingThread.IsBackground = true;
                NetworkingAndReceivingThread.Start(port);
            }
        }

        private void ProcessAndReceiveFile(object portBlob)
        {
            var port = (int)portBlob;
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            MessageHandler("Waiting for connection");

            var client = listener.AcceptTcpClient();
            listener.Stop();
            MessageHandler("Client connected");

            var netStream = client.GetStream();

            var fileStream = Handshake(netStream);
            ReceiveFile(fileStream, netStream);

            fileStream.Close();

            client.Close();

            CurrentState = TcpFileNetworkState.Ready;
        }

        private void ReceiveFile(FileStream fileStream, NetworkStream netStream)
        {
            MessageHandler("ReceivingFile");

            var fileBytes = new byte[1024];
            var bytesRead = 0;

            while ((bytesRead = netStream.Read(fileBytes, 0, 1024)) > 0)
            {
                fileStream.Write(fileBytes, 0, bytesRead);
            }

            MessageHandler("File Received");
        }

        private FileStream Handshake(NetworkStream netStream)
        {
            MessageHandler("Handshaking");

            var firstMessage = GetResponse(netStream);
            if (firstMessage != "HELLO")
            {
                throw new IOException("Invalid protocol response:" + firstMessage);
            }

            SendMessage("ACK", netStream);
            var fileName = GetResponse(netStream);
            var fileInfo = HandleFileReceived(fileName);

            if (fileInfo == null || fileInfo.Exists)
            {
                SendMessage("NOGO", netStream);
                throw new IOException("Cannot send that");
            }

            SendMessage("READY", netStream);
            return fileInfo.Create();
        }

        private string GetResponse(NetworkStream stream)
        {
            var responseBytes = new byte[1024];
            var responseLength = stream.Read(responseBytes, 0, 1024);
            return ASCIIEncoding.ASCII.GetString(responseBytes, 0, responseLength);
        }

        private void SendMessage(string message, NetworkStream stream)
        {
            var msgBytes = ASCIIEncoding.ASCII.GetBytes(message);
            stream.Write(msgBytes, 0, msgBytes.Length);
        }

    }
}
