using SMT.Networking.Interfaces;
using SMT.Networking.Interfaces.SimpleMessaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMT.Networking.Tcp
{
    public class SimpleTcpDataSender : ISimpleDataSender
    {
        private const int MESSAGE_BLOCK_SIZE = 1024; //not sure if this is the most efficient or not?

        private long CurrentTotalBytes;
        private long BytesSent;
        private long DiffBytes;
        private double DiffSeconds;

        public TransferInfo CurrentTransferInfo
        {
            get
            {
                return new TransferInfo(
                    progress: ((float)BytesSent) / ((float)CurrentTotalBytes),
                    bytesPerSecond: ((float)DiffBytes) / ((float)DiffSeconds));
            }
        }

        private TcpClient Client;
        private Thread SendThread;
        private bool InProgress;

        public SimpleTcpDataSender()
        {
            InProgress = false;
        }

        public bool Connect(string host, int port)
        {
            try
            {
                if (Client == null && !InProgress)
                {
                    Client = new TcpClient();
                    Client.Connect(host, port);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public void StartDataTransfer(Stream inputStream)
        {
            if (Client != null && !InProgress)
            {
                SendThread = new Thread(new ParameterizedThreadStart(TransferLoop));
                SendThread.IsBackground = true;
                SendThread.Start(inputStream);
            }
        }

        private void TransferLoop(object inputStreamBlob)
        {
            InProgress = true;
            Stream inputStream = (Stream)inputStreamBlob;

            CurrentTotalBytes = inputStream.Length;
            var outputStream = Client.GetStream();

            var fileBytes = new byte[MESSAGE_BLOCK_SIZE];
            var bytesRead = 0;

            int diffCounter = 0;
            long lastRecordedBytes = 0;
            DateTime lastRecordTime = DateTime.Now;

            try
            {
                while ((bytesRead = inputStream.Read(fileBytes, 0, MESSAGE_BLOCK_SIZE)) > 0)
                {
                    outputStream.Write(fileBytes, 0, bytesRead);
                    BytesSent += bytesRead;

                    if (diffCounter % 20 == 0)//lets see if a syncronous solution doesn't muck with the results
                    {
                        DiffSeconds = (DateTime.Now - lastRecordTime).TotalSeconds;
                        DiffBytes = BytesSent - lastRecordedBytes;
                        lastRecordedBytes = BytesSent;
                    }
                    ++diffCounter;
                }
            }
            catch
            {
            }
            finally
            {
                Cleanup();
            }
            InProgress = false;
        }

        private void Cleanup()
        {
            //the no fucks given cleanup
            try
            {
                Client.Close();
            }
            catch { }

            Client = null;
        }
    }
}
