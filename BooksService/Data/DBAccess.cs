using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace BooksService.Data
{
    static class DBAccess
    {
        private static SqlConnection g_Connection;

        public static SqlConnection getConnection()
        {
            if(g_Connection == null)
            {
                g_Connection = new SqlConnection(ConfigurationManager.AppSettings["sqlserver"]);
                g_Connection.Open();
            }

            return g_Connection;
        }

        public static void Close()
        {
            if(g_Connection != null)
            {
                g_Connection.Close();
                g_Connection = null;
            }
        }
    }
}
