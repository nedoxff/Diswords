using System;
using System.Collections.Generic;
using System.Linq;

namespace Diswords.Core.Databases
{
    public class DatabaseGame
    {
        public string Id;
        public string Language;
        public string LastLetter;
        public List<ulong> Players;
        public int Type;
        public ulong GuildId;
        public ulong ChannelId;
        public ulong CreatorId;

        public DatabaseGame(string id, string language, string lastLetter, string players, int type, long creatorId, long guildId, long channelId)
        {
            Id = id;
            Language = language;
            LastLetter = lastLetter;
            Type = type;
            Players = players.Split(";").Select(ulong.Parse).ToList();
            GuildId = (ulong)guildId;
            ChannelId = (ulong)channelId;
            CreatorId = (ulong)creatorId;
        }

        public void Update() => GameDatabaseHelper.InsertGame(this);
    }

    public static class GameDatabaseHelper
    {
        public static DatabaseGame GetGame(ulong id)
        {
            var reader = DatabaseHelper.ExecuteReader($"select * from games where id == {id}");
            reader.Read();
            var game = new DatabaseGame(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt64(5), reader.GetInt64(6), reader.GetInt64(7));
            reader.Close();
            return game;
        }

        public static IEnumerable<DatabaseGame> GetAllGames()
        {
            var reader = DatabaseHelper.ExecuteReader("select * from games");
            var list = new List<DatabaseGame>();
            while (reader.Read())
                list.Add(new DatabaseGame(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt64(5), reader.GetInt64(6), reader.GetInt64(7)));
            reader.Close();
            return list;
        }

        public static string GetUniqueId()
        {
            var ids = GetAllGames().Select(g => g.Id).Select(id => BitConverter.ToInt32(Convert.FromBase64String(id))).ToArray();
            var random = new Random((int) DateTime.UtcNow.Ticks);
            var id = random.Next(0, 1000001);
            while (ids.Contains(id))
                id = random.Next(0, 1000001);
            return Convert.ToBase64String(BitConverter.GetBytes(id));
        }

        public static void InsertGame(DatabaseGame game) => DatabaseHelper.ExecuteNonQuery($"replace into games values('{game.Id}', '{game.Language}', '{game.LastLetter}', '{string.Join(';', game.Players)}', {game.Type}, {game.CreatorId}, {game.GuildId}, {game.ChannelId})");
    }
}