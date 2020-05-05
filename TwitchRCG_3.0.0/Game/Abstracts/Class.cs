using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Abstracts
{
    public abstract class Class
    {
        protected List<string> ActionPriority;
        protected Skill baseSkill;
        protected string name;

        public Class(List<string> ActionPriority, Skill baseSkill, string name)
        {
            this.ActionPriority = ActionPriority;
            this.baseSkill = baseSkill;
            this.name = name;
        }


        public string getName()
        {
            return name;
        }

        public List<string> getActionPriority()
        {
            return ActionPriority;
        }

        public Skill getBaseSkill()
        {
            return baseSkill;
        }
    }
}
