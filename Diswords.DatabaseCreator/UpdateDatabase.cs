using System;
using System.IO;
using Diswords.Core;
using Diswords.Core.Databases;
using Serilog;

namespace Diswords.DatabaseCreator
{
    public static class UpdateDatabase
    {
        public static void Call()
        {
            Console.Clear();
            
            Log.Information("Updating the database in progress..");
            
            Log.Debug("Getting important data from the database..\nGetting guilds..");
            var guilds = GuildDatabaseHelper.GetAllGuilds();
            
            Log.Debug("Getting games..");
            var games = GameDatabaseHelper.GetAllGames();
            
            Log.Debug("Getting languages..");
            var (databaseLanguages, _) = LanguageInfo.GetLanguageInfo();
            
            var path = DatabaseHelper.Connection.ConnectionString.Replace("Data Source=", "").Trim();
            var backup = path + ".old";
            
            if (string.IsNullOrEmpty(path))
            {
                Log.Fatal("Cannot update database in memory mode! DataSource was null.");
                return;
            }

            Log.Debug("Backing up..");
            File.Copy(path, backup, true);

            try
            {
                Log.Debug("Creating the new database..");
                File.Delete(path);

                CreateDatabase.Call(path);
                
                foreach (var databaseLanguage in databaseLanguages)
                    ModifyLanguages.InstallLanguage(databaseLanguage);
                foreach (var databaseGuild in guilds)
                    GuildDatabaseHelper.InsertGuild(databaseGuild);
                foreach (var databaseGame in games)
                    GameDatabaseHelper.InsertGame(databaseGame);
                
                if (File.Exists(backup))
                    File.Delete(backup);

                Log.Information("Done!");
            }
            catch (Exception e)
            {
                Log.Error($"Failed to update database! Reverting.. Reason: {e}");

                if (File.Exists(path))
                    File.Delete(path);

                File.Move(backup, path);
            }
        }
    }
}