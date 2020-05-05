using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace TwitchComs
{
    /**
     * This class is responcible for getting the messages from Twitch through an irc client.
     * Each channel being connected to will have its own instance of this class.
     */
    class TwitchClientWriter
    {
        //consts: 
        //
        //ALLOWABLE_FAILURES - the number of times the client is allowed to fail to connect
        //RESET_CYCLE - the number of times the event will trigger before the class will disconnect and reconnect the irc client
        //username - the name of the twitch account being used to connect
        //path - the file path to the folder containing protected information
        //password - the OAuth token for the account being used to connect
        //trimChar - an array of characters used when parsing the irc messages
        //lengthMin - the minumum size of a message to be passed into the system.
        const int ALLOWABLE_FAILURES = 10;
        const int RESET_CYCLES = 100;
        private System.Timers.Timer messageCooldown;
        private const string username = "ddigitbot";
        private static string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\Debug\netcoreapp2.1", ""), @"Data\");
        private static string password = File.ReadAllText(Path.Combine(path, "Token.txt"));//do not push this!!!
        

        //objs: cross class shared resources
        //
        //messageQueue - Messages will be enqueued to this queue in order to make them availible to <KafkaProducer>
        //trigger - this timer is the trigger for the event to check for new messages
        //blacklist - a collection of users who ar enot participating and messages should be excluded 
        ConcurrentQueue<KeyValuePair<string, string>> MessageQueue;
        ConcurrentQueue<KeyValuePair<string, Exception>> ErrorQueue;
        private bool Alive;

        //vars: client resources
        //
        //client - the TCP client that connects to the twitch irc channel
        //writer - the stream writer that will write to the irc channel
        //reader - the stream reader that reads from the irc channel
        //cycles - the number of events that have triggered since the last reconnect
        //channel - the name of the twitch channel being connected to
        TcpClient client;
        StreamWriter writer;
        int cycles;
        string channel;
        string messageTemplate; 
        //Function: TwitchClient
        //
        //The constructor for this class.
        //
        //Parameters:
        //channel - the name of the channel to connect to
        //trigger - a refrence to the timer shared by all <TwitchClient> objects, triggers the event handler
        //queue - a refrence to the queue shared by all the <TwitchClient> objects and the <KafkaProducer>
        //blacklist - the users who ar enot participating and should be ignored
        public TwitchClientWriter(String channel, Shared.Dispatch dispatch)
        {
            Console.WriteLine(channel + " Thread Start");
            cycles = 0;
            this.MessageQueue = dispatch.GetTwitchOutQueue();
            this.ErrorQueue = dispatch.GetErrorQueue();
            this.channel = channel;
            messageCooldown = new System.Timers.Timer(2000);
            messageCooldown.AutoReset = false;
            Alive = true;

            messageTemplate = $":{username}!{username}@{username}.tmi.twitch.tv PRIVMSG #{channel} : ";
            //messageTemplate = $"PRIVMSG #{channel} :";
            Console.WriteLine("Channel Thread " + channel + " start");
            Connect();
        }

        /**
         * unsubscribes the event handler from <trigger> allowing the object to be disposed of
         */
       public void Kill()
        {
            Alive = false;
            Console.WriteLine("thread end");
        }

        /**
         * Closes the resource used by the class
         */
        private void closeClassResources()
        {
            try
            {
                client.Close();
            }
            catch (Exception e)
            {
                KeyValuePair<string, Exception> message = new KeyValuePair<string, Exception>("TwitchClientWriter", e);
                ErrorQueue.Enqueue(message);
            }
            try
            {
                writer.Close();
            }
            catch (Exception e)
            {
                KeyValuePair<string, Exception> message = new KeyValuePair<string, Exception>("TwitchClientWriter", e);
                ErrorQueue.Enqueue(message);
            }
            
        }

        /**
         * Closes the client resources and then reconnects
         */
        private void flushResources()
        {
            closeClassResources();
            Thread.Yield();
            Connect();
        }

        /**
         * This is the event handler that reads from the irc client when <trigger> triggers
         */
        private void TCWThread()
        {
            while (Alive)
            {
                cycles++;

                if (cycles > RESET_CYCLES)
                {
                    flushResources();
                    cycles = 0;
                }

                if (cycles % 100 == 0)
                {
                    Thread.Yield();
                }

                if (!client.Connected)
                {
                    Connect();
                }

                try
                {
                    if (!MessageQueue.IsEmpty && messageCooldown.Enabled == false)
                    {
                        KeyValuePair<string, string> msg;
                        var ready = MessageQueue.TryDequeue(out msg);
                        var message = messageTemplate + msg.Value;
                        writer.WriteLine(message);
                        writer.Flush();
                        messageCooldown.Enabled = true;
                    }
                    else
                    {
                        Thread.Yield();
                    }
                }
                catch (Exception j)
                {
                    KeyValuePair<string, Exception> message = new KeyValuePair<string, Exception>("TwitchClientWriter", j);
                    ErrorQueue.Enqueue(message);
                    flushResources();
                }

            }

        }

        /**
         * The function that connects all the client resources to the irc channel
         */
        void Connect()
        {
            Console.WriteLine("Connect to " + channel);
            int attempts = 0;
            do
            {

                try
                {
                    attempts++;
                    client = new TcpClient("irc.chat.twitch.tv.", 6667);
                    writer = new StreamWriter(client.GetStream());
                    writer.WriteLine("PASS " + password + Environment.NewLine
                        + "NICK " + username + Environment.NewLine
                        + "USER " + username + " 8 * :" + username);
                    writer.WriteLine("JOIN #" + channel);
                    writer.Flush();
                    
                    break;
                }
                catch (Exception e)
                {
                    KeyValuePair<string, Exception> message = new KeyValuePair<string, Exception>("TwitchClientWriter", e);
                    ErrorQueue.Enqueue(message);
                    if (attempts > ALLOWABLE_FAILURES)
                        throw new Exception("Network Error" + e.Message);
                    else
                        Thread.Sleep(1000);
                }
            } while (true);
        }



    }
}
