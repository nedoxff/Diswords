using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Diswords.Core;
using Serilog;

namespace Diswords.Cli
{
    public class LocaleInstaller
    {
        public static void Call()
        {
            Console.Clear();
            Log.Information(
                "It seems like you don't have the locales installed.\nThis process is automatic, don't worry.\nWe'll start in 5 seconds..");
            Thread.Sleep(5000);

            Update();
        }

        public static void InstallAll()
        {
            foreach (var language in GetLocales())
            {
                var path = $"Locales/{language}.locale";
                Log.Debug($"Downloading & loading {language}.. ");
                FileDownloader.Download(GetLocaleUrl(language), path);
                LocaleParser.Load(path);
            }
        }

        public static void Update()
        {
            Console.Clear();
            Log.Information("Updating locales in progress..");
            Log.Debug("Backing up locales..");

            if (Directory.Exists("Locales")) Directory.Move("Locales", "Locales_old");
            Directory.CreateDirectory("Locales");

            try
            {
                Locale.Clear();
                InstallAll();
            }
            catch (Exception e)
            {
                Log.Error($"Failed to update locales! Reverting.. Reason: {e}");

                if (Directory.Exists("Locales_old"))
                    Directory.Move("Locales_old", "Locales");
            }
            finally
            {
                LocaleParser.Load();
            }

            if (Directory.Exists("Locales_old"))
                Directory.Delete("Locales_old", true);
        }

        private static IEnumerable<string> GetLocales()
        {
            return new WebClient()
                .DownloadString("https://raw.githubusercontent.com/NedoProgrammer/DiswordsResources/main/locales.txt")
                .Split("\n").Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).ToArray();
        }

        private static string GetLocaleUrl(string language)
        {
            return $"https://raw.githubusercontent.com/NedoProgrammer/DiswordsResources/main/Locales/{language}.locale";
        }
    }
}