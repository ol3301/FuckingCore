using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Common.Utilities;

namespace Common.Networking
{
    public class AsyncAcceptor
    {
        private readonly Socket _listener;
        
        public AsyncAcceptor(int port)
        {
            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Any,  port);
            _listener = new Socket(localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(localEndpoint); 
        }

        public void Listen()
        {
            _listener.Listen(int.MaxValue);
            _listener.BeginAccept(AcceptCallback, _listener);
        }

        private void CloseSocket(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Log.Info($"Ошибка при попытке закрыть подключение", ex);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket newSocket = null;
            try
            {
                Socket listener = (Socket) ar.AsyncState;
                newSocket = listener.EndAccept(ar);
                
                
            }
            catch (Exception ex)
            {
                Log.Info("Ошибка при подключении нового сокета", ex);
                CloseSocket(newSocket);
            }
        }
    }
}