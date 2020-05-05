using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    //Class: Dispatch
    //
    //Handles communication between the different parts of the application.
    class Dispatch
    {
        ConcurrentQueue<KeyValuePair<string, string>> DatabaseQueue;
        ConcurrentQueue<KeyValuePair<string, string>> GameQueue;
        ConcurrentQueue<KeyValuePair<string, string>> TwitchOutQueue;
        ConcurrentQueue<KeyValuePair<string, string>> DiscordOutQueue;
        ConcurrentQueue<KeyValuePair<string, Exception>> ErrorQueue;


        public Dispatch()
        {
            DatabaseQueue = new ConcurrentQueue<KeyValuePair<string, string>>();
            GameQueue = new ConcurrentQueue<KeyValuePair<string, string>>();
            TwitchOutQueue = new ConcurrentQueue<KeyValuePair<string, string>>();
            DiscordOutQueue = new ConcurrentQueue<KeyValuePair<string, string>>();
            ErrorQueue = new ConcurrentQueue<KeyValuePair<string, Exception>>();
        }

        public Dispatch GetDispatch()
        {
            return this;
        }

        public ref ConcurrentQueue<KeyValuePair<string, string>> GetDatabaseQueue()
        {
            return ref DatabaseQueue;
        }

        public ref ConcurrentQueue<KeyValuePair<string, string>> GetGameQueue()
        {
            return ref GameQueue;
        }

        public ref ConcurrentQueue<KeyValuePair<string, string>> GetTwitchOutQueue()
        {
            return ref TwitchOutQueue;
        }

        public ref ConcurrentQueue<KeyValuePair<string, string>> GetDiscordOutQueue()
        {
            return ref DiscordOutQueue;
        }

        public ref ConcurrentQueue<KeyValuePair<string, Exception>> GetErrorQueue()
        {
            return ref ErrorQueue;
        }
    }
}
