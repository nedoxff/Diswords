using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Diswords.Core.Databases;

namespace Diswords.Core
{
    public class LanguageInfo
    {
        public static (string[], string[]) GetLanguageInfo()
        {
            Console.Write("Checking languages from GitHub.. ");
            var githubLanguages = GetLanguages();
            Console.WriteLine($"found {githubLanguages.Length} languages.");

            Console.Write("Checking languages in the database.. ");
            var tables = GetDatabaseTables();

            var databaseLanguages = githubLanguages.Where(l => tables.Contains(l)).ToArray();
            var availableLanguages = githubLanguages.Where(l => !tables.Contains(l)).ToArray();

            Console.WriteLine($"found {databaseLanguages.Length} languages.");

            return (databaseLanguages, availableLanguages);
        }

        public static (string[], string[]) GetRawLanguageInfo()
        {
            var githubLanguages = GetLanguages();
            var tables = GetDatabaseTables();

            var databaseLanguages = githubLanguages.Where(l => tables.Contains(l)).ToArray();
            var availableLanguages = githubLanguages.Where(l => !tables.Contains(l)).ToArray();

            return (databaseLanguages, availableLanguages);
        }

        public static string[] GetDatabaseTables()
        {
            var list = new List<string>();
            var reader =
                DatabaseHelper.ExecuteReader(
                    "select name from sqlite_master where type='table' and name not like 'sqlite_%'");
            while (reader.Read())
                list.Add(reader.GetString(0));
            reader.Close();
            return list.ToArray();
        }

        public static string[] GetLanguages()
        {
            return new WebClient()
                .DownloadString("https://raw.githubusercontent.com/NedoProgrammer/DiswordsResources/main/languages.txt")
                .Trim().Split("\n");
        }
    }
}