using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace Diswords.Core.Databases
{
    public class DatabaseGame
    {
        public string Id;
        public string Language;
        public int Type;
        public ulong GuildId;
        public ulong ChannelId;

        public DatabaseGame(string id, string language, int type, long guildId, long channelId)
        {
            Id = id;
            Language = language;
            Type = type;
            GuildId = (ulong)guildId;
            ChannelId = (ulong)channelId;
        }
    }

    public static class GameDatabaseHelper
    {
        public static DatabaseGame GetGame(ulong id)
        {
            var reader = DatabaseHelper.ExecuteReader($"select * from games where id == {id}");
            reader.Read();
            var game = new DatabaseGame(reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt64(3), reader.GetInt64(4));
            reader.Close();
            return game;
        }

        public static IEnumerable<DatabaseGame> GetAllGames()
        {
            var reader = DatabaseHelper.ExecuteReader("select * from games");
            var list = new List<DatabaseGame>();
            while (reader.Read())
                list.Add(new DatabaseGame(reader.GetString(0), reader.GetString(1), reader.GetInt32(2),
                    reader.GetInt64(3), reader.GetInt64(4)));
            reader.Close();
            return list;
        }

        public static void InsertGame(DatabaseGame game) => DatabaseHelper.ExecuteNonQuery($"replace into games values('{game.Id}', '{game.Language}', {game.Type}, {game.ChannelId}, {game.GuildId}");
    }
}