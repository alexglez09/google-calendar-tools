using Serilog;

namespace Calendar.Logging
{
    public interface IAppLogger
    {
        void Information(string message, params object[] args);

        void Warning(string message, params object[] args);

        void Warning(Exception exception, string message, params object[] args);

        void Error(string message, params object[] args);

        void Error(Exception exception, string message, params object[] args);
    }

    public class AppLogger : IAppLogger
    {
        private readonly ILogger logger;

        public AppLogger(ILogger logger)
        {
            this.logger = logger;
        }

        public void Information(string message, params object[] args)
        {
            this.logger.Information(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            this.logger.Warning(message, args);
        }

        public void Warning(Exception exception, string message, params object[] args)
        {
            this.logger.Warning(exception, message, args);
        }

        public void Error(string message, params object[] args)
        {
            this.logger.Error(message, args);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            this.logger.Error(exception, message, args);
        }
    }
}