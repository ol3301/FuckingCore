using System.Net.Sockets;
using Common.Utilities;
using Common.Utilities.Pool;

namespace Common.Networking
{
    public abstract class SocketManager<TSocketImplementation> 
        where TSocketImplementation:AsyncSocket, new()
    {
        protected int ThreadsCount { get; set; }
        private NetworkThread<TSocketImplementation>[] _networkThreads;
        private readonly SocketAsyncEventArgsPool _eventArgsPool;

        public SocketManager()
        {
            _eventArgsPool = new SocketAsyncEventArgsPool();
        }

        protected void BaseStartNetwork(int threadsCount)
        {
            ThreadsCount = threadsCount;
            _networkThreads = CreateNetworkThreads();

            foreach (var thread in _networkThreads)
                thread.Start();
        }

        protected virtual void OnSocketAccepted(Socket handler)
        {
            int threadIdx = SelectThreadWithMinConnections();
            SocketAsyncEventArgs e = GetSocketAsyncEventArgs(handler);
            
            TSocketImplementation socket = new TSocketImplementation();
            socket.Start(e);
            
            _networkThreads[threadIdx].AddNewSocket(e);
        }

        protected virtual void OnSocketRemoved(Socket handler)
        {
            Log.Info("OnSocketRemoved");
        }
        private SocketAsyncEventArgs GetSocketAsyncEventArgs(Socket handler)
        {
            SocketAsyncEventArgs e = _eventArgsPool.Pop();
            e.UserToken = handler;
            return e;
        }

        private int SelectThreadWithMinConnections()
        {
            int min = 0;
            for(int i=0;i<ThreadsCount; ++i)
                if (_networkThreads[i].GetConnectionsCount() < _networkThreads[min].GetConnectionsCount())
                    min = i;
            return min;
        }

        protected abstract NetworkThread<TSocketImplementation>[] CreateNetworkThreads();
    }
}