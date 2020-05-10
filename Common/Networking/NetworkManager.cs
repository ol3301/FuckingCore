using System;
using System.Net.Sockets;

namespace Common.Networking
{
    public class NetworkManager : SocketManager<NetworkSocket>
    {
        private readonly AsyncAcceptor _acceptor;
        
        public NetworkManager()
        {
            _acceptor = new AsyncAcceptor(12333, 1000);
        }

        public void StartNetwork(int threadsCount)
        {
            BaseStartNetwork(threadsCount);
            _acceptor.AcceptAsync(OnSocketAccepted);
        }

        protected override NetworkThread<NetworkSocket>[] CreateNetworkThreads()
        {
            NetworkThread<NetworkSocket>[] threads = new NetworkThread<NetworkSocket>[ThreadsCount];
            for (int i = 0; i < ThreadsCount; i++)
                threads[i] = new NetworkThread<NetworkSocket>();
            return threads;
        }
    }
}