using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HITteamBot.Repository.Controllers
{
    public class ActionsController
    {
        public static Timer SetTimer(TimerCallback timerCallback, object state, int dueTime, int period)
        {
            return new Timer(timerCallback, state, dueTime, period);
        }

        public static void Event(Object state)
        {
            Console.WriteLine(state.ToString());
        }
    }
}
