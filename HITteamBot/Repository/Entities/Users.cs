using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities
{
    public class Users
    {
        public long Id { get; set; }
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; }
        public long CharacterId { get; set; }
    }
}
