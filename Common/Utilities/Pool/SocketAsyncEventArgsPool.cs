using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Common.Utilities.Pool
{
    public class SocketAsyncEventArgsPool
    {
        private const int BufferSize = 4096;
        private readonly ConcurrentBag<SocketAsyncEventArgs> _socketAsyncEventArgses;

        public long Count => _socketAsyncEventArgses.Count;

        public SocketAsyncEventArgsPool()
        {
            _socketAsyncEventArgses = new ConcurrentBag<SocketAsyncEventArgs>();
        }

        public void Push(SocketAsyncEventArgs socketAsyncEventArgs) =>
            _socketAsyncEventArgses.Add(socketAsyncEventArgs);

        public SocketAsyncEventArgs Pop() =>
            _socketAsyncEventArgses.TryTake(out SocketAsyncEventArgs socketAsyncEventArgs)
                ? socketAsyncEventArgs
                : GenerateNew();
                
        private SocketAsyncEventArgs GenerateNew()
        {
            SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
            SetBuffer(socketAsyncEventArgs);
            return socketAsyncEventArgs;
        }

        private void SetBuffer(SocketAsyncEventArgs e) 
            => e.SetBuffer(new byte[BufferSize],0,BufferSize);
    }
}