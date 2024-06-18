

namespace SMT.Networking
{
    /// <summary>
    /// this interface is used by the network enterpret and write data to their underlying connections
    /// </summary>
    /// <typeparam name="T">Message Type</typeparam>
    public interface INetworkConnectionSerializer<T>
    {
        byte[] Serialize(T message);
        T Deserialize(byte[] data);
    }
}
