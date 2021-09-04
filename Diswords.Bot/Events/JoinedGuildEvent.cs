using System;
using System.Threading.Tasks;
using Diswords.Core;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Diswords.Bot.Events
{
    public class JoinedGuild
    {
        public static Task OnGuildCreated(DiscordClient sender, GuildCreateEventArgs e)
        {
            var guild = e.Guild;
            var channel = guild.SystemChannel ?? guild.GetDefaultChannel();
            var title = Locale.Get(guild.Id, "GuildJoinedTitle");
            var embed = new DiscordEmbedBuilder()
                .Build();
        }
    }
}