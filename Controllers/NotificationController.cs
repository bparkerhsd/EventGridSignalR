using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventGridSignalR.Hubs;
using EventGridSignalR.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace EventGridSignalR.Controllers
{
    [Produces("application/json")]
    [Route("api/Notification")]
    public class NotificationController : Controller
    {
        TelemetryClient client;

        public NotificationController()
        {
            client = new TelemetryClient();
            client.InstrumentationKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
        }

        public async void Post([FromBody]List<CustomEvent<SampleEventData>> value)
        {
            try
            {
                client.TrackEvent($"Post message received. {value.Count} event(s) received.");

                int count = 0;
                foreach (var eventValue in value)
                {
                    string data = JsonConvert.SerializeObject(eventValue);
                    Dictionary<string, string> allData = new Dictionary<string, string>();
                    allData.Add("event id", eventValue.Id);
                    allData.Add("eventdata", data);

                    client.TrackEvent($"Event Data", allData);
                    await SendEvent($"received an event grid message: {eventValue.Data.Name}");
                    count++;

                }
            }
            catch(Exception ex)
            {
                client.TrackEvent("Exception Occurred. Check Insight Exceptions.");
                client.TrackException(ex);
            }
        }
        private async Task SendEvent(string message)
        {
            try
            {
                client.TrackEvent("Start SendEvent");
                var connection = new HubConnectionBuilder()
                    .WithUrl("https://xxxxxxxxxxxxxxxx.azurewebsites.net/chat")
                    .WithTransport(Microsoft.AspNetCore.Sockets.TransportType.WebSockets)
                    .Build();
                
                connection.On<string>("Send", data =>
                {
                    Console.WriteLine($"Received: {data}");
                });

                var exceptions = new List<Exception>();

                for (int attempted = 0; attempted < 10; attempted++)
                {
                    try
                    {
                        if (attempted > 0)
                        {
                            System.Threading.Thread.Sleep(3000);
                        }
                        await connection.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }

                if (exceptions.Count == 0)
                    await connection.InvokeAsync("Send", message);
                else
                    client.TrackEvent($"Could not start connection: {exceptions[0].Message}. Attempts to connect: {exceptions.Count}");


            }
            catch (Exception ex)
            {
                client.TrackEvent($"SendEvent - Exception Occurred. Check Insight Exceptions. {ex.Message}");
                client.TrackException(ex);
            }

        }
    }
}

