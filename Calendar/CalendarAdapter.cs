using Calendar.Logging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

namespace Calendar
{
    public interface ICalendarAdapter
    {
        void UpdateEvents();
    }

    public class CalendarAdapter : ICalendarAdapter
    {
        private readonly IAppLogger logger;

        public CalendarAdapter(IAppLogger logger)
        {
            this.logger = logger;
        }

        public void UpdateEvents()
        {
            try
            {
                TryMain();
            }
            catch (Exception e)
            {
                this.logger.Error(e, "Unexpected error occurred moving calendar events.");
            }
        }

        private void TryMain()
        {
            var jsonFile = "dotnet-wrapper-8569d0bfd4b1.json";
            var calendarId = @"<email>";

            string[] scopes = { CalendarService.Scope.Calendar };
            var credential = ResolveCredential(jsonFile, scopes);
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Calendar Helper",
            });

            UpdatePastEventsToCurrentDate(service, calendarId);
        }

        private void UpdatePastEventsToCurrentDate(CalendarService service, string calendarId)
        {
            var eventsRequest = service.Events.List(calendarId);
            eventsRequest.TimeMin = DateTime.Today.AddMonths(-1);
            eventsRequest.TimeMax = DateTime.Today;
            eventsRequest.ShowDeleted = false;
            eventsRequest.SingleEvents = true;
            eventsRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            var events = eventsRequest.Execute();

            var today = DateTime.Today.Date;
            foreach (var eventToUpdate in events.Items)
            {
                eventToUpdate.Start.Date = today.ToString("yyyy-MM-dd");
                eventToUpdate.End.Date = today.AddDays(1).ToString("yyyy-MM-dd");

                this.logger.Information("Moving event {summary}.", eventToUpdate.Summary);

                var updateRequest = service.Events.Update(eventToUpdate, calendarId, eventToUpdate.Id);
                updateRequest.Execute();
            }
        }

        private ServiceAccountCredential ResolveCredential(string jsonFile, string[] scopes)
        {
            using var stream = new FileStream(jsonFile, FileMode.Open, FileAccess.Read);

            var config = Google.Apis.Json.NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(stream);

            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(config.ClientEmail)
                {
                    Scopes = scopes
                }.FromPrivateKey(config.PrivateKey));

            return credential;
        }
    }
}
