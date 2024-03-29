﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HITteamBot.Repository.Domain.DatabaseEntities.Characters
{
    public class Character
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public int Experience { get; set; }
        public short Level { get; set; }
        public long SPECIALsId { get; set; }
        public int Health { get; set; }
        public int CurrentHealth { get; set; }
        public int ActionPoints { get; set; }
        public int CurrentAP { get; set; }
        public short Rads { get; set; }
        public long Caps { get; set; }
        public bool IsAlive { get; set; }
    }
}
