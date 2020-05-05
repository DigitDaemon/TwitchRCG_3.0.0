using System;
using System.Collections.Generic;
using System.Text;
using Game.Agents;

namespace Game.Abstracts
{
    public interface Skill
    {
        string getSkill();

        KeyValuePair<string, string> getCondition(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus, List<KeyValuePair<string, string>> selfStatus);

        string getName();

        string useSkill(Agent target, Agent agent);
       
    }
}
