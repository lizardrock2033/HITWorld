using HITteamBot.Repository.Domain.DatabaseEntities.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters
{
    public class CharacterData
    {
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public long CharacterId { get; set; }
        public string CharacterName { get; set; }
        public short SkillPoints { get; set; }
        public SPECIAL SPECIAL { get; set; }
        public SPECIAL CurrentSPECIAL { get; set; }
    }
}
