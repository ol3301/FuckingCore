using System.ComponentModel;
using System.Net.Sockets;

namespace Common.Networking
{
    public class NetworkSocket : AsyncSocket
    {
        public NetworkSocket(Socket handler) : base(handler) { }
        public override void Start()
        {
            StartReceiveAsync();
        }

        public override bool Update()
        {
            return !Closed;
        }
    }
}