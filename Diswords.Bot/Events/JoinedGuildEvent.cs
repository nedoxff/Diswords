using System.Threading.Tasks;
using Diswords.Core;
using Diswords.Core.Databases;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Diswords.Bot.Events
{
    public static class JoinedGuildEvent
    {
        public static Task OnGuildCreated(DiscordClient sender, GuildCreateEventArgs e)
        {
            Task.Run(async () =>
            {
                var guild = e.Guild;
                var channel = guild.SystemChannel ?? guild.GetDefaultChannel();

                var text = Locale.Get("en", "GuildJoinedText");

                var embed = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Orange)
                    .WithDescription(text)
                    .WithTitle("Hello!")
                    .Build();

                var message = await channel.SendMessageAsync(embed);

                var databaseGuild = new DatabaseGuild(guild.Id, 0, 0, "en");
                GuildDatabaseHelper.InsertGuild(databaseGuild);

                text = Locale.Get("en", "GuildJoinedFinish");
                embed = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Green)
                    .WithDescription(text)
                    .WithTitle("Done!")
                    .Build();
                await message.ModifyAsync(embed);
            });
            return Task.CompletedTask;
        }
    }
}