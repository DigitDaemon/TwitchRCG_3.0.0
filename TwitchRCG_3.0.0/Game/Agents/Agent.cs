using Game.Agent_Dependencies;
using Game.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Game.Abstracts;

namespace Game.Agents
{
    public class Agent
    {
        static protected Random rng = new Random();
        protected string Name { get; }
        protected string Type { get; }
        protected int BaseHealth { get; }
        protected int CurrentHealth { get; set; }
        protected int BaseStrength { get; }
        protected int BaseMind { get; }
        protected int BaseConcentration { get; }
        protected int BaseMastery { get; }
        protected int BaseArmorValue { get; }
        protected int BaseWarding { get; }
        protected int BaseSpirit { get; }
        protected int Speed { get; }
        protected string DefaultActionName { get; }
        protected int Exp { get; }  

        private List<Modifier> ModsList { get; set; }
        protected List<Skill> Skills { get; }
        protected Item Item;
        protected List<string> ActionPriority;
        

        public Agent(string name, int baseHealth, int strength, int mind, int concentration, int mastery, int spirit, int exp,
            List<Skill> skills, int speed, Item item, List<string> actionPriority, string Type)
        {
            this.Name = name;
            this.BaseHealth = baseHealth;
            CurrentHealth = baseHealth;
            this.BaseStrength = strength;
            this.BaseMastery = mastery;
            this.BaseMind = mind;
            this.BaseConcentration = concentration;
            this.BaseSpirit = spirit;
            this.Skills = skills;
            this.Speed = speed;
            this.Item = item;
            ModsList = new List<Modifier>();
            this.Exp = exp;
            this.ActionPriority = actionPriority;
            this.Type = Type;
             
        }

        public int GetExp()
        {
            return Exp;
        }

        public string GetName()
        {
            return Name;
        }

        public void TakeDamage(int amount, string type, List<Modifier> effects)
        {
            var currentArmour = BaseArmorValue;
            var currentWarding = BaseWarding;
            foreach (Modifier mod in ModsList)
            {
                if (mod.defend())
                {
                    if (mod.target.Equals("Armour"))
                        currentArmour += mod.value;
                    else if (mod.target.Equals("Warding"))
                        currentWarding += mod.value;
                    else if (mod.target.Equals("Health"))
                        CurrentHealth += mod.value;
                }
            }
            if (type.Equals("Physical"))
            {
                CurrentHealth -= amount * (int)((100 - currentArmour) / 100f);
            }
            else if (type.Equals("Magical"))
            {
                CurrentHealth -= amount * (int)((100 - currentWarding) / 100f);
            }

            if (effects != null)
            {
                foreach (Modifier mod in effects)
                {
                    ModsList.Add(mod);
                }
            }

            CheckDeath();
        }

        public void CheckDeath()
        {
            if (CurrentHealth <= 0)
            {
                foreach(Modifier mod in ModsList)
                {
                    if (mod.onDeath())
                    {
                        if (mod.target.Equals("Health"))
                            CurrentHealth += mod.value;
                    }
                    
                }
                if (CurrentHealth <= 0)
                    throw new DeathException(Name + " has died.", this);
            }
        }

        public void Upkeep()
        {
            foreach (Modifier mod in ModsList)
            {
                if (mod.turnStart())
                {
                    if (mod.target.Equals("Health"))
                    {
                        CurrentHealth += mod.value;
                    }
                }

            }

            CheckDeath();
        }

        public void Endstep()
        {
            foreach (Modifier mod in ModsList)
            {
                if (mod.turnEnd())
                {
                    if (mod.target.Equals("Health"))
                    {
                        CurrentHealth += mod.value;
                    }
                }
                mod.duration--;
                if (mod.duration >= 0)
                {
                    ModsList.RemoveAt(ModsList.IndexOf(mod));
                }
            }

            CheckDeath();
        }

