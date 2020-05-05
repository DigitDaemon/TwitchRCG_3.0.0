using System;
using System.Collections.Generic;
using Medallion.Collections;

namespace Game.Encounters
{

    class MonsterEncounter : Game.Abstracts.Encounter
    {
        public List<Game.Agents.Agent> monsters;
        public List<Game.Agents.Agent> deadMonsters;
        PriorityQueue<Game.Agents.Agent> turnList;

        public MonsterEncounter(List<Game.Agents.Agent> playerList, List<Game.Agents.Agent> monsters)
            : base(playerList)
        {
            deadMonsters = new List<Game.Agents.Agent>();
            this.monsters = monsters;
            turnList = new PriorityQueue<Game.Agents.Agent>(new Game.SpeedComparer());
            
        }

        public override void checkCompletion(List<string> messages)
        {
            if(monsters.Count == 0 || playerList.Count == 0)
            {
                throw new Exceptions.GameOverException("The game is over", messages);
            }
        }

        public override string endEncounter()
        {
            var messages = "";

            if (playerList.Count > 0) {
                if(deadPlayers.Count == 0)
                {
                    messages = "The quest was successful and there were no casualties";
                }
                else
                {
                    var message = "The quest was successful but there were losses. ";
                    foreach(Game.Agents.Agent player in deadPlayers)
                    {
                        message += player.GetName() + " ";
                    }
                    message += "perished. However, ";
                    foreach (Game.Agents.Agent player in playerList)
                    {
                        message += player.GetName() + " ";
                    }
                    message += "managed to make it out alive after finishing off the monsters.";
                    messages = message;
                }
            }
            else
            {
                messages = "The quest has failed and all party members perished";
            }

            int exp = 0;
            foreach(Game.Agents.Agent monster in deadMonsters)
            {
                exp += monster.GetExp();
            }

            foreach(Game.Agents.Agent player in playerList)
            {
                PlayerUpdater.UpdateCharacter(exp, player.GetName());
            }
            foreach (Game.Agents.Agent player in deadPlayers)
            {
                PlayerUpdater.UpdateCharacter(exp/2, player.GetName());
            }

            return messages;
        }

        public override List<string> nextTurn()
        {
            if (turnList.Count == 0)
                FillTurnList();

            var messages = new List<string>();

            Game.Agents.Agent currentAgent = turnList.Dequeue();
            KeyValuePair<string, string> action = new KeyValuePair<string, string>("","");

            try
            {
                if (currentAgent.GetAgentType().Equals("Player"))
                {
                    action = currentAgent.GetAction(GetMonstersCondition(currentAgent.GetName()), GetPlayersCondition(currentAgent.GetName()));
                }
                else
                {
                    action = currentAgent.GetAction(GetPlayersCondition(currentAgent.GetName()), GetMonstersCondition(currentAgent.GetName()));
                }
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.Message);
                messages.Add(currentAgent.GetName() + " has skipped their turn.");
            }

