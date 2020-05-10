using System.Net.Sockets;
using Common.Utilities;
using Common.Utilities.Pool;

namespace Common.Networking
{
    public abstract class SocketManager
    {
        protected int ThreadsCount { get; set; }
        private NetworkThread[] _networkThreads;

        public SocketManager()
        {
            
        }

        protected void BaseStartNetwork(int threadsCount)
        {
            ThreadsCount = threadsCount;
            _networkThreads = CreateNetworkThreads();

            foreach (var thread in _networkThreads)
                thread.Start();
        }

        protected void OnSocketAcceptedBase(NetworkSocket handler)
        {
            int threadIdx = SelectThreadWithMinConnections();
            _networkThreads[threadIdx].AddNewSocketAndAllocateEventArgs(handler);
            handler.Start();
        }
        protected abstract NetworkThread[] CreateNetworkThreads();
        private int SelectThreadWithMinConnections()
        {
            int min = 0;
            for (int i = 0; i < ThreadsCount; ++i)
                if (_networkThreads[i].GetConnectionsCount() < _networkThreads[min].GetConnectionsCount())
                    min = i;
            return min;
        }
    }
}