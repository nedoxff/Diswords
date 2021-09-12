using System;
using System.Collections.Generic;
using System.Linq;
using Diswords.Bot.Game.Handlers;
using Diswords.Core.Databases;

namespace Diswords.Bot.Game
{
    public enum GameType
    {
        Normal    
    }

    public enum GameChannelType
    {
        Default,
        Room
    }

    public class GameInfo
    {
        public string LocaleName;
        public string Emoji;
        public string ShortName;
        public GameType Type;

        public GameInfo(string localeName, string emoji, string shortName, GameType type)
        {
            LocaleName = localeName;
            Emoji = emoji;
            ShortName = shortName;
            Type = type;
        }
    }

    public static class GameSettings
    {
        public static Dictionary<string, GameInfo> GameTypes = new()
        {
            {"normal", new GameInfo("GameType_Normal", "💬", "normal", GameType.Normal)}
        };

        public static Dictionary<string, GameHandler> Handlers = new();

        public static GameHandler GetHandler(GameType type, GameChannelType channelType, DatabaseGame game)
        {
            return type switch
            {
                GameType.Normal => new NormalGameHandler(game, channelType),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static GameType GetGameType(string shortName) => GameTypes.First(g => g.Key == shortName).Value.Type;
    }
}