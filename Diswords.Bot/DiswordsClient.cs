using System;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using Diswords.Bot.Commands;
using Diswords.Bot.Events;
using Diswords.Core;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
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
                ButtonBehavior = ButtonPaginationBehavior.DeleteButtons,
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                PollBehaviour = PollBehaviour.KeepEmojis,
                ResponseBehavior = InteractionResponseBehavior.Respond,
                ResponseMessage = "Something wrong went when processing your interaction!"
            });

            Log.Information("Enabling commands..");
            var commands = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDms = false,
                StringPrefixes = new [] {"$", "dw."},
                EnableMentionPrefix = true,
                IgnoreExtraArguments = false,
                 EnableDefaultHelp = true,
                 CaseSensitive = false
            });
            commands.RegisterCommands(Assembly.GetExecutingAssembly());
            commands.CommandErrored += CommandErrored.CommandsOnCommandErrored;

            Log.Information("Attaching events..");
            AttachEvents();
        }

        

        public static void Start()
        {
            Task.Run(() =>
            {
                _client.ConnectAsync();
                Task.Delay(-1);
            });
        }

        public static void AttachEvents()
        {
            _client.GuildCreated += JoinedGuildEvent.OnGuildCreated;
        }
    }
}