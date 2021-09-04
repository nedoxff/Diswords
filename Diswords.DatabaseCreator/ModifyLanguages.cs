using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using Diswords.Core.Databases;

namespace Diswords.DatabaseCreator
{
    public class ModifyLanguages
    {
        public static void Push()
        {
            Console.Clear();

            Console.Write("Checking languages from GitHub.. ");
            var githubLanguages = GetLanguages();
            Console.WriteLine($"found {githubLanguages.Length} languages.");

            Console.Write("Checking languages in the database.. ");
            var tables = GetDatabaseTables();

            var databaseLanguages = githubLanguages.Where(l => tables.Contains(l)).ToArray();
            var availableLanguages = githubLanguages.Where(l => !tables.Contains(l)).ToArray();

            Console.WriteLine($"found {databaseLanguages.Length} languages.");

            if (availableLanguages.Length == 0)
            {
                Console.WriteLine("No languages to add.");
                return;
            }

            Console.WriteLine("Please, select a language you want to add:");

            var index = ConsoleUtils.WaitForChoice(availableLanguages) - 1;
            var language = availableLanguages[index];

            var archive = $"{language}.zip";
            var dictionaryFile = $"{language}.txt";

            if (File.Exists(archive))
                File.Delete(archive);
            if (File.Exists(dictionaryFile))
                File.Delete(dictionaryFile);

            DownloadFile(GetLanguageUrl(language), archive);

            Console.WriteLine("\nExtracting the dictionary..");

            ZipFile.ExtractToDirectory(archive, Environment.CurrentDirectory);
            File.Delete(archive);

            Console.WriteLine("Creating the language table..");
            CreateLanguageTable(language);

            Console.WriteLine("Pushing to the database..");
            PushFile(language, dictionaryFile);
            File.Delete(dictionaryFile);

            Console.WriteLine("Done!");
        }

        public static void Remove()
        {
            Console.Clear();

            Console.Write("Checking languages from GitHub.. ");
            var githubLanguages = GetLanguages();
            Console.WriteLine($"found {githubLanguages.Length} languages.");

            Console.Write("Checking languages in the database.. ");
            var tables = GetDatabaseTables();

            var databaseLanguages = githubLanguages.Where(l => tables.Contains(l)).ToArray();

            Console.WriteLine($"found {databaseLanguages.Length} languages.");

            if (databaseLanguages.Length == 0)
            {
                Console.WriteLine("No languages to remove.");
                return;
            }

            Console.WriteLine("Please, select a language you want to remove:");

            var index = ConsoleUtils.WaitForChoice(databaseLanguages) - 1;
            var language = databaseLanguages[index];

            RemoveLanguage(language);
            Console.WriteLine("Done!");
        }

        private static void PushFile(string language, string file)
        {
            using var transaction = DatabaseHelper.Connection.BeginTransaction();
            var command = DatabaseHelper.Connection.CreateCommand();
            command.CommandText =
                $@"insert into {language} values($word)";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "$word";
            command.Parameters.Add(parameter);
            var lines = File.ReadAllLines(file).Select(s => s.ToLower()).Distinct().ToArray();
            foreach (var line in lines)
            {
                parameter.Value = line;
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        private static string[] GetDatabaseTables()
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

        private static void DownloadFile(string url, string to)
        {
            Console.WriteLine($"Downloading {to} from {url}..");
            var wc = new WebClient();
            var finished = false;

            wc.DownloadProgressChanged += (_, args) =>
                Console.WriteLine(
                    $"{args.BytesReceived}/{args.TotalBytesToReceive} bytes received | {args.ProgressPercentage}%");
            wc.DownloadFileCompleted += (_, _) =>
            {
                Console.WriteLine("Done.");
                finished = true;
            };
            wc.DownloadFileAsync(new Uri(url), to);

            while (!finished) Thread.Sleep(1);
        }

        private static void CreateLanguageTable(string language)
        {
            DatabaseHelper.ExecuteNonQuery($@"create table {language}
(
	word text not null
		constraint {language}_pk
			primary key
);

create unique index {language}_word_uindex
	on {language} (word);");
        }

        private static void RemoveLanguage(string language)
        {
            DatabaseHelper.ExecuteNonQuery($"drop table {language}");
        }

        private static string GetLanguageUrl(string language)
        {
            return $"https://github.com/NedoProgrammer/DiswordsResources/blob/main/{language}.zip?raw=true";
        }

        private static string[] GetLanguages()
        {
            return new WebClient()
                .DownloadString("https://raw.githubusercontent.com/NedoProgrammer/DiswordsResources/main/languages.txt")
                .Trim().Split("\n");
        }
    }
}