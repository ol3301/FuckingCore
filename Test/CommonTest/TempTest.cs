using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace CommonTest
{
    public class TempTest
    {
        [Test]
        public void Test1()
        {
            ExecWithBenchmark(() =>
            {
                List<byte[]> store = new List<byte[]>();

                byte[] buff;

                for (int i = 0; i < 100000; ++i)
                {
                    buff = ArrayPool<byte>.Shared.Rent(4096);
                    //store.Add(buff);
                } 
            });
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