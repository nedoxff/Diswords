using System;
using Serilog;

namespace Diswords.Cli
{
    public class Menu
    {
        public static void Start()
        {
            while (true)
            {
                var input = Console.ReadLine() ?? throw new ArgumentNullException();
                switch (input)
                {
                    case "exit":
                        Log.Information("Shutting down the bot.. (manual)");
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}