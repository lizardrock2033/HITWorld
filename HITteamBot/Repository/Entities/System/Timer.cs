using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HITteamBot.Repository.Entities.System
{
    public class EventsTimer
    {
        public string Username { get; set; }
        public string TimerName { get; set; }
        public Timer Timer { get; set; }
    }
}
