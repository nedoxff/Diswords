using System;
using Diswords.Core.Databases;

namespace Diswords.DatabaseCreator
{
    public class ModifyDatabase
    {
        public static void Call()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the menu!\nPlease, select one of the following:");
                var choice = ConsoleUtils.WaitForChoice("Push a new language", "Remove a language", "Clear guild table",
                    "Clear games table", "Exit");
                switch (choice)
                {
                    case 1:
                        ModifyLanguages.Push();
                        break;
                    case 2:
                        ModifyLanguages.Remove();
                        break;
                    case 3:
                        Console.Write("Please wait.. ");
                        ClearGuilds();
                        Console.WriteLine("Done");
                        break;
                    case 4:
                        Console.Write("Please wait.. ");
                        ClearGames();
                        Console.WriteLine("Done");
                        break;
                    case 5:
                        Console.WriteLine("Thank you for using DiswordsDatabaseCreator!");
                        Environment.Exit(0);
                        break;
                }

                Console.WriteLine("Press any key to continue..");
                Console.ReadKey(true);
            }
        }

        private static void ClearGuilds()
        {
            DatabaseHelper.ExecuteNonQuery("delete from guilds");
        }

        private static void ClearGames()
        {
            DatabaseHelper.ExecuteNonQuery("delete from games");
        }
    }
}