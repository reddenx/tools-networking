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
    public class TcpFileSender : TcpFileTransferBase
    {
        public TcpFileSender(Action<string> messageHandler, Action<TcpFileNetworkState> statusChangeHandler, Action<TransferState> transferInfoUpdateHandler)
            : base(messageHandler, statusChangeHandler, transferInfoUpdateHandler)
        { }

        public void SendFile(FileInfo fileInformation, string host, int port)
        {
            if (CurrentState == TcpFileNetworkState.Ready)
            {
                CurrentState = TcpFileNetworkState.Sending;

                NetworkingPipeThread = new Thread(new ParameterizedThreadStart(ProcessAndSendFile));
                NetworkingPipeThread.IsBackground = true;
                NetworkingPipeThread.Start(new SenderContext()
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

                using (var netStream = ShakeHands(context.Host, context.Port, context.FileInformation))
                {
                    using (var fileStream = context.FileInformation.OpenRead())
                    {
                        base.TransferStreamUntilExhausted(fileStream, netStream, context.FileInformation.Length);
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
        *    HELLO      ->
        * <- NAME
        *    [FILENAME] ->
        * <- SIZE
        *    [FILESIZE] ->
        * <- READY
        *    [FILEDATA] ->
        * 
        *     -CLOSE-
        */
        private NetworkStream ShakeHands(string host, int port, FileInfo fileInformation)
        {
            MessageHandler("Connecting");
            var client = new TcpClient();
            client.Connect(host, port);

            MessageHandler("Handshaking");
            var stream = client.GetStream();

            base.SendMessage("HELLO", stream);
            var helloResponse = base.GetResponse(stream);
            if (helloResponse != "NAME")
            {
                throw new IOException("Invalid protocol response:" + helloResponse);
            }

            base.SendMessage(fileInformation.Name, stream);
            var nameResponse = base.GetResponse(stream);
            if (nameResponse != "SIZE")
            {
                throw new IOException("Invalid protocol response:" + helloResponse);
            }

            base.SendMessage(fileInformation.Length.ToString(), stream);
            var sizeResponse = base.GetResponse(stream);
            if (sizeResponse != "READY")
            {
                throw new IOException("Invalid protocol response:" + sizeResponse);
            }

            return stream;
        }

        private class SenderContext
        {
            public FileInfo FileInformation { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
        }
    }
}
