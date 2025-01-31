using Calendar.Logging;
using Serilog;

namespace Calendar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var seriLogger = (ILogger)new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            IAppLogger logger = new AppLogger(seriLogger);

            ICalendarAdapter calendarAdapter = new CalendarAdapter(logger);

            logger.Information("Starting calendar tool.");

            calendarAdapter.UpdateEvents();

            logger.Information("Shutting down calendar tool.");
        }
    }
}