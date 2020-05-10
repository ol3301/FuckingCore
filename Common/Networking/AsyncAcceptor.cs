using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common.Utilities;
using Common.Utilities.Pool;

namespace Common.Networking
{
    public class AsyncAcceptor
    {
        private readonly int _maxConnections;
        private readonly Socket _listener;
        private readonly IPEndPoint _localEndpoint;
        
        private Action<Socket> _onAccepted;
        public AsyncAcceptor(int port, int maxConnection)
        {
            _maxConnections = maxConnection;
            _localEndpoint = new IPEndPoint(IPAddress.Any,  port);
            _listener = new Socket(_localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        
        public void AcceptAsync(Action<Socket> onAccepted)
        {
            _onAccepted = onAccepted;
            _listener.Bind(_localEndpoint); 
            _listener.Listen(_maxConnections);
            StartAccept(GenerateAcceptSocketAsyncEventArgs());
        }
        
        private SocketAsyncEventArgs GenerateAcceptSocketAsyncEventArgs()
        {
            SocketAsyncEventArgs accept = new SocketAsyncEventArgs();
            accept.Completed += (o,e) => ProcessAccept(e);
            return accept;
        }
        
        private void StartAccept(SocketAsyncEventArgs eventArgs)
        {
            eventArgs.AcceptSocket = null;
            if(!_listener.AcceptAsync(eventArgs))
                ProcessAccept(eventArgs);
        }

        private void ProcessAccept(SocketAsyncEventArgs eventArgs)
        {
            _onAccepted(eventArgs.AcceptSocket);
            StartAccept(eventArgs);
        }
    }
}