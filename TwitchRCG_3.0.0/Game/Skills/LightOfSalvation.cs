using Game.Abstracts;
using Game.Agent_Dependencies;
using System;
using System.Collections.Generic;
using System.Text;
using Game.Agents;

namespace Game.Skills
{
    class LightOfSalvation : Skill
    {
        static string name = "LightofSalvation";

        public KeyValuePair<string, string> getCondition(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<KeyValuePair<string, string>> selfStatus)
        {
            return GetCondition(enemyStatus, allyStatus, selfStatus);
        }

        static public KeyValuePair<string, string> GetCondition(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<KeyValuePair<string, string>> selfStatus)
        {
            if (!selfStatus.Exists(x => x.Value.Equals("Taunting")))
            {
               
                foreach (KeyValuePair<string, string> status in allyStatus)
                {
                    if (status.Value.Equals("Critical"))
                        return new KeyValuePair<string, string>(status.Key + " Player", GetName() + " Skill");
                }
                
            }

            return new KeyValuePair<string, string>("false", "");
        }

        public string getName()
        {
            return GetName();
        }

        static public string GetName()
        {
            return name;
        }

        public string getSkill()
        {
            throw new NotImplementedException();
        }

        public string useSkill(Agent target, Agent agent)
        {
            return UseSkill(target, agent);
        }

        static public string UseSkill(Agent target, Agent agent)
        {
            var outstring = agent.GetName() + " is using Light of Salvation to heal " + target.GetName() + ".";

            target.GetHealed(agent.Heal(), new List<Modifier>() { new Modifier("Hot", "start", "Health", 5, 4)});
            agent.Modify(new Agent_Dependencies.Modifier("Debuff", "status", 3, "Taunting"));

            return outstring;
        }
    }
}
