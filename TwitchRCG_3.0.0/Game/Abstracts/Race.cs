using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Abstracts
{
    public abstract class Race
    {
        protected int baseHealth = 69;

        protected int baseMind = 13;

        protected int baseStrength = 13;

        protected int baseSpirit = 13;

        protected int baseSpeed = 7;

        public Race(int baseHealth, int baseMind, int baseStrength, int baseSpirit, int baseSpeed)
        {
            this.baseHealth = baseHealth;
            this.baseMind = baseMind;
            this.baseStrength = baseStrength;
            this.baseSpirit = baseSpirit;
            this.baseSpeed = baseSpeed;
        }

        public int getBaseHealth()
        {
            return baseHealth;
        }

        public int getBaseMind()
        {
            return baseMind;
        }

        public int getBaseSpeed()
        {
            return baseSpeed;
        }

        public int getBaseStrength()
        {
            return baseStrength;
        }

        public int getBaseSpirit()
        {
            return baseSpirit;
        }
    }}
