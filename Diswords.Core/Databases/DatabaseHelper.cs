using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Serilog;

namespace Diswords.Core.Databases
{
    public static class DatabaseHelper
    {
        public static SQLiteConnection Connection { get; private set; }

        public static void OpenConnectionFromFile(string file)
        {
            try
            {
                if (!File.Exists(file))
                    SQLiteConnection.CreateFile(file);
                Connection = new SQLiteConnection($"Data Source={file}");
                Connection.Open();
                Log.Information($"Successfully opened a SQLite connection with {file}!");
            }
            catch (Exception e)
            {
                Log.Fatal($"Failed to DatabaseHelper.OpenConnectionFromFile! Reason: {e}");
            }
        }

        public static int ExecuteNonQuery(string command)
        {
            var rowsAffected = CreateCommand(command).ExecuteNonQuery();
            if (rowsAffected >= 100)
                Log.Warning(
                    $"Command \"{command}\" affected more than 100 rows in the database! Are you sure you did nothing wrong?");
            return rowsAffected;
        }

        public static SQLiteDataReader ExecuteReader(string command)
        {
            return CreateCommand(command).ExecuteReader();
        }

        public static object ExecuteScalar(string command)
        {
            return CreateCommand(command).ExecuteScalar();
        }

        private static SQLiteCommand CreateCommand(string command)
        {
            if (Connection.State != ConnectionState.Open)
                throw new Exception("DatabaseHelper: Cannot create command while the connection is closed!");
            var query = Connection.CreateCommand();
            query.CommandText = command;
            return query;
        }
    }
}