using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.Entity;
using BooksService.Data;
using System.Data.SqlClient;
using System.Data.Linq;
using dbAuthor = BooksService.Data.Elements.Entity.Author;

namespace BooksService.Data.Elements
{
    class Authors
    {
        private readonly List<dbAuthor> m_EAuthors = new List<dbAuthor>();

        public Authors()
        {
            using (SqlConnection conn = DBAccess.getConnection())
            {
                DataContext db = new DataContext(conn);

                Table<dbAuthor> authors = db.GetTable<dbAuthor>();
                foreach (dbAuthor a in authors)
                {
                    m_EAuthors.Add(a);
                }
            }
        }

        public Author[] list()
        {
            return m_EAuthors.ConvertAll<Author>(delegate (dbAuthor a) { return (Author)a; }).ToArray<Author>();
       }

        public Author add(string name)
        {
            foreach (dbAuthor a in m_EAuthors)
            {
                if(a.name == name)
                {
                    return (Author)a;
                }
            }

            using (SqlConnection conn = DBAccess.getConnection())
            {
                dbAuthor na = new dbAuthor();
                na.name = name;
                DataContext db = new DataContext(conn);
                db.GetTable<dbAuthor>().InsertOnSubmit(na);
                db.SubmitChanges();
                m_EAuthors.Add(na);
                return (Author)na;
            }
        }

        public List<dbAuthor> getForIDs(List<int> ids)
        {
            List<dbAuthor> authors = new List<dbAuthor>();

            foreach (dbAuthor a in m_EAuthors)
            {
                if(ids.Contains(a.id))
                {
                    authors.Add(a);
                }
            }

            return authors;
        }
    }
}
