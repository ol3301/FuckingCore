using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Common.Networking;
using Common.Utilities;
using NUnit.Framework;

namespace CommonTest
{
    public class PacketReceiverTest
    {
        private string _message = "Hello new alghorithm";

        [Test]
        public void TestGluePartialMessages2()
        {
            int missingBytes = 4;
            PacketReceiver packetReceiver = new PacketReceiver();
            byte[] messageBuffer1 = MakeMessage("first message");
            byte[] messageBuffer2 = MakeMessage(_message);
            byte[] messageBuffer = new byte[messageBuffer1.Length + messageBuffer2.Length - missingBytes];
            byte[] messageBufferMissigBytes = new byte[missingBytes];
            
            Array.Copy(messageBuffer1, messageBuffer, messageBuffer1.Length);
            Array.Copy(messageBuffer2, 0, messageBuffer, messageBuffer1.Length, messageBuffer2.Length - missingBytes);
            Array.Copy(messageBuffer2, messageBuffer2.Length - missingBytes, messageBufferMissigBytes, 0, missingBytes);

            foreach (var buff in messageBuffer)
            {
                packetReceiver.ReadBuffer(new byte[] {buff}).TryDequeue(out byte[] responseBuffer);
                if(responseBuffer != null)
                    Assert.AreEqual("first message", Encoding.UTF8.GetString(responseBuffer));
            }

            foreach (var buff in messageBufferMissigBytes)
            {
                packetReceiver.ReadBuffer(new byte[] {buff}).TryDequeue(out byte[] responseBuffer);
                if(responseBuffer != null)
                    Assert.AreEqual(_message, Encoding.UTF8.GetString(responseBuffer));
            }
        }

        [Test]
        public void TestGluePartialMessages()
        {
            int missingBytes = 4;
            PacketReceiver packetReceiver = new PacketReceiver();
            byte[] messageBuffer1 = MakeMessage("first message");
            byte[] messageBuffer2 = MakeMessage(_message);
            byte[] messageBuffer = new byte[messageBuffer1.Length + messageBuffer2.Length - missingBytes];
            byte[] messageBufferMissigBytes = new byte[missingBytes];
            
            Array.Copy(messageBuffer1, messageBuffer, messageBuffer1.Length);
            Array.Copy(messageBuffer2, 0, messageBuffer, messageBuffer1.Length, messageBuffer2.Length - missingBytes);
            Array.Copy(messageBuffer2, messageBuffer2.Length - missingBytes, messageBufferMissigBytes, 0, missingBytes);
            
            byte[] responseBuffer = packetReceiver.ReadBuffer(messageBuffer).Dequeue();
            Assert.AreEqual("first message", Encoding.UTF8.GetString(responseBuffer));
            responseBuffer = packetReceiver.ReadBuffer(messageBufferMissigBytes).Dequeue();
            Assert.AreEqual(_message, Encoding.UTF8.GetString(responseBuffer));
        }
        
        [Test]
        public void TestGlueMessages()
        {
            PacketReceiver packetReceiver = new PacketReceiver();
            byte[] messageBuffer1 = MakeMessage("first message");
            byte[] messageBuffer2 = MakeMessage(_message);
            byte[] messageBuffer = new byte[messageBuffer1.Length + messageBuffer2.Length];
            
            Array.Copy(messageBuffer1, messageBuffer, messageBuffer1.Length);
            Array.Copy(messageBuffer2, 0, messageBuffer, messageBuffer1.Length, messageBuffer2.Length);

            Queue<byte[]> buffersQueue  = packetReceiver.ReadBuffer(messageBuffer);
            Assert.AreEqual("first message", Encoding.UTF8.GetString(buffersQueue.Dequeue()));
            Assert.AreEqual(_message, Encoding.UTF8.GetString(buffersQueue.Dequeue()));
        }

        [Test]
        public void TestMaxLengthExceptionAndAddMessage()
        {
            PacketReceiver packetReceiver = new PacketReceiver();

            string bigMess = "";
            for (int i = 0; i < 300; ++i)
                bigMess += _message;

            try
            {
                packetReceiver.ReadBuffer(MakeMessage(bigMess));
            }
            catch (ProtocolViolationException ex)
            {
                Log.Info(ex.Message);
            }

            byte[] responseBuffer = packetReceiver.ReadBuffer(MakeMessage(_message)).Dequeue();
            Assert.AreEqual(_message, Encoding.UTF8.GetString(responseBuffer));
        }

        [Test]
        public void TestMutipleMessages()
        {
            PacketReceiver packetReceiver = new PacketReceiver();

            byte[] responseBuffer = packetReceiver.ReadBuffer(MakeMessage(_message)).Dequeue();
            Assert.AreEqual(_message, Encoding.UTF8.GetString(responseBuffer));

            responseBuffer = packetReceiver.ReadBuffer(MakeMessage("Olegolegolegoleg")).Dequeue();
            Assert.AreEqual("Olegolegolegoleg", Encoding.UTF8.GetString(responseBuffer));
        }

        [Test]
        public void TestMaxMessageSizePartial()
        {
            PacketReceiver packetReceiver = new PacketReceiver(1024);
            string bigStr = "";
            for (int i = 0; i < 500; ++i)
                bigStr += _message;
            byte[] messageBuffer = MakeMessage(bigStr);
            foreach (var buff in messageBuffer)
            {
                try
                {
                    packetReceiver.ReadBuffer(new byte [] { buff });
                }
                catch (ProtocolViolationException ex)
                {
                    Log.Info(ex.Message);
                    Assert.Pass();
                }    
            }
            
            Assert.Fail();
        }

        [Test]
        public void TestMaxMessageSizeFull()
        {
            PacketReceiver packetReceiver = new PacketReceiver(1024);
            string bigStr = "";
            for (int i = 0; i < 500; ++i)
                bigStr += _message;
            byte[] messageBuffer = MakeMessage(bigStr);

            try
            {
                packetReceiver.ReadBuffer(messageBuffer);
            }
            catch (ProtocolViolationException ex)
            {
                Log.Info(ex.Message);
                Assert.Pass();
            }
            Assert.Fail();
        }
        
        [Test]
        public void TestPartialMessage()
        {
            PacketReceiver packetReceiver = new PacketReceiver();
            
            byte[] messageBuffer = MakeMessage(_message);
            string message = "";
            foreach (var buff in messageBuffer)
            {
                packetReceiver.ReadBuffer(new byte[]{ buff }).TryDequeue(out byte[] responseBuffer);
                if (responseBuffer != null)
                    message += Encoding.UTF8.GetString(responseBuffer);   
            }
            Assert.AreEqual(_message, message);
        }

        [Test]
        public void TestFullMessage()
        {
            PacketReceiver packetReceiver = new PacketReceiver();
            byte[] messageBuffer = MakeMessage(_message);
            string message = "";
            byte[] responseBuffer = packetReceiver.ReadBuffer(messageBuffer).Dequeue();
            message += Encoding.UTF8.GetString(responseBuffer);
            Assert.AreEqual(_message, message);
        }

        public byte[] MakeMessage(string message)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            byte[] buffWithHeader = new byte[buff.Length + sizeof(int)];
            Array.Copy(BitConverter.GetBytes(buff.Length), buffWithHeader, sizeof(int));
            Array.Copy(buff, 0, buffWithHeader, sizeof(int), buff.Length);
            return buffWithHeader;
        }
    }
}