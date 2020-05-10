using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using Common.Networking;
using Common.Utilities;
using Helper;

namespace ConsoleApp
{
    class Program
    {
        public static Action<string> test { get; set; }
        static void Main(string[] args)
        {
            NetworkManager networkManager = new NetworkManager();
            networkManager.StartNetwork(1);

            Metrics.Instance().StartMonitoring();

            Console.ReadLine();
        }

   
        private static void ExecWithBenchmark(Action action)
        {
            Stopwatch s = Stopwatch.StartNew();
            action();
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds);
        }
    }
}