using SMT.Networking.Tcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking.Messages
{
    public class MessageBus
    {
        private readonly ITcpMessenger Output;
        private readonly Dictionary<int, List<byte[]>> PollQueue;
        private readonly Dictionary<int, Action<byte[]>> PushHandlers;
        public List<KeyValuePair<int, byte[]>> UnHandledMessages;

        public MessageBus(ITcpMessenger messenger)
        {
            this.Output = messenger;
            this.PollQueue = new Dictionary<int, List<byte[]>>();
            this.PushHandlers = new Dictionary<int, Action<byte[]>>();
            this.UnHandledMessages = new List<KeyValuePair<int, byte[]>>();

            this.Output.OnMessageReceived += HandleMessage;
        }

        private void HandleMessage(object sender, byte[] message)
        {
            var channel = BitConverter.ToInt32(message, 0);
            var messageBytes = message.Skip(4).ToArray();

            if (PollQueue.ContainsKey(channel))
            {
                PollQueue[channel].Add(messageBytes);
            }
            else if (PushHandlers.ContainsKey(channel))
            {
                PushHandlers[channel](messageBytes);
            }
            else
            {
                UnHandledMessages.Add(new KeyValuePair<int, byte[]>(channel, messageBytes));
            }
        }

        public void SendMessage(int channel, byte[] message)
        {
            Int32 channel32 = (Int32)channel;
            var messageBytes = BitConverter.GetBytes(channel32).Concat(message).ToArray();
            this.Output.Send(messageBytes);
        }

        public void RegisterPollQueue(int channel)
        {
            if (!this.PollQueue.ContainsKey(channel) && !this.PushHandlers.ContainsKey(channel))
            {
                PollQueue.Add(channel, new List<byte[]>());
            }
        }

        public void RegisterPushQueue(int channel, Action<byte[]> messageHandler)
        {
            if (!this.PollQueue.ContainsKey(channel) && !this.PushHandlers.ContainsKey(channel))
            {
                PushHandlers.Add(channel, messageHandler);
            }
        }
    }
}
