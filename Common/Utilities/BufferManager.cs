using System.Collections.Generic;
using System.Net.Sockets;

namespace Common.Utilities
{
    public class BufferManager
    {
        private readonly int _totalBytes;                
        private readonly byte[] _buffer;
        private readonly Stack<int> _freeIndexPool;    
        private int _currentIndex;
        private int _bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            _totalBytes = totalBytes;
            _currentIndex = 0;
            _bufferSize = bufferSize;
            _freeIndexPool = new Stack<int>();
            _buffer = new byte[_totalBytes];
        }
        
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (_freeIndexPool.Count > 0)
            {
                args.SetBuffer(_buffer, _freeIndexPool.Pop(), _bufferSize);
                return true;
            }
            if ((_totalBytes - _bufferSize) < _currentIndex)
                return false;
            args.SetBuffer(_buffer, _currentIndex, _bufferSize);
            _currentIndex += _bufferSize;
            return true;
        }

        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            args.SetBuffer(null, 0, 0);
            _freeIndexPool.Push(args.Offset);
        }
    }
}