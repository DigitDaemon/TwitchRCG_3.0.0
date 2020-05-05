using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Exceptions
{
    public class DeathException : Exception
    {
        Game.Agents.Agent source;

        public DeathException(Game.Agents.Agent source)
        {
            this.source = source;
        }

        public DeathException(string message, Game.Agents.Agent source)
            : base(message)
        {
            this.source = source;
        }

        public DeathException(string message, Exception inner, Game.Agents.Agent source)
            : base(message, inner)
        {
            this.source = source;
        }

        public Game.Agents.Agent getObject()
        {
            return source;
        }
    }
}
