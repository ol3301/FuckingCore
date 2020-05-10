using System;
using System.Net.Sockets;

namespace Common.Networking
{
    public class AsyncSocket
    {
        private Socket handler;
        public bool Closed { get; set; }

        public AsyncSocket()
        {
            Closed = false;
        }

        protected void StartReceiveAsync(SocketAsyncEventArgs eventArgs)
        {
            handler = eventArgs.UserToken as Socket;
            eventArgs.Completed += (o,e) =>ProcessReceiveAsync(e);
            if(!handler.ReceiveAsync(eventArgs))
                ProcessReceiveAsync(eventArgs);
        }

        private void ProcessReceiveAsync(SocketAsyncEventArgs eventArgs)
        {
            if (eventArgs.BytesTransferred <= 0 || eventArgs.SocketError != SocketError.Success)
                CloseSocket();
        }

        public virtual bool Update() => !Closed;

        public virtual void Start(SocketAsyncEventArgs eventArgs)
        {
            
        }


        private void CloseSocket()
        {
            Closed = true;
            try
            {
                handler.Shutdown(SocketShutdown.Send);
            }
            catch (Exception)
            {
                // ignored
            }

            handler.Close();
        }
    }
}