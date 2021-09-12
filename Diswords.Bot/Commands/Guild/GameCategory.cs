using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diswords.Core;
using Diswords.Core.Databases;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace Diswords.Bot.Commands.Guild
{
    public class GameCategory: BaseCommandModule
    {
        [Command("category")]
        public async Task Setategory(CommandContext ctx)
        {
            var locale = Locale.Get(ctx.Guild.Id);

            var error = locale["Error"];
            var timedOut = locale["TimedOut"];
            var success = locale["Success"];
            var category = locale["SelectCategory"];
            var categoryChanged = locale["CategoryChanged"];
            
            var builder = new DiscordMessageBuilder();
            builder.WithContent(DiscordEmoji.FromName(ctx.Client, ":point_down:"));
            builder.AddComponents(new DiscordSelectComponent("category", category,
                GetCategoryDropdown(ctx)));
            var message = await ctx.RespondAsync(builder);
            var result = await message.WaitForSelectAsync(ctx.User, "category", TimeSpan.FromSeconds(30));
            if (result.TimedOut)
            {
                await message.DeleteAsync();
                await ctx.RespondAsync(EmbedHelper.ErrorEmbed(timedOut, error));
            }
            else
            {
                var id = ulong.Parse(result.Result.Values[0]);
                var name = ctx.Guild.Channels.FirstOrDefault(c => c.Value.IsCategory && c.Key == id).Value;
                
                GuildDatabaseHelper.SetParentGameCategory(ctx.Guild.Id, id);
                
                await message.DeleteAsync();

                await ctx.RespondAsync(new DiscordEmbedBuilder()
                    .WithTitle(success)
                    .WithDescription(string.Format(categoryChanged, name.Name))
                    .WithColor(DiscordColor.SpringGreen)
                    .Build()
                );
            }
        }

        private static IEnumerable<DiscordSelectComponentOption> GetCategoryDropdown(CommandContext ctx) => ctx.Guild.Channels.Where(c => c.Value.IsCategory).Select(c => new DiscordSelectComponentOption(c.Value.Name, c.Key.ToString(), emoji: new DiscordComponentEmoji(ChatBoxEmoji)));

        private static DiscordEmoji ChatBoxEmoji => DiscordEmoji.FromUnicode("💬");
    }
}