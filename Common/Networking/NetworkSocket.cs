using System.ComponentModel;
using System.Net.Sockets;

namespace Common.Networking
{
    public class NetworkSocket : AsyncSocket
    {
        public override void Start(SocketAsyncEventArgs eventArgs)
        {
            StartReceiveAsync(eventArgs);
        }
    }
}