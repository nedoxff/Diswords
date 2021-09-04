using System;
using System.IO;
using Diswords.Core.Databases;
using Serilog;

namespace Diswords.DatabaseCreator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            InitializeLogger();
            Menu();
        }

        private static void Menu()
        {
            Console.Clear();
            Console.WriteLine(
                "Welcome to DiswordsDatabaseCreator!\nWith this tool you can create template tables, and add new languages!\nPlease, choose one of the following:");
            var createOrModify =
                ConsoleUtils.WaitForChoice("I want to create a new database.", "I want to edit an existing database.",
                    "I accidentally started this program (exit).");
            switch (createOrModify)
            {
                case 1:
                {
                    var path = ConsoleUtils.WaitForInput("Please, enter the path of the database: ",
                        s => !File.Exists(s));
                    DatabaseHelper.OpenConnectionFromFile(path);
                    CreateDatabase.Call();
                    break;
                }
                case 2:
                {
                    var path = ConsoleUtils.WaitForInput("Please, enter the path of the database: ", File.Exists);
                    DatabaseHelper.OpenConnectionFromFile(path);
                    ModifyDatabase.Call();
                    break;
                }
                case 3:
                {
                    Console.WriteLine("No worries! Thanks for using DiswordsDatabaseCreator anyway.");
                    break;
                }
            }
        }

        private static void InitializeLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}