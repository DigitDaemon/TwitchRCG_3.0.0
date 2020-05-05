using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;

namespace Game
{
    static class PlayerUpdater
    {
        //strings: ODBC configuration
        //
        //path - the path to the folder containing sensitive information for this project on my computer
        //dbConnString - the text contained inside the Connection.txt file with information for connecting to the database
        private static readonly string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\Debug\netcoreapp2.1", ""), @"Data\");
        static private readonly string dbConnString = File.ReadAllText(Path.Combine(path, "Connection.txt"));//do not push this!!!

        //objs: ODBS objects
        //
        //data - the data returned from a query
        //dbAdapter - the object that handles retrival of data from the database
        //dbCommand - the command builder for <dbAdapter>
        //connDB - the connection to the database
        static DataSet data;
        static OdbcDataAdapter dbAdapter = new OdbcDataAdapter();
        static readonly OdbcCommandBuilder dbCommand = new OdbcCommandBuilder(dbAdapter);
        static OdbcConnection connDB = new OdbcConnection(dbConnString);

        static public void UpdateCharacter(int exp, string player)
        {
            try
            {
                connDB.Open();
            }
            catch (OdbcException e)
            {
                Console.WriteLine(e.Message + "\n\n" + e.StackTrace);
            }
            data = new DataSet();
            dbAdapter.SelectCommand = new OdbcCommand("SELECT * FROM users WHERE twitch_name='" + player + "';", connDB);
            dbAdapter.Fill(data);
            var newExp = exp + int.Parse(data.Tables[0].Rows[0]["experience"].ToString());
            OdbcCommand update = new OdbcCommand("UPDATE users SET experience=" + newExp + "WHERE twitch_name='" + player + "';", connDB);
            update.ExecuteNonQueryAsync().Wait();
            connDB.Close();
        }
    }
}
