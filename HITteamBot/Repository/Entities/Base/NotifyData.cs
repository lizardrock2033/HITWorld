using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HITteamBot.Repository.Entities.Base
{
    public class NotifyData
    {
        public EventsTimer Timer { get; set; }
        public ITelegramBotClient BotClient { get; set; }
        public Chat Chat { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public NotifiedBy NotifiedBy { get; set; }
    }

    public enum NotifiedBy
    {
        None,
        Action
    }
}
