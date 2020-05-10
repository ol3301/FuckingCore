using System;
using System.Diagnostics;
using System.Threading;

namespace Common.Utilities
{
    public class Metric
    {
        private int _numConns=0;
        private int _numBytes = 0;
        private int _numDissconnections = 0;
        private int _numErrInCloseClientSocket;
        private Timer _worker;
        private Process _process = Process.GetCurrentProcess();
        
        public void StartMonitoring()
        {
            _worker = new Timer((o) =>
            {
                Console.Clear();
                Console.WriteLine("->Metrics");
                
                PrintSrvMetric();
                PrintErrorMetric();
            }, null, 0, 200);
        }

        private void PrintErrorMetric()
        {
            Console.WriteLine($"ERROR STATISTICS");
            Console.WriteLine($"{"onClose",-11}|");
            Console.WriteLine($"{_numErrInCloseClientSocket,-11}|");
        }

        private void PrintSrvMetric()
        {
            Console.WriteLine("SERVER STATISTICS");
            Console.WriteLine($"{"ПОДКЛЮЧЕНИЙ",-11}|{"ОТКЛЮЧЕНИЙ",-10}|{"ПРИНЯТО",-7}|{"PROCESSOR",-10}|{"MEMORY", -10}");
            Console.WriteLine($"{_numConns,-11}|{_numDissconnections,-10}|{_numBytes,-7}|{_process.UserProcessorTime,-10}|{GetMemoryUsageInMgb()}");
        }
        private string GetMemoryUsageInMgb()
        {
            return $"{GC.GetTotalMemory(true) / 1024}";
        }
        public void IncrementErrInCloseClientSocket() =>
            Interlocked.Increment(ref _numErrInCloseClientSocket);
        public void IncrementConnections() =>
            Interlocked.Increment(ref _numConns);
        public void IncrementBytes(int numBytes) =>
            Interlocked.Exchange(ref _numBytes, _numBytes+numBytes);
        public void IncrementDissconnetions() =>
            Interlocked.Increment(ref _numDissconnections);
    }
}