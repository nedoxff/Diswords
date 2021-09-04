using System.Collections.Generic;
using System.Data.Entity;

namespace Diswords.Core.Databases.Types
{
    public class DatabaseGuild
    {
        public ulong Id;
        public uint GamesPlayed;
        public string Language;

        public DatabaseGuild(ulong id, uint gamesPlayed, string language)
        {
            Id = id;
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
            var guild = new DatabaseGuild((ulong)reader.GetInt64(0), (uint)reader.GetInt32(1), reader.GetString(2));
            reader.Close();
            return guild;
        }

        public static IEnumerable<DatabaseGuild> GetAllGuilds()
        {
            var reader = DatabaseHelper.ExecuteReader("select * from guilds");
            var list = new List<DatabaseGuild>();
            while(reader.Read())
                list.Add(new DatabaseGuild((ulong)reader.GetInt64(0), (uint)reader.GetInt32(1), reader.GetString(2)));
            reader.Close();
            return list;
        }

        public static void InsertGuild(DatabaseGuild guild) => DatabaseHelper.ExecuteNonQuery($"replace into guilds values({guild.Id}, {guild.GamesPlayed}, '{guild.Language}')");

        public static string GetLanguage(ulong id) => (string) DatabaseHelper.ExecuteScalar($"select language from guilds where id == {id}");
        public static void SetLanguage(ulong id, string language) => DatabaseHelper.ExecuteNonQuery($"update guilds set language = '{language}' where guilds.id == {id}");
    }
}