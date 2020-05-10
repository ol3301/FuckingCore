using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using Common.Utilities.Pool;
using Helper;

namespace Common.Networking
{
    public class NetworkThread
    {
        private int _connections;
        private readonly System.Timers.Timer _updater;
        private Metrics _metrics;

        private readonly List<NetworkSocket> _sockets;
        private readonly ConcurrentBag<NetworkSocket> _newSockets;
        private readonly SocketAsyncEventArgsPool _eventArgsPool;

        public NetworkThread()
        {
            _updater = new System.Timers.Timer(10);
            _updater.Elapsed += Update;
            _updater.AutoReset = false;
            
            _newSockets = new ConcurrentBag<NetworkSocket>();
            _sockets = new List<NetworkSocket>();
            _eventArgsPool = new SocketAsyncEventArgsPool();
            _metrics = Metrics.Instance();
        }

        public void AddNewSocketAndAllocateEventArgs(NetworkSocket handler)
        {
            _metrics.AddConnection();
            Interlocked.Increment(ref _connections);
            handler.SetupEventArgs(GenerateEventArgs(), GenerateEventArgs());
            _newSockets.Add(handler);
        }

        public void Start() => _updater.Start();       
        public void Stop() => _updater.Stop();

        private SocketAsyncEventArgs GenerateEventArgs()
            => _eventArgsPool.Pop();
        private void FreeEventArgs(NetworkSocket handler)
        {
            _eventArgsPool.Push(handler.ReadEventArgs);
            _eventArgsPool.Push(handler.SendEventArgs);
        }
        private void Update(object s, ElapsedEventArgs e)
        {
            AddNewSockets();
            
            _sockets.RemoveAll(handler =>
            {
                if (!handler.Update())
                {
                    Interlocked.Decrement(ref _connections);
                    FreeEventArgs(handler);
                    _metrics.RmConnection();
                    return true;
                }
                return false;
            });

            _updater.Start();
        }

        private void AddNewSockets()
        {
            while (!_newSockets.IsEmpty)
            {
                _newSockets.TryTake(out NetworkSocket handler);
                if (handler.Closed)
                    continue;
                _sockets.Add(handler);
            }
        }

        public int GetConnectionsCount() => _connections;
    }
}