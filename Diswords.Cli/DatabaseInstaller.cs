using Diswords.Core;
using Diswords.DatabaseCreator;

namespace Diswords.Cli
{
    public static class DatabaseInstaller
    {
        public static void AutomaticInstall()
        {
            CreateDatabase.Call("database.db");
            foreach(var language in LanguageInfo.GetLanguages())
                ModifyLanguages.InstallLanguage(language);
        }

        public static void ManualInstall()
        {
            CreateDatabase.Call("database.db");
            ModifyDatabase.Call(null);
        }

        public static void Update() => UpdateDatabase.Call();
    }
}