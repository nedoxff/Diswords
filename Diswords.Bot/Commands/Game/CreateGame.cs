#nullable enable
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Diswords.Bot.Game;
using Diswords.Core;
using Diswords.Core.Databases;
using Diswords.Drawer;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace Diswords.Bot.Commands.Game
{
    [Group("game")]
    public partial class GameControls: BaseCommandModule
    {
        [Command("create")]
        public async Task CreateGameCommand(CommandContext ctx)
        {
            var locale = Locale.Get(ctx.Guild.Id);
            
            var gameType = await GetGameType(locale, ctx);

            if (gameType == null)
                return;

            var language = await GetLanguage(locale, ctx);

            if (language == null)
                return;
            
            var roomOrChannel = await GetRoomOrChannel(locale, ctx); 
            
            if (roomOrChannel == null)
                return;
            
            var creatingGame = locale["GameCreate_CreatingGame"];
            var generatingId = locale["GameCreate_GeneratingId"];
            switch (roomOrChannel)
            {
                case "room":
                {
                    var privateRoom = await GetPrivateRoom(locale, ctx);

                    if (privateRoom == null)
                        return;

                    var creatingChannel = locale["GameCreate_CreatingChannel"];
                    var generatingImage = locale["GameCreate_GeneratingImage"];

                    var message = await SendLoadingMessage(locale, ctx);
                    
                    await message.ModifyAsync(EmbedHelper.SimpleEmbed(generatingId));
                    var id = GameDatabaseHelper.GetUniqueId();

                    await message.ModifyAsync(EmbedHelper.SimpleEmbed(creatingChannel));
                    var categoryId = GuildDatabaseHelper.GetParentGameCategory(ctx.Guild.Id);
                    var category = categoryId == 0 ? null : ctx.Guild.GetChannel((ulong)categoryId);
                    var channel = await ctx.Guild.CreateChannelAsync(id, ChannelType.Text, category, "Playing Diswords!");
                    await channel.AddOverwriteAsync(ctx.Guild.EveryoneRole, Permissions.None, Permissions.All);

                    await message.ModifyAsync(EmbedHelper.SimpleEmbed(creatingGame));
                    var databaseGame = new DatabaseGame(id, language, "", ctx.User.Id.ToString(), (int)gameType.Type, (long)ctx.User.Id, (long)ctx.Guild.Id, (long)channel.Id);
                    GameDatabaseHelper.InsertGame(databaseGame);
                    var handler = GameSettings.GetHandler(gameType.Type, GameChannelType.Room, databaseGame);
                    GameSettings.Handlers[handler.Id] = handler;
                    await message.ModifyAsync(EmbedHelper.SimpleEmbed(generatingImage));
                    var image = NewRoomDrawer.Generate("", id, false);
                    await ctx.RespondAsync(new DiscordMessageBuilder()
                        .WithFile("new_game.png", image));
                    
                    await message.DeleteAsync();
                    handler.Setup();
                    break;
                }
                case "channel":
                {
                    var message = await SendLoadingMessage(locale, ctx);
                    await message.ModifyAsync(EmbedHelper.SimpleEmbed(generatingId));
                    var id = GameDatabaseHelper.GetUniqueId();
                    
                    await message.ModifyAsync(EmbedHelper.SimpleEmbed(creatingGame));
                    var databaseGame = new DatabaseGame(id, language, "", ctx.User.Id.ToString(), (int)gameType.Type, (long)ctx.User.Id, (long)ctx.Guild.Id, (long)ctx.Channel.Id);
                    GameDatabaseHelper.InsertGame(databaseGame);

                    await message.DeleteAsync();
                    break;
                }
            }

            await ctx.RespondAsync(DiscordEmoji.FromUnicode("✅"));
        }

        private static async Task<string?> GetRoomOrChannel(Locale locale, CommandContext ctx)
        {
            var roomOrChannel = locale["GameCreate_RoomOrChannel"];
            var room = locale["GameCreate_Room"];
            var channel = locale["GameCreate_Channel"];

            var builder = new DiscordMessageBuilder()
                .WithEmbed(EmbedHelper.SimpleEmbed(roomOrChannel));
            builder.AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "room", room), new DiscordButtonComponent(ButtonStyle.Primary, "channel", channel));
            var message = await ctx.RespondAsync(builder);
            var result = await message.WaitForButtonAsync(ctx.User, TimeSpan.FromSeconds(30));
            await message.DeleteAsync();
            if (!result.TimedOut) return result.Result.Id;
            var timedOut = locale["GameCreate_TimedOut"];
            await message.ModifyAsync(EmbedHelper.ErrorEmbed(timedOut));
            return null;

        }

        private static async Task<DiscordMessage> SendLoadingMessage(Locale locale, CommandContext ctx)
        {
            var giveMeAMoment = locale["GiveMeAMoment"];
            return await ctx.RespondAsync(EmbedHelper.SimpleEmbed(giveMeAMoment));
        }

        private static async Task<bool?> GetPrivateRoom(Locale locale, CommandContext ctx)
        {
            var privateRoom = locale["GameCreate_PrivateRoom"];
            var yes = locale["Yes"];
            var no = locale["No"];

            var builder = new DiscordMessageBuilder()
                .WithEmbed(EmbedHelper.SimpleEmbed(privateRoom));
            builder.AddComponents(new DiscordButtonComponent(ButtonStyle.Success, "yes", yes),
                new DiscordButtonComponent(ButtonStyle.Danger, "no", no));
            var message = await ctx.RespondAsync(builder);
            var result = await message.WaitForButtonAsync(ctx.User, TimeSpan.FromSeconds(30));
            await message.DeleteAsync();
            if (!result.TimedOut) return result.Result.Id == "yes";
            var timedOut = locale["GameCreate_TimedOut"];
            await message.ModifyAsync(EmbedHelper.ErrorEmbed(timedOut));
            return null;
        }

        private static async Task<GameInfo?> GetGameType(Locale locale, CommandContext ctx) => await WaitForDropdown(locale, ctx, "SelectGameType", () => GetGameTypeDropdown(locale),
            s => GameSettings.GameTypes.FirstOrDefault(i => i.Value.ShortName == s).Value);

        private static async Task<string?> GetLanguage(Locale locale, CommandContext ctx) =>
            await WaitForDropdown(locale, ctx, "SelectLanguage", GetLanguageDropdown, s => s);
        
        private static async Task<T?> WaitForDropdown<T>(Locale locale, CommandContext ctx, string placeholder, Func<IEnumerable<DiscordSelectComponentOption>> elements, Func<string, T> returnValue) where T: class
        {
            var placeholderText = locale[placeholder];
            var error = locale["Error"];
            var timedOut = locale["TimedOut"];
            
            var builder = new DiscordMessageBuilder();
            builder.WithContent(DiscordEmoji.FromName(ctx.Client, ":point_down:"));
            builder.AddComponents(new DiscordSelectComponent("dropdown", placeholderText,
                elements()));
            var message = await ctx.RespondAsync(builder);
            var result = await message.WaitForSelectAsync(ctx.User, "dropdown", TimeSpan.FromSeconds(30));
            await message.DeleteAsync();
            if (!result.TimedOut)
                return returnValue(result.Result.Values[0]);
            await message.DeleteAsync();
            await ctx.RespondAsync(EmbedHelper.ErrorEmbed(timedOut, error));
            return null;
        }
        private static IEnumerable<DiscordSelectComponentOption> GetGameTypeDropdown(Locale locale) => GameSettings.GameTypes.Values.Select(info => new DiscordSelectComponentOption($"{locale[info.LocaleName]}", info.ShortName, emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode(info.Emoji)))).ToList();
        private static IEnumerable<DiscordSelectComponentOption> GetLanguageDropdown()
        {
            return LanguageInfo.GetRawLanguageInfo().Item1.Select(info =>
            {
                var locale = Locale.Locales[info];
                return new DiscordSelectComponentOption($"{locale.Name} ({locale.NativeName})", locale.ShortName,
                    emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode(locale.Flag)));
            }).ToList();
        }
    }
}