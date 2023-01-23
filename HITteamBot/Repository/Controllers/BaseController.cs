using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HITteamBot.Repository.Controllers
{
    public class BaseController
    {
        public static Timer SetTimer(TimerCallback timerCallback, object state, int dueTime, int period)
        {
            return new Timer(timerCallback, state, dueTime, period);
        }
    }
}
