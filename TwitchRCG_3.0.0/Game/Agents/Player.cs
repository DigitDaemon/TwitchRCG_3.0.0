using Game.Abstracts;
using Game.Agent_Dependencies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Agents
{
    public class Player : Agent
    {
        protected Class spec { get; }
        protected Race race { get; } 
        

        public Player(int baseHealth, int strength, int mind, int concentration, int mastery, int spirit, List<Skill> skills, int speed, Class spec, Race race, string playerName)
           : base(playerName, baseHealth, strength, mind, concentration, mastery, spirit, skills, speed)
        {
            this.spec = spec;
            this.race = race;
        }

        public Player(int baseHealth, int strength, int mind, int concentration, int mastery, int spirit, List<Skill> skills, int speed, Class spec, Race race, string playerName, Item item)
           : base(playerName, baseHealth, strength, mind, concentration, mastery, spirit, skills, speed, item)
        {
            this.spec = spec;
            this.race = race;
        }

        public override KeyValuePair<string,string> getAction(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus)
        {
            return this.GetAction(enemyStatus, allyStatus, spec.getActionPriority());
        }

        public void guard()
        {
                Modify(new Modifier("Buff", "defend", "Armour", 2, 2));
                Modify(new Modifier("Buff", "defend", "Warding", 2, 2));
            
        }

        public override void defaultAction()
        {
            guard();
        }

        public override string getDefaultAction()
        {
            return "Guard";
        }

        public override string getType()
        {
            return "Player";
        }
    }
}
