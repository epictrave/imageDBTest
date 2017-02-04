using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace test1
{
    class SqliteDB
    {
        string DbFile = "test1.db";
        string ConnectionString = "Data Source=test.db;Version=3;";
        public SqliteDB()
        {
            createDB();
        }
        private void createDB() {
            if (!System.IO.File.Exists(DbFile))
            {
                SQLiteConnection.CreateFile(DbFile);
            }
        }
            
        
        private void createTable() {
            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
            sqliteConn.Open();
            string query = "Create table if not exists score (name varchar(20), score int)";
            SQLiteCommand cmd = new SQLiteCommand(query, sqliteConn);
            cmd.ExecuteNonQuery();
            sqliteConn.Close();

        }

    }
}
