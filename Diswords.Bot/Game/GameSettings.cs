using System.Collections.Generic;

namespace Diswords.Bot.Game
{
    public enum GameType
    {
        Normal    
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
    
    public class GameSettings
    {
        public static Dictionary<string, GameInfo> GameTypes = new()
        {
            {"normal", new GameInfo("GameType_Normal", "💬", "normal", GameType.Normal)}
        };
    }
}