using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;

namespace Game.Builders
{
    class CharacterBuilder
    {
        //strings: ODBC configuration
        //
        //path - the path to the folder containing sensitive information for this project on my computer
        //dbConnString - the text contained inside the Connection.txt file with information for connecting to the database
        private static string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\Debug\netcoreapp2.1", ""), @"Data\");
        static private string dbConnString = File.ReadAllText(Path.Combine(path, "Connection.txt"));//do not push this!!!

        //objs: ODBS objects
        //
        //data - the data returned from a query
        //dbAdapter - the object that handles retrival of data from the database
        //dbCommand - the command builder for <dbAdapter>
        //connDB - the connection to the database
        static DataSet data;
        static OdbcDataAdapter dbAdapter = new OdbcDataAdapter();
        static OdbcCommandBuilder dbCommand = new OdbcCommandBuilder(dbAdapter);
        static OdbcConnection connDB = new OdbcConnection(dbConnString);

        static public Agents.Agent buildCharacter(string uname)
        {
            uname = uname.TrimStart(new char[] { '\0', ':' });
            try
            {
                if (!connDB.State.HasFlag(ConnectionState.Open))
                    connDB.Open();
            }
            catch (OdbcException e)
            {
                Console.WriteLine(e.Message + "\n\n" + e.StackTrace);
            }

            data = new DataSet();
            dbAdapter.SelectCommand = new OdbcCommand("SELECT * FROM users WHERE twitch_name='" + uname + "';", connDB);
            dbAdapter.Fill(data);

            if (data.Tables[0].Rows.Count == 0)
                throw new Game.Exceptions.NoSuchPlayerException("There is no " + uname + " in the database.");

            var levelBoost = .04 * float.Parse(data.Tables[0].Rows[0]["level"].ToString());
            levelBoost += 1;

            Abstracts.Class spec;
            Abstracts.Race race;

            int temper = (int)(levelBoost * float.Parse(data.Tables[0].Rows[0]["temper"].ToString()));
            int cheer = (int)(levelBoost * float.Parse(data.Tables[0].Rows[0]["cheer"].ToString()));
            int curiosity = (int)(levelBoost * float.Parse(data.Tables[0].Rows[0]["curiosity"].ToString()));
            int charisma = (int)(levelBoost * float.Parse(data.Tables[0].Rows[0]["charisma"].ToString()));
            int empathy = (int)(levelBoost * float.Parse(data.Tables[0].Rows[0]["empathy"].ToString()));

            //[0] = highest stat
            //[1] = second highest stat
            //[2] = lowest stat
            int[] statpicks = new int[] { 0, 1, 0 };

            int[] stats = new int[] { temper, cheer, curiosity, charisma, empathy };

            for (int i = 1; i < 5; i++)
            {
                if (stats[i] > stats[statpicks[0]])
                {
                    statpicks[1] = statpicks[0];
                    statpicks[0] = i;
                }
                else if (stats[i] > stats[statpicks[1]])
                {
                    statpicks[1] = i;
                }

                if(stats[i] < stats[statpicks[2]])
                {
                    statpicks[2] = i;
                }
            }

            foreach(int stat in stats)
            {
                Console.WriteLine(stat);
            }
            foreach(int pick in statpicks)
            {
                Console.WriteLine(pick);
            }

            if ((statpicks[0] == 0 && statpicks[1] == 1) || (statpicks[0] == 1 && statpicks[1] == 0))
                spec = new Classes.Barbarian();
            else if ((statpicks[0] == 0 && statpicks[1] == 2) || (statpicks[0] == 2 && statpicks[1] == 0))
                spec = new Classes.Sorcerer();
            else if ((statpicks[0] == 0 && statpicks[1] == 3) || (statpicks[0] == 3 && statpicks[1] == 0))
                spec = new Classes.Paladin();
            else if ((statpicks[0] == 0 && statpicks[1] == 4) || (statpicks[0] == 4 && statpicks[1] == 0))
                spec = new Classes.Fighter();
            else if ((statpicks[0] == 1 && statpicks[1] == 2) || (statpicks[0] == 2 && statpicks[1] == 1))
                spec = new Classes.Druid();
            else if ((statpicks[0] == 1 && statpicks[1] == 3) || (statpicks[0] == 3 && statpicks[1] == 1))
                spec = new Classes.Bard();
            else if ((statpicks[0] == 1 && statpicks[1] == 4) || (statpicks[0] == 4 && statpicks[1] == 1))
                spec = new Classes.Cleric();
            else if ((statpicks[0] == 2 && statpicks[1] == 3) || (statpicks[0] == 3 && statpicks[1] == 2))
                spec = new Classes.Wizard();
            else if ((statpicks[0] == 2 && statpicks[1] == 4) || (statpicks[0] == 4 && statpicks[1] == 2))
                spec = new Classes.Warlock();
            else 
                spec = new Classes.Rogue();

            Console.WriteLine(spec.getName());

            if ((statpicks[0] == 1 && statpicks[2] == 0))
                race = new Races.Halfling();
            else if ((statpicks[0] == 0 && statpicks[2] == 3))
                race = new Races.Orc();
            else if ((statpicks[0] == 4 && statpicks[2] == 1) || (statpicks[0] == 3 && statpicks[2] == 1))
                race = new Races.Drow();
            else if ((statpicks[0] == 4 && statpicks[2] == 0))
                race = new Races.Tiefling();
            else if ((statpicks[0] == 2 && statpicks[2] == 4) || (statpicks[0] == 3 && statpicks[2] == 4))
                race = new Races.Gnome();
            else if ((statpicks[0] == 2 && statpicks[2] == 1) || (statpicks[0] == 3 && statpicks[2] == 0))
                race = new Races.Elf();
            else if ((statpicks[0] == 0 && statpicks[2] == 1) || (statpicks[0] == 2 && statpicks[2] == 3))
                race = new Races.HalfElf();
            else if ((statpicks[0] == 1 && statpicks[2] == 4) || (statpicks[0] == 1 && statpicks[2] == 3) ||
                (statpicks[0] == 4 && statpicks[2] == 2) || (statpicks[0] == 3 && statpicks[2] == 2))
                race = new Races.Dwarf();
            else
                race = new Races.Human();

           

            int baseHealth = race.getBaseHealth() + (int)((1f / 3f) * temper + (int)((1f / 4f) * empathy));
            //Console.WriteLine("baseHealth" + baseHealth);
            int strength = race.getBaseStrength() + (int)((1f / 4f) * temper);
            //Console.WriteLine("strength" + strength);
            int mind = race.getBaseMind() + (int)((1f / 3f) * curiosity);
            //Console.WriteLine("mind" + mind);
            int mastery = (int)((1f / 2f) * empathy);
            //Console.WriteLine("mastery" + mastery);
            int concentration = (int)((1f / 2f) * charisma);
            //Console.WriteLine("concentration" + concentration);
            int spirit = race.getBaseSpirit() + (int)((1f / 3f) * cheer);
            var skills = new List<Abstracts.Skill>();
            if (spec.getBaseSkill() != null)
                skills.Add(spec.getBaseSkill());
            int speed = race.getBaseSpeed() + (int)((1f / 4f) * cheer) + (int)((1f / 4f) * charisma);
            //Console.WriteLine("speed" + speed);
 
            connDB.Close();
            var actionPriority = spec.getActionPriority();
            var type = "Player";
            
            Agents.Agent player = new Agents.Agent(uname, baseHealth, strength, mind, concentration, mastery, spirit, 0, skills, speed, null, actionPriority, type);


            return player;
        }
    }
}
