using System;

namespace Iris.Logging
{
    public interface ILogger
    {
        void Log(string message);
        void LogError(string message);
    }

    public class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.Out.WriteLine($"APP LOG::{Environment.MachineName}::{message}");
        }

        public void LogError(string message)
        {
            Console.Error.WriteLine($"APP LOG::{Environment.MachineName}::{message}");
        }
    }
}
