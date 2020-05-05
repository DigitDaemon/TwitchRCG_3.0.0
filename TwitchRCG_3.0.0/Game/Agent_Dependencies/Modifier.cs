using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Agent_Dependencies
{
    public class Modifier
    {
        public string type { get; }
        public string target { get; }
        public int value { get; }
        public int duration { get; set; }
        public string trigger { get;  }
        public string status { get; }


        public Modifier(string type, string trigger, string target, int value, int duration)
        {
            this.type = type;
            this.trigger = trigger;
            this.target = target;
            this.value = value;
            this.duration = duration;
        }

        public Modifier(string type, string trigger, int duration, string status)
        {
            this.type = type;
            this.trigger = trigger;
            this.duration = duration;
            this.status = status;
        }

        public bool getStatus()
        {
            if (trigger.Equals("status"))
                return true;
            else
                return false;
        }

        public bool turnStart(){
            
            if (trigger.Equals("start"))
                return true;
            else
                return false;

        }

        public bool getHealed()
        {
            if (trigger.Equals("healed"))
            {
                return true;

            }
            else
                return false;
        }

        public bool attack()
        {
            if (trigger.Equals("attack"))
                return true;
            else
                return false;
        }

        public bool defend()
        {
            if (trigger.Equals("defend"))
                return true;
            else
                return false;
        }

        public bool turnEnd()
        {
            duration--;
            if (trigger.Equals("end"))
                return true;
            else
                 return false;
        }

        public bool onDeath()
        {
            if (trigger.Equals("death"))
                return true;
            else
                return false;
        }
        
    }
}
