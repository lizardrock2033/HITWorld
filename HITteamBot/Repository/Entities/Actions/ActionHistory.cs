using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Actions
{
    public class ActionHistory
    {
        public string Username { get; set; }
        public ActionType ActionType { get; set; }
        public string ActionName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public List<ActionReward> Rewards { get; set; }
        public ActionConsequences Consequences { get; set; }
    }
}
