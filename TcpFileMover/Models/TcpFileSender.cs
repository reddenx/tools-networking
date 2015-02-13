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
    public class TcpFileSender
    {
        private Action<string> MessageHandler;
        private Action<TcpFileNetworkState> StatusChangeHandler;

        private Thread NetworkingAndSendingThread;

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

        public TcpFileSender(Action<string> messageHandler, Action<TcpFileNetworkState> statusChangeHandler)
        {
            MessageHandler = messageHandler;
            StatusChangeHandler = statusChangeHandler;
            CurrentState = TcpFileNetworkState.Ready;
        }

        public void SendFile(FileInfo fileInformation, string host, int port)
        {
            if (CurrentState == TcpFileNetworkState.Ready)
            {
                CurrentState = TcpFileNetworkState.Sending;
                NetworkingAndSendingThread = new Thread(new ParameterizedThreadStart(ProcessAndSendFile));
                NetworkingAndSendingThread.IsBackground = true;
                NetworkingAndSendingThread.Start(new SenderContext()
                {
                    FileInformation = fileInformation,
                    Host = host,
                    Port = port,
                });
            }
        }

        private void ProcessAndSendFile(object contextBlob)
        {
            try
            {
                var context = (SenderContext)contextBlob;

                var client = new TcpClient();
                var stream = ShakeHands(context.Host, context.Port, client, context.FileInformation);
                SendFile(context.FileInformation, stream);

                stream.Close();
            }
            catch (Exception e)
            {
                MessageHandler("Error:" + e.Message);
            }

            CurrentState = TcpFileNetworkState.Ready;
        }

        private void SendFile(FileInfo fileInfo, NetworkStream netStream)
        {

            MessageHandler("SendingFile");

            var fileStream = fileInfo.OpenRead();
            var fileBytes = new byte[1024];
            var bytesRead = 0;

            while ((bytesRead = fileStream.Read(fileBytes, 0, 1024)) > 0)
            {
                netStream.Write(fileBytes, 0, bytesRead);
            }

            MessageHandler("FileSent");

        }

        private NetworkStream ShakeHands(string host, int port, TcpClient client, FileInfo fileInformation)
        {
            MessageHandler("Connecting");
            client.Connect(host, port);

            MessageHandler("Handshaking");
            var stream = client.GetStream();

            var helloResponse = SendWithResponse("HELLO", stream);
            if (helloResponse != "ACK")
            {
                throw new IOException("Invalid protocol response:" + helloResponse);
            }

            var nameResponse = SendWithResponse(fileInformation.Name, stream);
            if (nameResponse != "READY")
            {
                throw new IOException("Invalid protocol response:" + helloResponse);
            }

            return stream;
        }

        private string SendWithResponse(string message, NetworkStream stream)
        {
            var msgBytes = ASCIIEncoding.ASCII.GetBytes(message);
            stream.Write(msgBytes, 0, msgBytes.Length);

            var responseBytes = new byte[1024];
            var responseLength = stream.Read(responseBytes, 0, 1024);
            return ASCIIEncoding.ASCII.GetString(responseBytes, 0, responseLength);
        }

        private class SenderContext
        {
            public FileInfo FileInformation { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
        }
    }
}
