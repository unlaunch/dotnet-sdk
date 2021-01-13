#if NETSTANDARD
using System;
using Microsoft.Extensions.Logging;

namespace io.unlaunch
{
    class MsLogger : IUnlaunchLogger
    {
        private static readonly ILoggerFactory LoggerFactory = new LoggerFactory();
        private readonly ILogger _logger;

        public MsLogger(Type type)
        {
            _logger = LoggerFactory.CreateLogger(type);
        }

        public void Debug(string message)
        {
            _logger.LogDebug(message);
        }

        public void Debug(string message, Exception e)
        {
            _logger.LogDebug(message, e);
        }

        public void Info(string message)
        {
            _logger.LogInformation(message);
        }

        public void Info(string message, Exception e)
        {
            _logger.LogInformation(message, e);
        }

        public void Trace(string message)
        {
            _logger.LogTrace(message);
        }

        public void Trace(string message, Exception e)
        {
            _logger.LogTrace(message, e);
        }

        public void Warn(string message)
        {
            _logger.LogWarning(message);
        }

        public void Warn(string message, Exception e)
        {
            _logger.LogWarning(message, e);
        }

        public void Error(string message)
        {
            _logger.LogError(message);
        }

        public void Error(string message, Exception e)
        {
            _logger.LogError(message, e);
        }
    }
}

#endif