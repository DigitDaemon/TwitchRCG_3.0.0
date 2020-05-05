using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Agents
{
    class SkeletonPriest : Abstracts.Monster
    {

        const int baseSpeed = 8;

        static List<string> actionPriority = new List<string>() { "Heal", "MagAttack" };

        public SkeletonPriest(int UnId, int speed)
            : base("Skeleton_Priest_" + UnId, 80, 2, 25, 3, 0, 15, new List<Abstracts.Skill>(), baseSpeed + speed, 2, 30)
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
