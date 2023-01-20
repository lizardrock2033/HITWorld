using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters
{
    public class Skills
    {
        public List<ActiveSkill> ActiveSkills { get; set; }
        public List<PassiveSkill> PassiveSkills { get; set; }
    }

    public class ActiveSkill
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public int Cost { get; set; }
        public List<Effect> Effects { get; set; }
        public string Description { get; set; }
    }

    public class PassiveSkill
    {
        public string Name { get; set; }
        public List<Effect> Effects { get; set; }
        public string Description { get; set; }
    }
}
