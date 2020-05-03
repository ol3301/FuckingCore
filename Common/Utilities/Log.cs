using System;

namespace Common.Utilities
{
    public static class Log
    {
        public static void Info(string message)
        {
            Console.WriteLine(message);
        }
        
        public static void Info(string message, Exception ex)
        {
            Info($"{message}:");
            Info(ex.Message);
        }
    }
}