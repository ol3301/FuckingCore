using Helper;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Common.Utilities.Pool
{
    public class SocketAsyncEventArgsPool
    {
        private const int BufferSize = 4096;
        private readonly ConcurrentBag<SocketAsyncEventArgs> _socketAsyncEventArgses;
        private readonly Metrics _metrics;

        public long Count => _socketAsyncEventArgses.Count;

        public SocketAsyncEventArgsPool()
        {
            _metrics = Metrics.Instance();
            _socketAsyncEventArgses = new ConcurrentBag<SocketAsyncEventArgs>();
        }

        public void Push(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            _metrics.RmContextActive();
            _socketAsyncEventArgses.Add(socketAsyncEventArgs);
        }

        public SocketAsyncEventArgs Pop()
        {
            _socketAsyncEventArgses.TryTake(out SocketAsyncEventArgs socketAsyncEventArgs);
            if (socketAsyncEventArgs != null)
            {
                _metrics.AddContextReuse();
                _metrics.AddContextActive();
                return socketAsyncEventArgs;
            }
            return GenerateNewEventArgs();
        }
                
        private SocketAsyncEventArgs GenerateNewEventArgs()
        {
            _metrics.AddContextActive();
            _metrics.AddContextAll();
            SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
            SetBuffer(socketAsyncEventArgs);
            return socketAsyncEventArgs;
        }

        private void SetBuffer(SocketAsyncEventArgs e) 
            => e.SetBuffer(new byte[BufferSize],0,BufferSize);
    }
}