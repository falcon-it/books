using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.Entity;
using System.Data.SqlClient;
using BooksService.Data;
using dbGenre = BooksService.Data.Elements.Entity.Genre;

namespace BooksService.Data.Elements
{
    class Genres
    {
        private readonly List<dbGenre> m_EGenres = new List<dbGenre>();

        public Genres()
        {
            using (SqlConnection conn = DBAccess.getConnection())
            {
                DataContext db = new DataContext(conn);

                Table<dbGenre> genres = db.GetTable<dbGenre>();
                foreach(dbGenre gen in genres)
                {
                    m_EGenres.Add(gen);
                }
            }
        }
        public Genre[] list()
        {
            return m_EGenres.ConvertAll<Genre>(delegate (dbGenre g) { return (Genre)g; }).ToArray<Genre>();
        }

        public Genre add(string name)
        {
            foreach (dbGenre g in m_EGenres)
            {
                if (g.name == name) { return (Genre)g; }
            }

            using (SqlConnection conn = DBAccess.getConnection())
            {
                dbGenre ng = new dbGenre();
                ng.name = name;
                DataContext db = new DataContext(conn);
                db.GetTable<dbGenre>().InsertOnSubmit(ng);
                db.SubmitChanges();
                m_EGenres.Add(ng);
                return (Genre)ng;
            }
        }

        public dbGenre getForID(int id)
        {
            foreach (dbGenre g in m_EGenres)
            {
                if (g.id == id) { return g; }
            }

            throw new KeyNotFoundException("не удалось найти жанр");
        }
    }
}
