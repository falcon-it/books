using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.Entity;
using System.Data.SqlClient;
using BooksService.Data;

namespace BooksService.Data.Elements
{
    class Users
    {
        private readonly List<User> m_EUsers = new List<User>();

        public Users()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM users", DBAccess.getConnection());
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        m_EUsers.Add(new User((int)reader.GetValue(0), (string)reader.GetValue(1)));
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }

        public User login(string name)
        {
            foreach (User u in m_EUsers)
            {
                if(u.name == name) { return u; }
            }

            SqlCommand cmd = new SqlCommand("INSERT INTO users (name) VALUES (@name);SELECT SCOPE_IDENTITY();", DBAccess.getConnection());
            cmd.Parameters.Add(new SqlParameter("@name", name));
            User nu = new User(decimal.ToInt32((decimal)cmd.ExecuteScalar()), name);
            m_EUsers.Add(nu);
            return nu;
        }
    }
}
