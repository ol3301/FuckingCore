using System;
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
        public void TestMutipleMessages()
        {
            PacketReceiver packetReceiver = new PacketReceiver();

            byte[] responseBuffer = packetReceiver.ReadBuffer(MakeMessage(_message));
            Assert.AreEqual(_message, Encoding.UTF8.GetString(responseBuffer));

            responseBuffer = packetReceiver.ReadBuffer(MakeMessage("Olegolegolegoleg"));
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
                byte[] responseBuffer = packetReceiver.ReadBuffer(new byte[]{ buff });
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
            byte[] responseBuffer = packetReceiver.ReadBuffer(messageBuffer);
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