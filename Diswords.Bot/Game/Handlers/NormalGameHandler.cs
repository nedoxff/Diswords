using Diswords.Core.Databases;
using DSharpPlus.Entities;

namespace Diswords.Bot.Game.Handlers
{
    public class NormalGameHandler : GameHandler
    {
        public NormalGameHandler(DatabaseGame game, GameChannelType type) : base(game, type)
        {
        }

        public override void Start()
        {
        }


        //TODO: The fast mode is not yet done, but this will be left for future review.
        /*
        await message.PinAsync();
        var voteToStart = Locale["Room_VoteToStart"];
        var vote = await GameChannel.SendMessageAsync(EmbedHelper.SimpleEmbed(DiscordColor.Orange,
            voteToStart));
        var voteResult = await vote.DoPollAsync(new[] {DiscordEmoji.FromUnicode("✅")}, PollBehaviour.KeepEmojis,
            TimeSpan.FromMinutes(5));
        */


        public override async void Setup()
        {
            var firstMessageTitle = Locale["Room_FirstMessage_Title"];
            var firstMessage = Locale["Room_FirstMessage"];
            var message =
                await GameChannel.SendMessageAsync(EmbedHelper.SimpleEmbed(DiscordColor.SpringGreen, firstMessage,
                    firstMessageTitle));
            if (GameChannelType == GameChannelType.Room) await message.PinAsync();

            if (GameType == GameType.Normal)
                Start();
        }

        public override void NewWord(DiscordUser user, string word)
        {
        }

        public override void Stop()
        {
        }

        public override void CleanUp()
        {
        }

        public override void OnPlayerJoin(DiscordUser user)
        {
        }

        public override void OnPlayerLeave(DiscordUser user)
        {
        }
    }
}