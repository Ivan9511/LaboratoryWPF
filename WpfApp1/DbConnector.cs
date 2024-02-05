using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class DbConnector
    {
        public static SqliteConnection OpenConnection()
        {
            SqliteConnection connection = new SqliteConnection("Data Source=C:\\C# project\\2024\\Attempt 2\\Laboratory.db");
            connection.Open();

            return connection;
        }
    }
}
