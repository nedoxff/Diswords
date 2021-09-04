namespace Diswords.Core.Databases.Types
{
    public class DatabaseGuild
    {
        public ulong Id;
        public uint GamesPlayed;
        public string Language;

        public DatabaseGuild(long id, int gamesPlayed, string language)
        {
            Id = (ulong)id;
            GamesPlayed = (uint)gamesPlayed;
            Language = language;
        }
    }
}