using Game.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Classes
{
    class Fighter : Class
    {
        public Fighter()
            : base(new List<string>() { "Item", "Skill", "PhysAttack", "Guard" }, new Skills.PowerAttack(), "Fighter")
        {

        }
    }

    class Sorcerer : Class
    {
        public Sorcerer() : base(new List<string>() { "Item", "Skill", "MagAttack", "Guard" }, new Skills.Fireball(), "Sorcerer")
        {

        }
       
    }

    class Barbarian : Class
    {
        public Barbarian() : base(new List<string>() { "Item", "Skill", "PhysAttack", "Guard" }, new Skills.AttackRecklessly(), "Barbarian") {
        }
        
    }

    class Paladin : Class
    {
        public Paladin() : base( new List<string>() { "Item", "Skill", "PhysAttack", "Guard" }, new Skills.LightOfSalvation(), "Paladin")
        {

        }
        
    }

    class Bard : Class
    {
        public Bard():base(new List<string>() { "Item", "Skill", "Heal", "MagAttack", "Guard" }, null, "Bard")
        {

        }
        
    }

    class Rogue : Class
    {
        public Rogue():base(new List<string>() { "Item", "Skill", "PhysAttack", "Guard" },null, "Rogue")
        {

        }
    }

    class Wizard : Class
    {
        public Wizard():base(new List<string>() { "Item", "Skill", "MagAttack", "Guard" },null, "Wizard")
        {

        }
    }

    class Warlock : Class
    {
        public Warlock():base(new List<string>() { "Item", "Skill", "MagAttack", "Guard" },null, "Warlock")
        {

        }
        
    }

    class Cleric : Class
    {
        public Cleric() : base(new List<string>() { "Item", "Skill", "Heal", "PhysAttack", "Guard" }, null, "Cleric")
        {

        }
        
    }

    class Druid : Class
    {
        public Druid() : base(new List<string>() { "Item", "Skill", "MagAttack", "Guard" }, null, "Druid")
        {

        }
    }
}
