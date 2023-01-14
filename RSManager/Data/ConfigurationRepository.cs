using Microsoft.Data.Sqlite;
using RSManager.Data.Base;
using RSManager.Models.Dto;
using RSManager.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Data
{
    internal class ConfigurationRepository : SqLiteRepository, IConfigurationRepository
    {
        public ConfigurationRepository() : base("Configuration")
        {
        }

        protected override List<string> CreateDefaultTables()
        {
            string sql = $@"CREATE TABLE {DbName} 
                          (Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                           Uri varchar(500), 
                           User varchar(100), 
                           Password text, 
                           Domain varchar(100),
                           RememberPassword bit)";

            return new List<string>() { sql };
        }

        public void AddConnectionInfo(ConnectionDto connection)
        {
            using (var sqliteConnection = GetConnection())
            {
                sqliteConnection.Open();

                string sql = $@"INSERT INTO {DbName} (Uri,User,Password,Domain,RememberPassword)
                                VALUES (@uri,@user,@pass,@domain,@rememberPass) ";

                SqliteCommand command = new SqliteCommand(sql, sqliteConnection);
                command.Parameters.AddWithValue("@uri", (object)connection.Uri ?? DBNull.Value);
                command.Parameters.AddWithValue("@user", (object)connection.User ?? DBNull.Value);
                command.Parameters.AddWithValue("@pass", (object)connection.Password ?? DBNull.Value);
                command.Parameters.AddWithValue("@domain", (object)connection.Domain ?? DBNull.Value);
                command.Parameters.AddWithValue("@rememberPass", (object)connection.RememberPassword ?? false);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteConnectionInfo(int id)
        {
            throw new NotImplementedException();
        }

        public ConnectionDto GetConnectionInfo()
        {
            using (var sqliteConnection = GetConnection())
            {
                sqliteConnection.Open();

                string sql = $@"SELECT Id,Uri,User,Password,Domain, RememberPassword 
                                FROM {DbName} ";

                SqliteCommand command = new SqliteCommand(sql, sqliteConnection);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string uri = reader.IsDBNull(reader.GetOrdinal("Uri")) ? null : reader.GetString(reader.GetOrdinal("Uri"));
                    string user = reader.IsDBNull(reader.GetOrdinal("User")) ? null : reader.GetString(reader.GetOrdinal("User"));
                    string pass = reader.IsDBNull(reader.GetOrdinal("Password")) ? null : reader.GetString(reader.GetOrdinal("Password"));
                    string domain = reader.IsDBNull(reader.GetOrdinal("Domain")) ? null : reader.GetString(reader.GetOrdinal("Domain"));
                    bool rememberPassword = !reader.IsDBNull(reader.GetOrdinal("RememberPassword")) && reader.GetBoolean(reader.GetOrdinal("RememberPassword"));

                    ConnectionDto result = new ConnectionDto(uri,user,pass,domain, rememberPassword);

                    return result;
                }

                return null;
            }
        }

        public void UpdateConnectionInfo(int id, ConnectionDto connection)
        {
            using (var sqliteConnection = GetConnection())
            {
                sqliteConnection.Open();

                string sql = $@"UPDATE {DbName} 
                             SET Uri = @uri,
                                 User = @user,
                                 Password = @pass,
                                 Domain = @domain,
                                 RememberPassword = @rememberPass";

                SqliteCommand command = new SqliteCommand(sql, sqliteConnection);
                command.Parameters.AddWithValue("@uri", (object)connection.Uri ?? DBNull.Value);
                command.Parameters.AddWithValue("@user", (object)connection.User ?? DBNull.Value);
                command.Parameters.AddWithValue("@pass", (object)connection.Password ?? DBNull.Value);
                command.Parameters.AddWithValue("@domain", (object)connection.Domain ?? DBNull.Value);
                command.Parameters.AddWithValue("@rememberPass", (object)connection.RememberPassword ?? false);
                command.ExecuteNonQuery();
            }
        }
    }
}
