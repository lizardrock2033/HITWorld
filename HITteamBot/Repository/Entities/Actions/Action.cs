using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Actions
{
    public class Action
    {
        public string Name { get; set; }
        public ActionType Type { get; set; }
        public short DurationInMinutes { get; set; }
        public List<ActionReward> Rewards { get; set; }
    }

    public class ActionReward
    {
        public ActionRewardType Type { get; set; }
        public long Amount { get; set; }
    }

    public enum ActionType
    {
        Exploring,
        Trading,
        Fight
    }

    public enum ActionRewardType
    {
        Experience,
        Caps,
        Junk,
        Item
    }
}
