using Game.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;
using Game.Agents;

namespace Game.Skills
{
    class PowerAttack : Skill
    {
        static string name = "PowerAttack";

        public KeyValuePair<string, string> getCondition(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<KeyValuePair<string, string>> selfStatus)
        {
            return GetCondition(enemyStatus, allyStatus, selfStatus);
        }

        static public KeyValuePair<string, string> GetCondition(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<KeyValuePair<string, string>> selfStatus)
        {
            if (!selfStatus.Exists(x => x.Value.Equals("Critical")))
            {
                if (enemyStatus.Exists(x => x.Value.Equals("Taunting")))
                    return new KeyValuePair<string, string>(enemyStatus.Find(x => x.Value.Equals("Taunting")).Key + " Monster", GetName() + " Skill");
                foreach (KeyValuePair<string, string> enemyCon in enemyStatus)
                {
                    if (enemyCon.Value.Equals("Critical") || enemyCon.Value.Equals("Vulnerable"))
                        return new KeyValuePair<string, string>(enemyCon.Key + " Monster", GetName() + " Skill");
                }
                foreach (KeyValuePair<string, string> enemyCon in enemyStatus)
                {
                    if (enemyCon.Value.Equals("Hurt"))
                        return new KeyValuePair<string, string>(enemyCon.Key + " Monster", GetName() + " Skill");
                }
                foreach (KeyValuePair<string, string> enemyCon in enemyStatus)
                {
                    if (enemyCon.Value.Equals("Healthy"))
                        return new KeyValuePair<string, string>(enemyCon.Key + " Monster", GetName() + " Skill");
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
            var outstring = agent.GetName() + " is using a Power Attack on " + target.GetName() + ".";

            agent.Modify(new Agent_Dependencies.Modifier("Buff", "attack", "Strength", 4, 1));
            agent.Modify(new Agent_Dependencies.Modifier("Debuff", "attack", "Mastery", -2, 1));
            target.TakeDamage(agent.PhysAttack(), "Physical", null);

            return outstring;
        }
    }
}
