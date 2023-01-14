using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RSManager.Data.Base
{
    internal abstract class SqLiteRepository : IRepository
    {
        protected readonly string DbName;

        protected SqLiteRepository(string name)
        {
            this.DbName = name;
            Initialize();
        }

        protected void Initialize()
        {
            CreateDatabase();
        }

        protected SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source=data\\{DbName}.db");
        }

        protected virtual List<string> CreateDefaultTables()
        {
            return new List<string>();
        }

        internal void CreateDatabase()
        {
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }

            if (File.Exists($@"data\\{DbName}.db"))
            {
                return;
            }

            using (var connection = GetConnection())
            {
                connection.Open();

                foreach (var sql in CreateDefaultTables())
                {
                    SqliteCommand command = new SqliteCommand(sql, connection);
                    command.ExecuteNonQuery(); 
                }
            }
        }

    }
}
