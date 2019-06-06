using System;
using System.Threading;

namespace SIS.MvcFramework.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
            => Console.WriteLine($"[{DateTime.Now:dd-MM-yyy HH:mm:ss}] [#{Thread.CurrentThread.ManagedThreadId}] {message}");
    }
}