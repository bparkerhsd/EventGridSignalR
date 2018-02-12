using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventGridSignalR.Models
{
    public class CustomEvent<T>
    {

        public string Id { get; set; }

        public string EventType { get; set; }

        public string Subject { get; set; }

        public string EventTime { get; set; }

        public T Data { get; set; }

        public CustomEvent()
        {
            
        }
    }
}
