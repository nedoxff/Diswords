using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Serilog;

namespace Diswords.Bot.Events
{
    public static class CommandErrored
    {
        public static async Task CommandsOnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Oh no!")
                .WithDescription(
                    $"An internal error occured while executing your command.\nPlease see the stacktrace, screenshot it, and report to the developer:\n```\n{e.Exception.Message}\n```")
                .WithThumbnail(
                    "https://media.discordapp.net/attachments/881149786949050418/882893073896591400/image.png")
                .WithColor(DiscordColor.DarkRed)
                .Build();
            await e.Context.Message.RespondAsync(embed);
            Log.Error(e.Exception.ToString());
        }
    }
}