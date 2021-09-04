using System.Drawing;
using DSharpPlus.Entities;

namespace Diswords.Bot
{
    public class EmbedHelper
    {
        public static DiscordEmbed SimpleEmbed(string description, string title = "") =>
            SimpleEmbed(DiscordColor.Gray, description, title);
        
        public static DiscordEmbed SimpleEmbed(DiscordColor color, string description, string title = "") =>
            new DiscordEmbedBuilder().WithColor(color).WithTitle(title).WithDescription(description).Build();

        public static DiscordEmbed ErrorEmbed(string description, string title = "") =>
            SimpleEmbed(DiscordColor.DarkRed, description, title);
    }
}