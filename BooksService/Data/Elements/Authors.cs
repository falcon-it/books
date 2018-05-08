using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.Entity;
using BooksService.Data;
using System.Data.SqlClient;

namespace BooksService.Data.Elements
{
    class Authors
    {
        private readonly List<Author> m_EAuthors = new List<Author>();

        public Authors()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM author", DBAccess.getConnection());
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        m_EAuthors.Add(new Author((int)reader.GetValue(0), (string)reader.GetValue(1)));
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }

        public Author[] list()
        {
            return m_EAuthors.ToArray();
       }

        public Author add(string name)
        {
            foreach (Author a in m_EAuthors)
            {
                if(a.name == name)
                {
                    return a;
                }
            }

            SqlCommand cmd = new SqlCommand("INSERT INTO author (name) VALUES (@name);SELECT SCOPE_IDENTITY();", DBAccess.getConnection());
            cmd.Parameters.Add(new SqlParameter("@name", name));
            Author na = new Author(decimal.ToInt32((decimal)cmd.ExecuteScalar()), name);
            m_EAuthors.Add(na);
            return na;
        }

        public Author[] getForIDs(List<int> ids)
        {
            List<Author> authors = new List<Author>();

            foreach (Author a in m_EAuthors)
            {
                if(ids.Contains(a.id))
                {
                    authors.Add(a);
                }
            }

            return authors.ToArray();
        }
    }
}
