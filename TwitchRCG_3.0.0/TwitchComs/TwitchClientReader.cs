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
    class TwitchClientReader
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
        const int RESET_CYCLES = 1000;
        private const string username = "DigitDaemon";
        private static string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\Debug\netcoreapp2.1", ""), @"Data\");
        private static string password = File.ReadAllText(Path.Combine(path, "Token.txt"));//do not push this!!!
        private static char[] trimChar = new char[69] { 'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p',
            'q','r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G','H','I','J','K','L',
            'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',' ','.','@','#','!', '_', '-', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
        const int lengthMin = 8;

        //objs: cross class shared resources
        //
        //messageQueue - Messages will be enqueued to this queue in order to make them availible to <KafkaProducer>
        ConcurrentQueue<KeyValuePair<string, string>> GameQueue;
        ConcurrentQueue<KeyValuePair<string, string>> DatabaseQueue;
        ConcurrentQueue<KeyValuePair<string, Exception>> ErrorQueue;


        //vars: client resources
        //
        //client - the TCP client that connects to the twitch irc channel
        //writer - the stream writer that will write to the irc channel
        //reader - the stream reader that reads from the irc channel
        //cycles - the number of events that have triggered since the last reconnect
        //channel - the name of the twitch channel being connected to
        TcpClient client;
        StreamWriter writer;
        StreamReader reader;
        private int cycles;
        private string channel;
        private bool live;

        //Function: TwitchClient
        //
        //The constructor for this class.
        //
        //Parameters:
        //channel - the name of the channel to connect to
        //trigger - a refrence to the timer shared by all <TwitchClient> objects, triggers the event handler
        //queue - a refrence to the queue shared by all the <TwitchClient> objects and the <KafkaProducer>
        //blacklist - the users who ar enot participating and should be ignored
        public TwitchClientReader(string channel, Shared.Dispatch dispatch)
        {
            Console.WriteLine("Twitch Message Fetch Object Start");
            this.cycles = 0;
            this.GameQueue = dispatch.GetGameQueue();
            this.DatabaseQueue = dispatch.GetDatabaseQueue();
            this.channel = channel;
        }

        /**
         * Does the primary connect to the twitch irc channel and then subscribes its event handler to <trigger>
         */
        public void CThread()
        {
            Console.WriteLine("Twitch Message Fetch Thread Start");
            Connect();
            live = true;
            while (live)
            {
                fetch();
            }
        }

        /**
         * Ends the loop, allowing the thread to die.
         */
       public void Kill()
        {
            live = false;
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
                KeyValuePair<string, Exception> message = new KeyValuePair<string, Exception>("TwitchClientReader", e);
                ErrorQueue.Enqueue(message);
            }
            try
            {
                writer.Close();
            }
            catch (Exception e)
            {
                KeyValuePair<string, Exception> message = new KeyValuePair<string, Exception>("TwitchClientReader", e);
                ErrorQueue.Enqueue(message);
            }
            try
            {
                reader.Close();
            }
            catch (Exception e)
            {
                KeyValuePair<string, Exception> message = new KeyValuePair<string, Exception>("TwitchClientReader", e);
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
        private void fetch()
        {
            cycles++;

            if (cycles > RESET_CYCLES)
            {
                flushResources();
                cycles = 0;
            }

            if (cycles % 100 == 0)
            {
                Thread.Sleep(500);
            }

            if (!client.Connected)
            {
                Connect();
            }

            try
            {
                if (client.Available > 0 || reader.Peek() >= 0)
                {

                    var message = reader.ReadLine();
                    var uname = "";

                    message = message.Remove(0, 1);
                    if (message.Contains("@") && !message.Contains("JOIN"))
                    {
                        while (!message[0].Equals('!'))
                        {
                            uname += message[0];
                            message = message.Remove(0, 1);
                        }

                        message = message.TrimStart(trimChar);
                        message = message.Remove(0, 1);
                        KeyValuePair<string, string> messageOut = new KeyValuePair<string, string>(uname, message);

                        if (message.Contains("!join"))
                        {
                            GameQueue.Enqueue(messageOut);
                        }
                        else
                        {
                            DatabaseQueue.Enqueue(messageOut);
                        }
                        
                    }
                    


                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception j)
            {
                KeyValuePair<string, Exception> message = new KeyValuePair<string, Exception>("TwitchClientReader", j);
                ErrorQueue.Enqueue(message);
                flushResources();
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
                    reader = new StreamReader(client.GetStream());
                    writer.WriteLine("PASS " + password + Environment.NewLine
                        + "NICK " + username + Environment.NewLine
                        + "USER " + username + " 8 * :" + username);
                    writer.WriteLine("JOIN #" + channel);
                    writer.Flush();
                    
                    break;
                }
                catch (Exception e)
                {
                    KeyValuePair<string, Exception> message = new KeyValuePair<string, Exception>("TwitchClientReader", e);
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
