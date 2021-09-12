using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Diswords.Core.Databases;
using Serilog;

namespace Diswords.Core
{
    public class Locale
    {
        public static readonly Dictionary<string, Locale> Locales = new();
        public string Flag;
        public string Name;
        public string NativeName;
        public Dictionary<string, string> Properties = new();
        public string ShortName;

        public Locale()
        {
        }

        public Locale(string name, string shortName, string flag, string nativeName)
        {
            Name = name;
            ShortName = shortName;
            Flag = flag;
            NativeName = nativeName;
        }

        public string this[string key] => Properties[key];

        public static string Get(string language, string property)
        {
            return Locales[language][property];
        }

        public static string Get(ulong id, string property)
        {
            return Get(id)[property];
        }

        public static Locale Get(ulong id)
        {
            return Locales[GuildDatabaseHelper.GetLanguage(id)];
        }

        public static void Clear()
        {
            Locales.Clear();
        }
    }

    public class LocaleParser
    {
        public static void Load(string file)
        {
            Load(Path.GetFileNameWithoutExtension(file), File.ReadAllText(file));
        }

        public static void Load(string language, string data)
        {
            Log.Debug($"Parsing {language}..");
            var locale = new Locale();
            var dictionary = data.Split("\n").Where(line =>
                    !string.IsNullOrEmpty(line) && !string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                .Select(line => line.Split("=>")).ToDictionary(split => split[0].Trim(),
                    split => split[1].Trim().Replace("\\n", "\n"));
            foreach (var (key, value) in dictionary)
                switch (key)
                {
                    case "_Name":
                        locale.Name = value;
                        break;
                    case "_ShortName":
                        locale.ShortName = value;
                        break;
                    case "_Flag":
                        locale.Flag = value;
                        break;
                    case "_NativeName":
                        locale.NativeName = value;
                        break;
                    default:
                        locale.Properties[key] = value;
                        break;
                }

            Locale.Locales[language] = locale;
        }

        public static void Load()
        {
            if (!Directory.Exists("Locales"))
                throw new Exception("LocaleParser.Load() requires the Locales directory to exist!");

            Log.Information("Loading locales..");
            foreach (var file in Directory.GetFiles("Locales"))
                Load(file);
        }
    }
}