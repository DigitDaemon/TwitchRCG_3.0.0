using System;
using System.Collections.Generic;
using System.Text;
using Game.Agents;

namespace Game
{
    public class SpeedComparer : IComparer<Game.Agents.Agent>
    {
        public int Compare(Agent x, Agent y)
        {
            if (x.GetSpeed() >= y.GetSpeed())
                return 1;
            else
                return -1;
        }
    }
}
