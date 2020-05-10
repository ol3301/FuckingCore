using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using Common.Networking;
using Common.Utilities;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkManager networkManager = new NetworkManager();
            networkManager.StartNetwork(1);
            
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