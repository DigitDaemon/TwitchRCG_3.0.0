using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;

namespace Game
{
    /**
     * This class maintains the threads for the <GameApplication> and handles starting and shutting them down
     */
    class ThreadController
    {


        //bool: active
        //Keeps the loop in <ControlLoop> active
        bool active;

       
        //objs: Encounter Lists
        //
        //encounter - a list of ongoing encounters
        //cooldownList - a list of channels that are on cooldown and cannot have a new encounter started yet
        List<KeyValuePair<string, Threads.EncounterController>> encounters;
        List<KeyValuePair<string, System.Timers.Timer>> cooldownList;

        //objs: Queues
        //
        //gameMessageQueue - game related messages coming from <GameCommandConsumer>
        //otherMessageQueue - other relevent messages coming from <GameCommandConsumer
        //twitchOutQueue - messages to me passed to <KafkaProducer>
        //discordOutQueue - messages to be passed to <KafakProducer>
        ConcurrentQueue<KeyValuePair<string, string>> gameMessageQueue;
        ConcurrentQueue<KeyValuePair<string, string>> twitchOutQueue;
        ConcurrentQueue<KeyValuePair<string, string>> discordOutQueue;

        /**
         * The constructor for <ThreadController>
         * Initialises all objs and threads needed for the <GameApplication> to run
         */
        public ThreadController(Shared.Dispatch dispatch)
        {
            gameMessageQueue = dispatch.GetGameQueue();
            twitchOutQueue = dispatch.GetTwitchOutQueue();
            discordOutQueue = dispatch.GetDiscordOutQueue();

            encounters = new List<KeyValuePair<string, Threads.EncounterController>>();
            cooldownList = new List<KeyValuePair<string, System.Timers.Timer>>();
            active = true;
        }

        public void ControlLoop()
        {
            Console.WriteLine("ControlLoop start");
            while (active)
            {
                if (!gameMessageQueue.IsEmpty)
                {
                    KeyValuePair<string, string> message;
                    gameMessageQueue.TryDequeue(out message);
                    AddJoinGame(message.Key);
                }

                if (encounters.Exists(x => x.Value.gameOver))
                {
                    encounters.Remove(encounters.Find(x => x.Value.gameOver));
                }


            }

        }

        public void AddJoinGame(string message)
        {
            Console.WriteLine("AddJoinGame" + message);
            var channel = message.Substring(0, message.IndexOf(" "));
            var uname = message.Substring(message.IndexOf(" "), message.Length - channel.Length).Trim();
            if (encounters.Exists(x => x.Key == channel))
            {
                encounters.Find(x => x.Key == channel).Value.AddPlayer(uname);

            }
            else if(!cooldownList.Exists(x => x.Key == channel)) { 
                startEncounter(message);
                twitchOutQueue.Enqueue(new KeyValuePair<string, string>(channel, $"@{uname} is starting a quest! type '!dQuest' to join the party!"));
                discordOutQueue.Enqueue(new KeyValuePair<string, string>(channel, $"{uname} is starting a quest in {channel}!"));
            }
            else
            {
                twitchOutQueue.Enqueue(new KeyValuePair<string, string>(channel, $"@{uname}, {channel}'s guild is currently recovering from their last quest."));
            }
            

            
        }

        public void startEncounter(string message)
        {
            
            var channel = message.Substring(0, message.IndexOf(" "));
            var uname = message.Substring(message.IndexOf(" "), message.Length - channel.Length).Trim();
            Console.WriteLine("Encounter started for " + channel);
            addCooldown(channel);

            KeyValuePair<string, Threads.EncounterController> newEncounter = new KeyValuePair<string, Threads.EncounterController>(channel, new Threads.EncounterController(channel, ref twitchOutQueue, ref discordOutQueue));
            Thread encounterThread = new Thread(newEncounter.Value.encounterThread);
            encounterThread.Start();
            newEncounter.Value.AddPlayer(uname);
            encounters.Add(newEncounter);
            
        }

        public void addCooldown(string channel)
        {
            KeyValuePair<string, System.Timers.Timer> cooldown = new KeyValuePair<string, System.Timers.Timer>(channel, new System.Timers.Timer(600000));
            cooldown.Value.AutoReset = false;
            cooldown.Value.Enabled = true;
            cooldown.Value.Elapsed += cooldownEnded;
            cooldownList.Add(cooldown);
        }

        public void cooldownEnded(Object source, ElapsedEventArgs e)
        {
            for(int i = 0; i < cooldownList.Count; i++)
            {
                if (!cooldownList[i].Value.Enabled)
                {
                    cooldownList[i].Value.Elapsed -= cooldownEnded;
                    cooldownList[i].Value.Dispose();
                    twitchOutQueue.Enqueue(new KeyValuePair<string, string>(cooldownList[i].Key, "There is a new Quest posted! Start your adventure by typing in '!dQuest' into chat."));
                    cooldownList.Remove(cooldownList.Find(x => x.Key.Equals(cooldownList[i].Key)));
                   
                    break;
                }
            }
        }

        /**
         * Provides feedback on the state of the threads in <DatabaseApplication>
         */
        public void listThreads()
        {
           
        }

        /**
         * Closes all of the threads and then ends the program
         */
        public void exit()
        {
            active = false;
            System.Environment.Exit(0);
        }
    }
}
