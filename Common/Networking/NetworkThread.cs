using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using Common.Utilities;

namespace Common.Networking
{
    public class NetworkThread<Sock>
        where Sock:AsyncSocket
    {
        private int _connections;
        private readonly System.Timers.Timer _updater;

        private readonly List<SocketAsyncEventArgs> _sockets;
        private readonly ConcurrentBag<SocketAsyncEventArgs> _newSockets;
        
        public Action OnSocketRemoved { get; set; }

        public NetworkThread()
        {
            _updater = new System.Timers.Timer(500);
            _updater.Elapsed += Update;
            _updater.AutoReset = false;
            
            _newSockets = new ConcurrentBag<SocketAsyncEventArgs>();
            _sockets = new List<SocketAsyncEventArgs>();
        }

        public void AddNewSocket(SocketAsyncEventArgs eventArgs)
        {
            Interlocked.Increment(ref _connections);
            _newSockets.Add(eventArgs);
            Log.Info("Добавили в очередь");
        }

        public void Start() => _updater.Start();
        

        public void Stop() => _updater.Stop();
        

        private void Update(object s, ElapsedEventArgs e)
        {
            AddNewSockets();
            Log.Info(_sockets.Count.ToString());
            
            _sockets.RemoveAll(handler =>
            {
                if (!((Sock)(handler.UserToken)).Update())
                {
                    Interlocked.Decrement(ref _connections);
                    OnSocketRemoved();
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
                _newSockets.TryTake(out SocketAsyncEventArgs eventArgs);
                Sock handler = eventArgs.UserToken as Sock;
                if (handler.Closed)
                    continue;
                _sockets.Add(eventArgs);
            }
        }

        public int GetConnectionsCount() => _connections;
    }
}