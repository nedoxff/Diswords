using System;
using System.IO;
using System.Linq;
using System.Threading;
using CommandLine;
using Diswords.Bot;
using Diswords.Core;
using Diswords.Core.Databases;
using Serilog;

namespace Diswords.Cli
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var arguments = ParseArguments(ref args);

            Console.Clear();
            InitializeLogger(arguments.Debug);
            Config.LoadFromFile(arguments.ConfigFile);

            if (!File.Exists("database.db"))
            {
                Log.Information("It seems like you're running the bot for first time, we'll call the installer in 5 seconds..");
                Thread.Sleep(5000);
                if(arguments.ManualDatabaseInstall) DatabaseInstaller.ManualInstall();
                else DatabaseInstaller.AutomaticInstall();
                Console.Clear();
            }
            else DatabaseHelper.OpenConnectionFromFile("database.db");

            if (!Directory.Exists("Locales"))
            {
                LocaleInstaller.Call();
                Console.Clear();    
            }
            else
                LocaleParser.Load();
            
            if (!Directory.Exists("Resources"))
            {
                ResourcesInstaller.Call();
                Console.Clear();    
            }
            else
                ResourceContainer.Load();
            

            DiswordsClient.Initialize(arguments.Debug);

            DiswordsClient.Start();
            Menu.Start();
        }

        private static bool ContainsArgument(ref string[] args, string argument)
        {
            return args.Any(a => a.ToLower().Trim() == argument.ToLower().Trim());
        }

        private static CommandLineOptions ParseArguments(ref string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args);
            if (!parseResult.Errors.Any()) return parseResult.Value;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[FATAL] Failed to parse CLI arguments.");
            Console.ResetColor();

            Environment.Exit(1);

            return parseResult.Value;
        }

        private static void InitializeLogger(bool debug = false)
        {
            File.WriteAllText("log_latest.txt", "");
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                if(debug)
                    .MinimumLevel.Debug()
                else
                    .MinimumLevel.Information()
#endif
                .WriteTo.Console()
                .WriteTo.File("log_latest.txt", shared: true)
                .CreateLogger();

            Log.Information("Logger successfully initialized!");
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class CommandLineOptions
        {
            [Option('c', "config", Required = false, Default = "config.json",
                HelpText =
                    "If you have a config file not in the executable directory, you can specify this argument and the bot will use that file as the config.")]
            public string ConfigFile { get; set; }

            [Option('d', "debug", Required = false, Default = false,
                HelpText = "Want to see more output? Specify this argument.")]
            public bool Debug { get; set; }
            
            [Option('m', "manual-install", Required = false, Default = false, HelpText = "Don't want to have troubles with databases? Specify this argument.")]
            public bool ManualDatabaseInstall { get; set; }
        }
    }
}