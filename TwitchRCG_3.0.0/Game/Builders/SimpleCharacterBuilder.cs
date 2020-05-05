using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;

namespace Game.Builders
{

    static class SimpleCharacterBuilder
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
                if(!connDB.State.HasFlag(ConnectionState.Open))
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

            Abstracts.Class spec = new Classes.Fighter();
            Abstracts.Race race = new Races.Human();

            int temper = (int)(levelBoost * float.Parse(data.Tables[0].Rows[0]["temper"].ToString()));
            int cheer = (int)(levelBoost * float.Parse(data.Tables[0].Rows[0]["cheer"].ToString()));
            int curiosity = (int)(levelBoost * float.Parse(data.Tables[0].Rows[0]["curiosity"].ToString()));
            int charisma = (int)(levelBoost * float.Parse(data.Tables[0].Rows[0]["charisma"].ToString()));
            int empathy = (int)(levelBoost * float.Parse(data.Tables[0].Rows[0]["empathy"].ToString()));

            int baseHealth = race.getBaseHealth() + (int)((1f / 3f) * temper + (int)((1f / 4f) * empathy));
            Console.WriteLine("baseHealth" + baseHealth);
            int strength = race.getBaseStrength() + (int)((1f / 4f) * temper);
            Console.WriteLine("strength" + strength);
            int mind = race.getBaseMind() + (int)((1f / 3f) * curiosity);
            Console.WriteLine("mind" + mind);
            int mastery = (int)((1f / 2f) * empathy);
            Console.WriteLine("mastery" + mastery);
            int concentration = (int)((1f / 2f) * charisma);
            Console.WriteLine("concentration" + concentration);
            int spirit = race.getBaseSpirit() + (int)((1f / 3f) * cheer);
            var skills = new List<Abstracts.Skill>();
            int speed = race.getBaseSpeed() + (int)((1f / 4f) * cheer) + (int)((1f / 4f) * charisma);
            Console.WriteLine("speed" + speed);
            Agents.Agent player = new Agents.Agent(baseHealth, strength, mind, concentration, mastery, spirit, skills, speed, spec, race, uname);
            connDB.Close();

            return player;
        }
    }
}
