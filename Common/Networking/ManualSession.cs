namespace Common.Networking
{
    public class SocketSession
    {
        private readonly AsyncAcceptor _serverSocket;
        private readonly PacketReceiver _packetReceiver;
        
        public SocketSession(AsyncAcceptor asyncAcceptor)
        {
            _serverSocket = asyncAcceptor;
            _packetReceiver = new PacketReceiver();
        }
    }
}