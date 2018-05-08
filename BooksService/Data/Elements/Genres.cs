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
    class Genres
    {
        private readonly List<Genre> m_EGenres = new List<Genre>();

        public Genres()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM genre", DBAccess.getConnection());
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        m_EGenres.Add(new Genre((int)reader.GetValue(0), (string)reader.GetValue(1)));
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
        public Genre[] list()
        {
            return m_EGenres.ToArray();
        }

        public Genre add(string name)
        {
            foreach (Genre g in m_EGenres)
            {
                if (g.name == name) { return g; }
            }

            SqlCommand cmd = new SqlCommand("INSERT INTO genre (name) VALUES (@name);SELECT SCOPE_IDENTITY();", DBAccess.getConnection());
            cmd.Parameters.Add(new SqlParameter("@name", name));
            Genre ng = new Genre(decimal.ToInt32((decimal)cmd.ExecuteScalar()), name);
            m_EGenres.Add(ng);
            return ng;
        }

        public Genre getForID(int id)
        {
            foreach (Genre g in m_EGenres)
            {
                if (g.id == id) { return g; }
            }

            throw new KeyNotFoundException("не удалось найти жанр");
        }
    }
}
