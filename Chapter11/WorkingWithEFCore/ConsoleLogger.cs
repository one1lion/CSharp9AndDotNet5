using Microsoft.Extensions.Logging;
using System;
using static System.Console;

namespace Packt.Shared
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger();
        }

        // if your logger uses unmanaged resources,
        // you can release the memory here
        public void Dispose() {}
    }

    public class ConsoleLogger : ILogger
    {
        // if your logger uses unmanaged resources, you can
        // return the class that implements IDisposable here
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // to avoid overlogging, you can filter on the log level
            switch(logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Information:
                case LogLevel.None:
                    return false;
                case LogLevel.Debug:
                case LogLevel.Warning:
                case LogLevel.Error:
                case LogLevel.Critical:
                default:
                    return true;
            };
        }

        //TState = Object, stupid naming. 
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Log the level and event identifier
            Write($"Level: {logLevel}, Event ID: {eventId.Id}");
            if (eventId.Id == 20100)
            {
                WriteLine(formatter.ToString());
                // log the level and event identifier
                Write($"Level: {logLevel}, Event ID: : {eventId.Id}, Event: {eventId.Name}");
                // only output the state or exception if it exist
                if (state != null)
                {
                    Write($", State: {state}");
                }

                if (exception != null)
                {
                    Write($", Exception: {exception.Message}");
                }
                WriteLine();
            }

            // only output the state or exception if it exist
            if (state != null)
            {
                Write($", State: {state}");
            }

            if (exception != null)
            {
                Write($", Exception: {exception.Message}");
            }
            WriteLine();
        }
    }
}