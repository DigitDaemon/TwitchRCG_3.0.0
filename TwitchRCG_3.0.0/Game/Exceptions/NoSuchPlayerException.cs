using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Exceptions
{
    class NoSuchPlayerException : Exception
    {
        public NoSuchPlayerException()
        {
        }

        public NoSuchPlayerException(string message)
            : base(message)
        {
        }

        public NoSuchPlayerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
