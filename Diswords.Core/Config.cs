using System;
using System.IO;
using Newtonsoft.Json;
using Serilog;

namespace Diswords.Core
{
    public static class Config
    {
        public static string Token;

        public static void LoadFromFile(string file)
        {
            try
            {
                if (!File.Exists(file))
                {
                    Log.Fatal("Your config file doesn't exist!");
                    Environment.Exit(1);
                }

                var parsed = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(file));
                if (parsed == null)
                {
                    Log.Fatal("Failed to parse JSON config!");
                    Environment.Exit(1);
                }

                Token = parsed.Token;
            }
            catch (Exception e)
            {
                Log.Fatal($"Failed to Config.LoadFromFile! Reason: {e}");
            }
        }
    }
}