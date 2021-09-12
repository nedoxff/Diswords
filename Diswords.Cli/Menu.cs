using System;
using Diswords.DatabaseCreator;
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
                        Exit();
                        break;
                    case "menu":
                        Console.Clear();
                        CallMenu();
                        break;
                }
            }
        }

        private static void CallMenu()
        {
            Console.WriteLine("Welcome to the Diswords Dashboard!\nPlease, select one of the following:");
            var choice = ConsoleUtils.WaitForChoice("Update the database", "Update the locales", "Exit");
            switch (choice)
            {
                case 1:
                    DatabaseInstaller.Update();
                    break;
                case 2:
                    LocaleInstaller.Update();
                    break;
                case 3:
                    Exit();
                    break;
            }

            Console.Clear();
            Console.WriteLine("Done!\n");
        }

        private static void Exit()
        {
            Log.Information("Shutting down the bot.. (manual)");
            Environment.Exit(0);
        }
    }
}