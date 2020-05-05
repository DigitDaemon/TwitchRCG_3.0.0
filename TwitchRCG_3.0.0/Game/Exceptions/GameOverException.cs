using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Exceptions
{
    class GameOverException : Exception
    {
        List<string> messages { get; }
        public GameOverException(List<string> messages)
        {
            this.messages = messages;
        }

        public GameOverException(string message, List<string> messages)
            : base(message)
        {
            this.messages = messages;
        }

        public GameOverException(string message, Exception inner, List<string> messages)
            : base(message, inner)
        {
            this.messages = messages;
        }

        public List<string> getMessages()
        {
            return messages;
        }
    }
}
