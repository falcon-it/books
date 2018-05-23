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
        public static SqlConnection getConnection()
        {
            SqlConnection _Connection = new SqlConnection(ConfigurationManager.AppSettings["sqlserver"]);
            _Connection.Open();
            return _Connection;
        }
    }
}
