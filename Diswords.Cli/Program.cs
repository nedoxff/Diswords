using System;
using System.IO;
using System.Linq;
using CommandLine;
using Diswords.Bot;
using Diswords.Core;
using Serilog;

namespace Diswords.Cli
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var arguments = ParseArguments(ref args);

            InitializeLogger(arguments.Debug);
            Config.LoadFromFile(arguments.ConfigFile);

            //if(!File.Exists("database.db"))
            //ResourcesCli.InstallMissingResources();

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

        private class CommandLineOptions
        {
            [Option('c', "config", Required = false, Default = "config.json",
                HelpText =
                    "If you have a config file not in the executable directory, you can specify this argument and the bot will use that file as the config.")]
            public string ConfigFile { get; set; }

            [Option('d', "debug", Required = false, Default = false,
                HelpText = "Want to see more output? Specify this argument.")]
            public bool Debug { get; set; }
        }
    }
}