        public int PhysAttack()
        {
            var currentStrength = BaseStrength;
            var currentMastery = BaseMastery;
            foreach(Modifier mod in ModsList)
            {
                if (mod.attack())
                {
                    if (mod.target.Equals("Strength"))
                    {
                        currentStrength += mod.value;
                    }
                    else if (mod.target.Equals("Mastery"))
                    {
                        currentMastery += mod.value;
                    }
                }
            }
            var attackBase = currentStrength - (int)((float)currentStrength * (1f / (2f + (float)currentMastery)));
            Console.WriteLine("attackBase" + attackBase);
            var attackRange = 2 * (int)((float)currentStrength * (1f / (2f + (float)currentMastery)));
            Console.WriteLine("attackRange" + attackRange);
            var damage = attackBase + rng.Next(attackRange);
            Console.WriteLine("damage" + damage);
            return damage;
        }

        public int MagicAttack()
        {
            var currentMind = BaseMind;
            var currentConcentration = BaseConcentration;
            foreach (Modifier mod in ModsList)
            {
                if (mod.attack())
                {
                    if (mod.target.Equals("Mind"))
                    {
                        currentMind += mod.value;
                    }
                    else if (mod.target.Equals("Concentration"))
                    {
                        currentConcentration += mod.value;
                    }
                }
            }
            var attackBase = currentMind - (int)((float)currentMind * (1f / (2f + (float)currentConcentration)));
            var attackRange = 2 * (int)((float)currentMind * (1f / (2f + (float)currentConcentration)));
            var damage = attackBase + rng.Next(attackRange);
            return damage;
        }

        public int GetCurrentHealth()
        {
            return CurrentHealth;
        }

        public List<KeyValuePair<string, string>> GetStatus()
        {

            var statusList = new List<KeyValuePair<string, string>>();

            foreach (Modifier mod in ModsList)
            {
                if (mod.getStatus())
                {
                    statusList.Add(new KeyValuePair<string, string>(Name, mod.status));
                }
            }

            if (CurrentHealth > (float)(BaseHealth * 66))
            {
                statusList.Add(new KeyValuePair<string,string>(Name,"Healthy"));
            }
            else if (CurrentHealth < (float)(BaseHealth * 33))
            {
                statusList.Add(new KeyValuePair<string, string>(Name, "Critical"));
            }
            else
            {
                statusList.Add(new KeyValuePair<string, string>(Name, "Hurt"));
            }

            var debuffs = 0;
            var dots = 0;
            if (ModsList.Count > 0)
            {
                foreach (Modifier mod in ModsList)
                {
                    if (mod.type.Equals("Debuff"))
                        debuffs++;
                    if (mod.type.Equals("Dot"))
                        dots++;
                }
            }

            if(debuffs > 2)
            {
                statusList.Add(new KeyValuePair<string, string>(Name, "Heavily Debuffed"));
            }else if (debuffs > 0)
            {
                statusList.Add(new KeyValuePair<string, string>(Name, "Debuffed"));
            }

            if (dots > 2)
            {
                statusList.Add(new KeyValuePair<string, string>(Name, "Heavily Dotted"));
            }
            else if (dots > 0)
            {
                statusList.Add(new KeyValuePair<string, string>(Name, "Dotted"));
            }


            return statusList;

        }

