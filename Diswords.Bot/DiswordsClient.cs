using System;
using System.Threading.Tasks;
using Diswords.Bot.Commands;
using Diswords.Core;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Diswords.Bot
{
    public static class DiswordsClient
    {
        private static DiscordClient _client;

        public static void Initialize(bool debug = false)
        {
            var loggerFactory = new LoggerFactory().AddSerilog();
            _client = new DiscordClient(new DiscordConfiguration
            {
                Token = Config.Token,
                LoggerFactory = loggerFactory,
                Intents = DiscordIntents.All,
                TokenType = TokenType.Bot,

#if DEBUG
                MinimumLogLevel = LogLevel.Debug,
#else
                if(debug)
                    MinimumLogLevel = LogLevel.Debug
                else
                    MinimumLogLevel = LogLevel.Information
#endif
            });

            Log.Information("Enabling interactivity..");
            _client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromSeconds(30),
                ButtonBehavior = ButtonPaginationBehavior.DeleteMessage,
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                PaginationDeletion = PaginationDeletion.DeleteMessage,
                PollBehaviour = PollBehaviour.KeepEmojis,
                ResponseBehavior = InteractionResponseBehavior.Respond,
                ResponseMessage = "Something wrong went when processing your interaction!"
            });

            Log.Information("Enabling slash commands..");
            var slashCommands = _client.UseSlashCommands();
            slashCommands.RegisterCommands<Ping>(764887344829956107);
        }

        public static void Start()
        {
            Task.Run(() =>
            {
                _client.ConnectAsync();
                Task.Delay(-1);
            });
        }
    }
}