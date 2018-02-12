using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventGridSignalR.Hubs
{
    public class ChatHub : Hub
    {
        public Task Send(string message)
        {
            var client = new TelemetryClient();
            client.InstrumentationKey = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
            client.TrackEvent($"ChatHub Send received. {message}");
            
            return Clients.All.InvokeAsync("Send", message);
        }
    }
}
