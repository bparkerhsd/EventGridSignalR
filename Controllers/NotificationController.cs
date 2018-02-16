using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventGridSignalR.Hubs;
using EventGridSignalR.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace EventGridSignalR.Controllers
{
    [Produces("application/json")]
    [Route("api/Notification")]
    public class NotificationController : Controller
    {
        TelemetryClient client;
        IHubContext<ChatHub> chatHub;

        public NotificationController(IHubContext<ChatHub> chatHub)
        {
            client = new TelemetryClient();
            client.InstrumentationKey = "c4c04e6f-ac5e-49f1-a36f-dd61c4ba16cb";
            this.chatHub = chatHub;
        }

        public async void Post([FromBody]List<CustomEvent<SampleEventData>> value)
        {
            try
            {
                client.TrackEvent($"NotificationController:Post");

                int count = 0;
                foreach (var eventValue in value)
                {
                    string data = JsonConvert.SerializeObject(eventValue);
                    Dictionary<string, string> allData = new Dictionary<string, string>();
                    allData.Add("event id", eventValue.Id);
                    allData.Add("eventdata", data);

                    client.TrackEvent($"NotificationController:Post - Event Data", allData);
                    await chatHub.Clients.All.InvokeAsync("Send", data);
                    client.TrackEvent($"NotificationController:Post - Completed Clients.All.InvokeAsync", allData);
                    count++;

                }
            }
            catch (Exception ex)
            {
                client.TrackEvent("Exception Occurred. Check Insight Exceptions.");
                client.TrackException(ex);
            }
        }
    }
}

