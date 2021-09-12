using System;
using System.Threading.Tasks;
using Diswords.Core;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Diswords.Bot.Commands
{
    public class Ping : BaseCommandModule
    {
        [Command("ping")]
        public async Task PingCommand(CommandContext ctx)
        {
            var beforeResponse = DateTime.UtcNow;
            var locale = Locale.Get(ctx.Guild.Id);
            var calculatingString = locale["Calculating"];
            var embed = new DiscordEmbedBuilder()
                .WithDescription(calculatingString)
                .Build();
            var message = await ctx.RespondAsync(embed);

            var afterResponse = DateTime.UtcNow;
            var difference = (afterResponse - beforeResponse).Milliseconds;

            var formatted = string.Format(locale["PingMessage"], ctx.Client.Ping, difference);

            embed = new DiscordEmbedBuilder()
                .WithTitle(DiscordEmoji.FromName(ctx.Client, ":ping_pong:") + "!")
                .WithColor(DiscordColor.Orange)
                .WithDescription(
                    formatted)
                .Build();
            await message.ModifyAsync(embed);
        }
    }
}