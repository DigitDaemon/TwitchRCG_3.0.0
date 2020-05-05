using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;
using Game.Encounters;

namespace Game.Threads
{
    class EncounterController
    {
        string channel;
        Game.Abstracts.Encounter encounter;
        System.Timers.Timer joinPeriod;
        ConcurrentQueue<KeyValuePair<string, string>> twitchOutQueue;
        ConcurrentQueue<KeyValuePair<string, string>> discordOutQueue;
        List<Agents.Agent> Players;
        bool open;
        public bool gameOver;

        public EncounterController(string channel, ref ConcurrentQueue<KeyValuePair<string, string>> twitchOutQueue, ref ConcurrentQueue<KeyValuePair<string, string>> discordOutQueue)
        {
            this.channel = channel;
            this.twitchOutQueue = twitchOutQueue;
            this.discordOutQueue = discordOutQueue;
            Players = new List<Game.Agents.Agent>();
            open = true;
            gameOver = false;

            joinPeriod = new System.Timers.Timer(60000);//change this back
            joinPeriod.Elapsed += closeJoin;
            joinPeriod.Enabled = true;
            Console.WriteLine("EncounterController created" + channel);
        }

        public void closeJoin(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("closeJoin" + channel);
            open = false;
            joinPeriod.Elapsed -= closeJoin;
            joinPeriod.Dispose();
            enqueTwitch("The party is leaving for the quest, wish them luck!");
            enqueDiscord("The party is leaving for the quest, wish them luck!");
        }

        public void encounterThread()
        {
            Console.WriteLine("EncounterThread start" +channel);
            while (open)
            {
                Thread.Sleep(500);
            }
            encounter = Game.Builders.SimpleEncounterBuilder.BuildEncounter(Players);
            while (!gameOver)
            {
                try
                {
                   var messageList = encounter.nextTurn();
                   foreach(string message in messageList)
                    {
                        discordOutQueue.Enqueue(new KeyValuePair<string,string>(channel,message));
                    }
                    Thread.Sleep(2000);
                }
                catch(Exceptions.GameOverException go)
                {
                    foreach(string message in go.getMessages())
                    {
                        discordOutQueue.Enqueue(new KeyValuePair<string, string>(channel, message));
                    }
                    gameOver = true;
                }
            }
            enqueTwitch(encounter.endEncounter());
            enqueDiscord(encounter.endEncounter());
        }

        public void enqueTwitch(string message)
        {
            twitchOutQueue.Enqueue(new KeyValuePair<string, string>(channel, message));
        }

        public void enqueDiscord(string message)
        {
            discordOutQueue.Enqueue(new KeyValuePair<string, string>(channel, message));
        }

        public void AddPlayer(string uname)
        {
            if (open)
            {
                if (!Players.Exists(x => x.GetName().Equals(uname))) {
                    try
                    {
                        Players.Add(Builders.CharacterBuilder.buildCharacter(uname));
                    }
                    catch (Exceptions.NoSuchPlayerException nsp)
                    {
                        enqueTwitch($"@{uname}You are not currently registered in TwitchRCG. You can register by typing '!dJoin' into chat.");
                    }
                }
            }
            else
            {
                enqueTwitch($"Sorry @{uname} the party has already left.");
            }
            enqueDiscord($"{uname} has joined {channel} guild's party!");
        }
    }
}
