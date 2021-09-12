using Diswords.Core;
using Diswords.Core.Databases;
using DSharpPlus.Entities;

namespace Diswords.Bot.Game
{
    public abstract class GameHandler
    {
        public DatabaseGame InternalGame;
        public GameChannelType GameChannelType;
        public GameType GameType;
        public DiscordChannel GameChannel;
        public Locale Locale;
        public string Id;

        public GameHandler(DatabaseGame game, GameChannelType type)
        {
            Id = game.Id;
            GameChannelType = type;
            GameType = (GameType)game.Type;
            InternalGame = game;
            GameChannel = DiswordsClient.GetGuild(game.GuildId).GetChannel(game.ChannelId);
            Locale = Locale.Get(game.GuildId);
        }

        public abstract void Start();
        public abstract void Setup();
        public abstract void NewWord(DiscordUser user, string word);
        public abstract void Stop();
        public abstract void CleanUp();
        public abstract void OnPlayerJoin(DiscordUser user);
        public abstract void OnPlayerLeave(DiscordUser user);
    }
}