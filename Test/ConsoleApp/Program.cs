using System;
using Common.Networking;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(BitConverter.GetBytes(1).Length);
            
            Console.WriteLine("Hello World!");
        }
    }
}