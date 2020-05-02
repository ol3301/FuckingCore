using System;
using System.Net;

namespace Common.Networking
{
    public enum ReceiveState
    {
        ReceivePrefix,
        Payload
    }

    public class PacketReceiver
    {
        private readonly int _maxMessageSize;
        private ReceiveState _receiveState = ReceiveState.ReceivePrefix;
        private byte[] _headerBuffer = new byte[sizeof(int)];
        private byte[] _packetBuffer;
        private int _bytesReceived;
        
        public PacketReceiver(int maxMessageSize = 2048)
        {
            _maxMessageSize = maxMessageSize;
        }

        public byte[] ReadBuffer(byte[] readBuffer)
        {
            int readPos = 0;
            int bytesToPayLoad=0;
            while ((readBuffer.Length - readPos) > 0)
            {
                if (_receiveState == ReceiveState.ReceivePrefix)
                {
                    bytesToPayLoad = Math.Min((_headerBuffer.Length - _bytesReceived), readBuffer.Length);
                    Array.Copy(readBuffer, readPos, _headerBuffer, _bytesReceived, bytesToPayLoad);
                    readPos += bytesToPayLoad;
                    ReadComplete(bytesToPayLoad);
                }
                else if (_receiveState == ReceiveState.Payload)
                {
                    bytesToPayLoad = Math.Min(_packetBuffer.Length - _bytesReceived, readBuffer.Length);
                    Array.Copy(readBuffer, readPos, _packetBuffer, _bytesReceived, bytesToPayLoad);
                    readPos += bytesToPayLoad;
                    ReadComplete(bytesToPayLoad);
                    if (_receiveState == ReceiveState.ReceivePrefix)
                        return _packetBuffer;
                }
            }
            return null;
        }
        private void ReadComplete(int bytesToPayLoad)
        {
            _bytesReceived += bytesToPayLoad;
            if (_receiveState == ReceiveState.ReceivePrefix)
            {
                if ((_headerBuffer.Length - _bytesReceived) != 0)
                    return;
                int lengthPacket = BitConverter.ToInt32(_headerBuffer, 0);
                if (lengthPacket > _maxMessageSize)
                    throw new ProtocolViolationException($"lengthPacket has been: {lengthPacket}. Max message size: {_maxMessageSize}"); 
                
                _packetBuffer = new byte[lengthPacket];
                _receiveState = ReceiveState.Payload;
                _bytesReceived = 0;
            }
            else if (_receiveState == ReceiveState.Payload)
            {
                if ((_packetBuffer.Length - _bytesReceived) != 0)
                    return;
                _receiveState = ReceiveState.ReceivePrefix;
                _bytesReceived = 0;
            }
        }
    }
    
}