        public KeyValuePair<string, string> GetAction(List<KeyValuePair<string, string>> enemyStatus, List<KeyValuePair<string, string>> allyStatus)
        {
            foreach (string action in ActionPriority)
            {
                Console.WriteLine(action);

                if (action.Equals("Item"))
                {
                    if (Item != null)
                    {
                        foreach (KeyValuePair<string, string> status in this.GetStatus())
                        {
                            if (Item.getCondition().Equals(status.Value))
                            {
                                return new KeyValuePair<string, string>(Name, "UsedItem");
                            }

                        }
                    }
                }
                else if (action.Equals("Skill"))
                {
                        foreach (Skill skill in Skills)
                        {
                            Console.WriteLine(skill.getName());
                            var skillcon = skill.getCondition(enemyStatus, allyStatus, GetStatus());
                        Console.WriteLine(skillcon.Key + " " + skillcon.Value);
                            if (!skillcon.Key.Equals("false"))
                            {
                                return skillcon;
                            }
                                 
                        }
                    
                }
                else if (action.Equals("Heal"))
                {
                    foreach (KeyValuePair<string, string> status in allyStatus)
                    {
                        if (status.Value.Equals("Critical"))
                            return new KeyValuePair<string, string>(status.Key, "Heal");
                    }
                    foreach (KeyValuePair<string, string> status in GetStatus())
                    {
                        if (status.Value.Equals("Critical"))
                            return new KeyValuePair<string, string>(status.Key, "Heal");
                    }
                    foreach (KeyValuePair<string, string> status in allyStatus)
                    {
                        if (status.Value.Equals("Hurt"))
                            return new KeyValuePair<string, string>(status.Key, "Heal");
                    }
                    foreach (KeyValuePair<string, string> status in GetStatus())
                    {
                        if (status.Value.Equals("Hurt"))
                            return new KeyValuePair<string, string>(status.Key, "Heal");
                    }
                }
                else if (action.Equals("PhysAttack"))
                {
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Taunting"))
                            return new KeyValuePair<string, string>(status.Key, "PhysAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Vulnerable") || status.Value.Equals("Critical"))
                            return new KeyValuePair<string, string>(status.Key, "PhysAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Hurt"))
                            return new KeyValuePair<string, string>(status.Key, "PhysAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Healthy"))
                            return new KeyValuePair<string, string>(status.Key, "PhysAttack");
                    }
                }
                else if (action.Equals("MagAttack"))
                {
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Taunting"))
                            return new KeyValuePair<string, string>(status.Key, "MagAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Vulnerable") || status.Value.Equals("Critical"))
                            return new KeyValuePair<string, string>(status.Key, "MagAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Hurt"))
                            return new KeyValuePair<string, string>(status.Key, "MagAttack");
                    }
                    foreach (KeyValuePair<string, string> status in enemyStatus)
                    {
                        if (status.Value.Equals("Healthy"))
                            return new KeyValuePair<string, string>(status.Key, "MagAttack");
                    }
                }
                else if (action.Equals("Guard"))
                {
                    return new KeyValuePair<string, string>(Name, "Guard");
                }
            }
            throw new NullReferenceException("The action selection phase was not able to return a value.");
        }
        
        public void Modify(Modifier mod)
        {
            ModsList.Add(mod);
        }
        
        public int GetSpeed()
        {
            return Speed;
        }

        public string GetAgentType() {
            return Type;
        }
        
        public int Heal()
        {
            var currentSpirit = BaseSpirit;
            var currentConcentration = BaseConcentration;
            foreach (Modifier mod in ModsList)
            {
                if (mod.attack())
                {
                    if (mod.target.Equals("Spirit"))
                    {
                        currentSpirit += mod.value;
                    }
                    else if (mod.target.Equals("Concentration"))
                    {
                        currentConcentration += mod.value;
                    }
                }
            }
            var healBase = currentSpirit - (int)((float)currentSpirit * (1f / (2f + (float)currentConcentration)));
            var healRange = 2 * (int)((float)currentSpirit * (1f / (2f + (float)currentConcentration)));
            var heal
 = healBase + rng.Next(healRange);
            return heal;
        }

        public void GetHealed(int healin, List<Modifier> effects)
        {
            var currentHealing = healin;
            foreach (Modifier mod in ModsList)
            {
                if (mod.getHealed())
                {
                     if (mod.target.Equals(""))
                        currentHealing += mod.value;
                }
            }

            if (effects != null)
            {
                foreach (Modifier mod in effects)
                {
                    ModsList.Add(mod);
                }
            }

            CurrentHealth += currentHealing;
        }

        public string UseSkill(Agent target, string skillName)
        {
            foreach(Skill skill in Skills)
            {
                if (skill.getName().Equals(skillName))
                {
                    return skill.useSkill(target, this);
                }
            }

            return "";
        }
    }
}
