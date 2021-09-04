using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diswords.Core;
using Diswords.Core.Databases.Types;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Diswords.Bot.Commands
{
    public class Languages : BaseCommandModule
    {

        [Command("language")]
        public async Task SetCommand(CommandContext ctx)
        {
            var guildId = ctx.Guild.Id;

            var locale = Locale.Get(guildId);
            var error = locale["Error"];
            var timedOut = locale["TimedOut"];
            var selectLanguage = locale["SelectLanguage"];

            var builder = new DiscordMessageBuilder();
            builder.WithContent(DiscordEmoji.FromName(ctx.Client, ":point_down:"));
            builder.AddComponents(new DiscordSelectComponent("language", selectLanguage,
                GetLanguageDropdown()));
            var message = await ctx.RespondAsync(builder);
            var result = await message.WaitForSelectAsync(ctx.User, "language", TimeSpan.FromSeconds(30));
            if (result.TimedOut)
            {
                await message.DeleteAsync();
                await ctx.RespondAsync(EmbedHelper.ErrorEmbed(timedOut, error));
            }
            else
            {
                
                GuildDatabaseHelper.SetLanguage(ctx.Guild.Id, result.Result.Values[0]);
                locale = Locale.Get(guildId);
                var languageChanged = locale["LanguageChanged"];
                var success = locale["Success"];

                await message.DeleteAsync();

                await ctx.RespondAsync(new DiscordEmbedBuilder()
                    .WithTitle(success)
                    .WithDescription(languageChanged)
                    .WithColor(DiscordColor.SpringGreen)
                    .Build()
                );
            }
        }

        private static IEnumerable<DiscordSelectComponentOption> GetLanguageDropdown() => Locale.Locales.Values.Select(language => new DiscordSelectComponentOption($"{language.Name} ({language.NativeName})", language.ShortName, emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode(language.Flag)))).ToList();
    }
}