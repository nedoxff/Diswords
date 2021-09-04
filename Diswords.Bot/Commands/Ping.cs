using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Diswords.Bot.Commands
{
    public class Ping : ApplicationCommandModule
    {
        [SlashCommand("ping", "Pong!")]
        public async Task PingCommand(InteractionContext ctx)
        {
            var beforeResponse = DateTime.UtcNow;

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                    .WithDescription("Calculating..")));

            var afterResponse = DateTime.UtcNow;
            var difference = (afterResponse - beforeResponse).Milliseconds;

            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle(DiscordEmoji.FromName(ctx.Client, ":ping_pong:") + "!")
                    .WithColor(DiscordColor.Orange)
                    .WithDescription(
                        $"Our client ping is: `{ctx.Client.Ping}ms`\nAnd the slash response ping is: `{difference}ms`")));
        }
    }
}