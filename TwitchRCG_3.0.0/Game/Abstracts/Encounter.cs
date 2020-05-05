using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Abstracts
{
    public abstract class Encounter
    {
        protected List<Game.Agents.Agent> playerList { get; }
        protected List<Game.Agents.Agent> deadPlayers { get; }

        public Encounter(List<Game.Agents.Agent> playerList)
        {
            this.playerList = playerList;
            this.deadPlayers = new List<Game.Agents.Agent>();
        }

        public abstract List<string> nextTurn();

        public abstract void checkCompletion(List<string> messages);

        public abstract string endEncounter();
    }
}
