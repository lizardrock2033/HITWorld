using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Base
{
    public class Permissions
    {
        public string Username { get; set; }
        public PermissionsType Type { get; set; }
    }

    public enum PermissionsType
    {
        Player,
        Moderator,
        Administrator
    }
}
