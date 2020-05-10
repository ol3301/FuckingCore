using ConsoleTables;
using System;
using System.Diagnostics;
using System.Threading;

namespace Helper
{
    public class Metrics
    {
        private static Metrics metric = new Metrics();

        
        private int _connections = 0;
        private int _connectionsAll = 0;
        private int _bytesCount = 0;
        private int _socketClose=0;
        private int _contexts = 0;
        private int _contextsAll = 0;
        private int _contextsReuse = 0;
        private Timer _worker;
        private ConsoleTable _serverStatistics;
        private ConsoleTable _systemStatistics;
        public Metrics()
        {
            _serverStatistics = new ConsoleTable("АКТИВНЫХ ПОДКЛЮЧЕНИЙ", "ВСЕГО ПОДКЛЮЧЕНИЙ", "ЗАКРЫТО СОКЕТОВ");
            _serverStatistics.AddRow(_connections, _connectionsAll, _socketClose);

            _systemStatistics = new ConsoleTable("АКТИВНЫХ КОНТЕКСТОВ", "ВСЕГО КОНТЕКСТОВ", "ПЕРЕИСПОЛЬЗОВАНО КОНТЕКСТОВ", "ПРИНЯТО БАЙТ");
            _systemStatistics.AddRow(_contexts, _contextsAll, _contextsReuse, _bytesCount);
        }


        public static Metrics Instance() => metric;

        public void StartMonitoring()
        {
            _worker = new Timer((o) =>
            {               
                Console.SetCursorPosition(0,0);
                UpdateServerStatistics();
                UpdateSystemStatictics();
            }, null, 0, 150);
        }
        private void UpdateSystemStatictics()
        {
            _systemStatistics.Rows[0].SetValue(_contexts, 0);
            _systemStatistics.Rows[0].SetValue(_contextsAll, 1);
            _systemStatistics.Rows[0].SetValue(_contextsReuse, 2);
            _systemStatistics.Rows[0].SetValue(_bytesCount, 3);
            _systemStatistics.Write(Format.Alternative);
        }
        private void UpdateServerStatistics()
        {
            _serverStatistics.Rows[0].SetValue(_connections, 0);
            _serverStatistics.Rows[0].SetValue(_connectionsAll, 1);
            _serverStatistics.Rows[0].SetValue(_socketClose, 2);
            _serverStatistics.Write(Format.Alternative);
        }
        public void AddContextActive()
            => Interlocked.Increment(ref _contexts);
        public void AddContextAll()
            => Interlocked.Increment(ref _contextsAll);
        public void RmContextActive()
            => Interlocked.Decrement(ref _contexts);
        public void AddContextReuse()
            => Interlocked.Increment(ref _contextsReuse);
        public void AddSocketClose()
            => Interlocked.Increment(ref _socketClose);
        public void AddConnection()
        {
            Interlocked.Increment(ref _connections);
            Interlocked.Increment(ref _connectionsAll);
        }
        public void RmConnection()
            => Interlocked.Decrement(ref _connections);      
        public void AddBytes(int countBytes) =>
            Interlocked.Exchange(ref _bytesCount, _bytesCount + countBytes);

    }
}