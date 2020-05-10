using System;
using System.Net.Sockets;

namespace Common.Networking
{
    public class NetworkManager : SocketManager
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

        protected void OnSocketAccepted(Socket handler)
        {
            NetworkSocket socket = new NetworkSocket(handler);
            OnSocketAcceptedBase(socket);
        }

        protected override NetworkThread[] CreateNetworkThreads()
        {
            NetworkThread[] threads = new NetworkThread[ThreadsCount];
            for (int i = 0; i < ThreadsCount; i++)           
                threads[i] = new NetworkThread();          
            return threads;
        }
    }
}