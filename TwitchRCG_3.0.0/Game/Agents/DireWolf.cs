using Game.Agent_Dependencies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Agents
{
    public class DireWolf : Abstracts.Monster
    {
        const int baseSpeed = 12;
        
        static List<string> actionPriority = new List<string>(){ "PhysAttack" };

        public DireWolf(int UnId, int speed)
            : base("Dire_Wolf_"+UnId,120,30,0,0,1,0,new List<Abstracts.Skill>(),baseSpeed + speed, 3, 35)
        {
        }

        static int getBaseSpeed()
        {
            return baseSpeed;
        }

        public override void defaultAction()
        {
            
        }

        public override KeyValuePair<string, string> getAction(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus)
        {
            return this.getAction(enemyStatus, allyStatus, actionPriority);
        }

        public override string getDefaultAction()
        {
            return "Wait";
        }

        
    }
}
