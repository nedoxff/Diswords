using System;
using Diswords.Core.Databases;

namespace Diswords.DatabaseCreator
{
    public class CreateDatabase
    {
        public static void Call()
        {
            Console.Clear();
            Console.WriteLine("This process is automatic, please wait..");

            Console.WriteLine("Creating guilds..");
            CreateGuilds();

            Console.WriteLine("Creating games..");
            CreateGames();

            Console.WriteLine("Done! Press any key to continue.");
            Console.ReadKey(true);
            ModifyDatabase.Call();
        }

        private static void CreateGuilds()
        {
            DatabaseHelper.ExecuteNonQuery(@"create table guilds
(
	id int not null
		constraint guilds_pk
			primary key,
	games_played int,
	default_language text default 'en'
);

create unique index guilds_id_uindex
	on guilds (id);");
        }

        private static void CreateGames()
        {
            DatabaseHelper.ExecuteNonQuery(@"create table games
(
	id text not null
		constraint games_pk
			primary key,
	language text not null,
	type int not null,
	guild_id int not null,
	channel_id int not null,
	players text not null,
	used_words text not null
);

create unique index games_channel_id_uindex
	on games (channel_id);

create unique index games_guild_id_uindex
	on games (guild_id);

create unique index games_id_uindex
	on games (id);");
        }
    }
}