using System.Collections.Generic;
using System.Data.SQLite;
using Serilog;

namespace Diswords.Core.Databases
{
    public class DatabaseGuild
    {
        public ulong Id;
        public ulong ParentGameCategory;
        public uint GamesPlayed;
        public string Language;

        public DatabaseGuild(ulong id, ulong parentGameCategory, uint gamesPlayed, string language)
        {
            Id = id;
            ParentGameCategory = parentGameCategory;
            GamesPlayed = gamesPlayed;
            Language = language;
        }
    }
    
    public static class GuildDatabaseHelper
    {
        public static DatabaseGuild GetGuild(ulong id)
        {
            var reader = DatabaseHelper.ExecuteReader($"select * from guilds where id == {id}");
            reader.Read();
            var guild = new DatabaseGuild((ulong)reader.GetInt64(0), (ulong)reader.GetInt64(1), (uint)reader.GetInt32(2), reader.GetString(3));
            reader.Close();
            return guild;
        }

        public static IEnumerable<DatabaseGuild> GetAllGuilds()
        {
            var reader = DatabaseHelper.ExecuteReader("select * from guilds");
            var list = new List<DatabaseGuild>();
            while(reader.Read())
                list.Add(new DatabaseGuild((ulong)reader.GetInt64(0), (ulong)reader.GetInt64(1), (uint)reader.GetInt32(2), reader.GetString(3)));
            reader.Close();
            return list;
        }

        private static void GuildNonQuery(ulong id, string query)
        {
            AddIfDoesntExist(id);
            DatabaseHelper.ExecuteNonQuery(query);
        }

        private static object GuildScalar(ulong id, string query)
        {
            AddIfDoesntExist(id);
            return DatabaseHelper.ExecuteScalar(query);
        }

        private static SQLiteDataReader GuildReader(ulong id, string query)
        {
            AddIfDoesntExist(id);
            return DatabaseHelper.ExecuteReader(query);
        }

        public static void InsertGuild(DatabaseGuild guild) => DatabaseHelper.ExecuteNonQuery($"replace into guilds values({guild.Id}, {guild.GamesPlayed}, {guild.ParentGameCategory}, '{guild.Language}')");

        public static string GetLanguage(ulong id) => (string) GuildScalar(id, $"select language from guilds where id == {id}");
        public static void SetLanguage(ulong id, string language) => GuildNonQuery(id, $"update guilds set language = '{language}' where guilds.id == {id}");

        public static void SetParentGameCategory(ulong id, ulong category) =>
            GuildNonQuery(id,
                $"update guilds set parent_game_category = {category} where guilds.id == {id}");

        public static long GetParentGameCategory(ulong id) 
        {
            var reader = GuildReader(id, $"select parent_game_category from guilds where guilds.id == {id}");
            reader.Read();
            var category = reader.GetInt64(0);
            reader.Close();
            return category;
        }

        private static void AddIfDoesntExist(ulong id)
        {
            var exists = (long)DatabaseHelper.ExecuteScalar($"select exists(select 1 from guilds where guilds.id == {id})");
            if (exists != 0) return;
            Log.Warning($"Guild {id} wasn't found in the database! Adding..");
            InsertGuild(new DatabaseGuild(id, 0, 0, "en"));
        }
        
    }
}