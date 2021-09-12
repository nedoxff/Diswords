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
    public class ResourcesInstaller
    {
        public static void Call()
        {
            Console.Clear();
            Log.Information(
                "It seems like you don't have all of the resources installed.\nThis process is automatic, don't worry.\nWe'll start in 5 seconds..");
            Thread.Sleep(5000);
            Update();
        }

        public static void Update()
        {
            Console.Clear();
            Log.Information("Updating resources in progress..");
            Log.Debug("Backing up resources..");

            if (Directory.Exists("Resources")) Directory.Move("Resources", "Resources_old");
            Directory.CreateDirectory("Resources");

            try
            {
                foreach (var file in GetFiles())
                {
                    var path = $"Resources/{file}";
                    FileDownloader.Download(GetFileUrl(file), path);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to update resources! Reverting.. Reason: {e}");

                if (Directory.Exists("Resources_old"))
                    Directory.Move("Resources_old", "Resources");
            }
            finally
            {
                ResourceContainer.Load();
            }
        }

        private static IEnumerable<string> GetFiles()
        {
            return new WebClient()
                .DownloadString("https://raw.githubusercontent.com/NedoProgrammer/DiswordsResources/main/resources.txt")
                .Split("\n")
                .Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim());
        }

        private static string GetFileUrl(string file)
        {
            return $"https://github.com/NedoProgrammer/DiswordsResources/blob/main/Resources/{file}?raw=true";
        }
    }
}