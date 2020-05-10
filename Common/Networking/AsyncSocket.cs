using Common.Utilities;
using Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Common.Networking
{
    public class AsyncSocket
    {
        private Metrics _metrics;
        private readonly Socket _handler;
        private bool _writing;

        private Queue<byte[]> _writingQueue;

        public bool Closed { get; set; }
        public SocketAsyncEventArgs ReadEventArgs { get; set; }
        public SocketAsyncEventArgs SendEventArgs { get; set; }

        public AsyncSocket(Socket handler)
        {
            _metrics = Metrics.Instance();
            _handler = handler;
            Closed = false;
            _writing = false;
            _writingQueue = new Queue<byte[]>();
        }

        protected void StartReceiveAsync()
        {
            if(!_handler.ReceiveAsync(ReadEventArgs))
                ProcessReceiveAsync(null, ReadEventArgs);
        }

        private void ProcessReceiveAsync(object o, SocketAsyncEventArgs eventArgs)
        {
            if (eventArgs.BytesTransferred <= 0 || eventArgs.SocketError != SocketError.Success)
            {
                CloseSocket();
                return;
            }
            _metrics.AddBytes(eventArgs.BytesTransferred);
            StartReceiveAsync();
        }

        protected void QueueBuffer(byte[] sendBuffer)
        {
            _writingQueue.Enqueue(sendBuffer);
            ProcessWrintingQueueAsync();
        }

        private void ProcessWrintingQueueAsync()
        {
            if (_writing)
                return;
            _writing = true;
        }

        private void ProcessSendAsync(object sender, SocketAsyncEventArgs eventArgs)
        {
            if(eventArgs.BytesTransferred <= 0 || eventArgs.SocketError != SocketError.Success)
            {
                CloseSocket();
                return;
            }
        }

        public virtual bool Update() => false;

        public virtual void Start()
        {
            
        }


        private void CloseSocket()
        {
            Closed = true;
            ReleaseEventArgs();
            try
            {
                _handler.Shutdown(SocketShutdown.Send);
            }
            catch (Exception)
            {
                // ignored
            }
            _metrics.AddSocketClose();
            _handler.Close();
        }

        public void SetupEventArgs(SocketAsyncEventArgs readEventArgs, SocketAsyncEventArgs sendEventArgs)
        {
            ReadEventArgs = readEventArgs;
            SendEventArgs = sendEventArgs;
            ReadEventArgs.Completed += ProcessReceiveAsync;
            SendEventArgs.Completed += ProcessSendAsync;
        }

        private void ReleaseEventArgs()
        {
            ReadEventArgs.Completed -= ProcessReceiveAsync;
            SendEventArgs.Completed -= ProcessSendAsync;
        }
    }
}