            if (action.Value.Equals("PhysAttack"))
            {
                Console.WriteLine(currentAgent.GetName() + " is physically attacking " + action.Key);
                messages.Add(currentAgent.GetName() + " is physically attacking " + action.Key);
                try
                {
                    if (currentAgent.GetAgentType().Equals("Player"))
                    {
                        var damage = currentAgent.PhysAttack();
                        Console.WriteLine(damage);
                        monsters.Find(x => x.GetName().Equals(action.Key)).TakeDamage(damage, "Physical", null);
                    }
                    else
                    {
                        var damage = currentAgent.PhysAttack();
                        Console.WriteLine(damage);
                        playerList.Find(x => x.GetName().Equals(action.Key)).TakeDamage(damage, "Physical", null);
                    }
                }
                catch (Exceptions.DeathException death)
                {
                    if (death.getObject().GetAgentType().Equals("Player"))
                    {
                        deadPlayers.Add(death.getObject());
                        playerList.RemoveAt(playerList.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    else
                    {
                        deadMonsters.Add(death.getObject());
                        monsters.RemoveAt(monsters.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    checkCompletion(messages);
                }
            }
            else if (action.Value.Equals("MagAttack"))
            {
                Console.WriteLine(currentAgent.GetName() + " is attacking " + action.Key + " with magic");
                messages.Add(currentAgent.GetName() + " is attacking " + action.Key + " with magic");

                try
                {
                    if (currentAgent.GetAgentType().Equals("Player"))
                        monsters.Find(x => x.GetName().Equals(action.Key)).TakeDamage(currentAgent.PhysAttack(), "Magical", null);
                    else
                        playerList.Find(x => x.GetName().Equals(action.Key)).TakeDamage(currentAgent.PhysAttack(), "Magical", null);
                }
                catch (Exceptions.DeathException death)
                {
                    if (death.getObject().GetAgentType().Equals("Player"))
                    {
                        deadPlayers.Add(death.getObject());
                        playerList.RemoveAt(playerList.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    else
                    {
                        deadMonsters.Add(death.getObject());
                        monsters.RemoveAt(monsters.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    checkCompletion(messages);
                }
            }
            else if (action.Value.Equals("Heal"))
            {
                Console.WriteLine(currentAgent.GetName() + " is healing " + action.Key + ".");
                messages.Add(currentAgent.GetName() + " is healing " + action.Key + ".");
                if (currentAgent.GetAgentType().Equals("Player"))
                {
                    playerList.Find(x => x.GetName().Equals(action.Key)).GetHealed(currentAgent.Heal(), null);
                }
                else
                {
                    monsters.Find(x => x.GetName().Equals(action.Key)).GetHealed(currentAgent.Heal(), null);
                }
            }
            else if (action.Value.Contains("Skill"))
            {
                var target = action.Key.Substring(0, action.Key.IndexOf(" "));
                var type = action.Key.Substring(action.Key.IndexOf(" "), action.Key.Length - target.Length).Trim();
                var skillname = action.Value.Substring(0, action.Value.IndexOf(" "));
                try
                {
                    if (type.Equals("Monster"))
                    {
                        var message = currentAgent.UseSkill(monsters.Find(x => x.GetName().Equals(target)), skillname);
                        //Console.WriteLine(message);
                        messages.Add(message);
                    }
                    else
                    {
                        var message = currentAgent.UseSkill(playerList.Find(x => x.GetName().Equals(target)), skillname);
                        //Console.WriteLine(message);
                        messages.Add(message);
                    }
                }
                catch (Exceptions.DeathException death)
                {
                    if (death.getObject().GetAgentType().Equals("Player"))
                    {
                        deadPlayers.Add(death.getObject());
                        playerList.RemoveAt(playerList.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    else
                    {
                        deadMonsters.Add(death.getObject());
                        monsters.RemoveAt(monsters.IndexOf(death.getObject()));
                        Console.WriteLine(death.Message);
                        messages.Add(death.Message);
                    }
                    checkCompletion(messages);
                }
            }

                return messages;     
        }

        public void FillTurnList()
        {
            foreach (Game.Agents.Agent player in playerList)
            {
                turnList.Enqueue(player);
            }
            foreach (Game.Agents.Agent monster in monsters)
            {
                turnList.Enqueue(monster);
            }
        }

        public List<KeyValuePair<string,string>> GetPlayersCondition(string source)
        {
            List<KeyValuePair<string, string>> conditionList = new List<KeyValuePair<string, string>>();
            foreach(Game.Agents.Agent player in playerList)
            {
                if (!player.GetName().Equals(source))
                {
                    var list = player.GetStatus();
                    foreach(KeyValuePair<string, string> status in list)
                    {
                        conditionList.Add(status);
                    }
                }
            }

            return conditionList;
        }

        public List<KeyValuePair<string, string>> GetMonstersCondition(string source)
        {
            List<KeyValuePair<string, string>> conditionList = new List<KeyValuePair<string, string>>();
            foreach (Game.Agents.Agent monster in monsters)
            {
                if (!monster.GetName().Equals(source))
                {
                    var list = monster.GetStatus();
                    foreach (KeyValuePair<string, string> status in list)
                    {
                        conditionList.Add(status);
                    }
                }
            }

            return conditionList;
        }


    }
}
