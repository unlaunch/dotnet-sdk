#if NET45
using System;
using Common.Logging;

namespace io.unlaunch
{
    class CommonLogger : IUnlaunchLogger
    {
        private readonly ILog _logger;

        public CommonLogger(Type type)
        {
            _logger = LogManager.GetLogger(type);
        }
        
        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(string message, Exception e)
        {
            _logger.Debug(message, e);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(string message, Exception e)
        {
            _logger.Info(message, e);
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Trace(string message, Exception e)
        {
            _logger.Trace(message, e);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Warn(string message, Exception e)
        {
            _logger.Warn(message, e);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception e)
        {
            _logger.Error(message, e);
        }
    }
}
#endif
