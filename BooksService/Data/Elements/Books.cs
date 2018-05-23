using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.Entity;
using BooksService.Data;
using System.Data.SqlClient;
using System.Data;
using System.Data.Linq;
using dbBook = BooksService.Data.Elements.Entity.Book;
using dbAuthorBookLink = BooksService.Data.Elements.Entity.AuthorBookLink;

namespace BooksService.Data.Elements
{
    class Books
    {
        private readonly List<dbBook> m_EBook = new List<dbBook>(), m_EBookDeleted = new List<dbBook>();
        //                          book_id, []author_id
        private readonly Dictionary<int, List<int>> m_LinkToAuthor = new Dictionary<int, List<int>>();
        private readonly Authors m_Authors;
        private readonly Genres m_Genres;

        public Books(Authors authors, Genres genres)
        {
            m_Authors = authors;
            m_Genres = genres;

            using (SqlConnection conn = DBAccess.getConnection())
            {
                DataContext db = new DataContext(conn);

                Table<dbAuthorBookLink> links = db.GetTable<dbAuthorBookLink>();

                foreach(dbAuthorBookLink ab in links)
                {
                    if (!m_LinkToAuthor.ContainsKey(ab.book_id))
                    {
                        m_LinkToAuthor.Add(ab.book_id, new List<int>());
                    }

                    m_LinkToAuthor[ab.book_id].Add(ab.author_id);
                }

                Table<dbBook> books = db.GetTable<dbBook>();

                foreach(dbBook b in books)
                {
                    b.genre = m_Genres.getForID(b.genre_id);
                    b.authors = (m_LinkToAuthor.ContainsKey(b.id) ? 
                        m_Authors.getForIDs(m_LinkToAuthor[b.id]) : new List<Entity.Author>());
                    if (b.isDeleted) { m_EBookDeleted.Add(b); }
                    else { m_EBook.Add(b); }
                }
            }
        }

        public Book[] list()
        {
            return m_EBook.ConvertAll<Book>(delegate (dbBook b) { return (Book)b; }).ToArray();
        }

        public Book add(string name, int count, float price, Author[] authors, Genre genre)
        {
            using (SqlConnection conn = DBAccess.getConnection())
            {
                DataContext db = new DataContext(conn);
                db.Transaction = db.Connection.BeginTransaction();

                try
                {
                    dbBook nb = new dbBook();
                    nb.name = name;
                    nb.count = count;
                    nb.price = price;
                    nb.genre_id = genre.id;
                    nb.genre = m_Genres.getForID(nb.genre_id);

                    db.GetTable<dbBook>().InsertOnSubmit(nb);
                    db.SubmitChanges();

                    List<int> aIDs = new List<int>();
                    Table<dbAuthorBookLink> linkTbl = db.GetTable<dbAuthorBookLink>();
                    foreach (Author a in authors)
                    {
                        dbAuthorBookLink nl = new dbAuthorBookLink();
                        nl.book_id = nb.id;
                        nl.author_id = a.id;
                        linkTbl.InsertOnSubmit(nl);
                        aIDs.Add(a.id);
                    }
                    nb.authors = m_Authors.getForIDs(aIDs);
                    db.SubmitChanges();

                    m_EBook.Add(nb);
                    m_LinkToAuthor.Add(nb.id, aIDs);

                    db.Transaction.Commit();

                    return (Book)nb;
                }
                catch(Exception e)
                {
                    db.Transaction.Rollback();
                    throw e;
                }
            }
        }

        public void save(Book book)
        {
            using (SqlConnection conn = DBAccess.getConnection())
            {
                DataContext db = new DataContext(conn);
                db.Transaction = db.Connection.BeginTransaction();

                try
                {
                    dbBook ub = (from b in db.GetTable<dbBook>() where b.id == book.id select b).First();
                    ub.name = book.name;
                    ub.count = book.count;
                    ub.price = book.price;
                    ub.genre_id = book.genre.id;
                    ub.genre = m_Genres.getForID(ub.genre_id);

                    Table<dbAuthorBookLink> link_table = db.GetTable<dbAuthorBookLink>();
                    var del_links = from l in link_table where l.book_id == book.id select l;
                    foreach(var del_item in del_links) { link_table.DeleteOnSubmit(del_item); }

                    List<int> aIDs = new List<int>();
                    Func<int, int, dbAuthorBookLink> link_builder = 
                        delegate (int author_id, int book_id)
                        {
                            dbAuthorBookLink newLink = new dbAuthorBookLink();
                            newLink.book_id = book_id;
                            newLink.author_id = author_id;
                            return newLink;
                        };
                    foreach (Author a in book.authors)
                    {
                        aIDs.Add(a.id);
                        link_table.InsertOnSubmit(link_builder(a.id, ub.id));
                    }
                    ub.authors = m_Authors.getForIDs(aIDs);

                    db.SubmitChanges();

                    int indx = m_EBook.FindIndex(x => x.id == book.id);
                    if(indx == -1) { m_EBook.Add(ub); }
                    else { m_EBook[indx] = ub; }

                    if(m_LinkToAuthor.ContainsKey(ub.id)) { m_LinkToAuthor[ub.id] = aIDs; }
                    else { m_LinkToAuthor.Add(ub.id, aIDs); }

                    db.Transaction.Commit();
                }
                catch(Exception)
                {
                    db.Transaction.Rollback();
                }
            }
        }

        public void delete(Book book)
        {
            using (SqlConnection conn = DBAccess.getConnection())
            {
                DataContext db = new DataContext(conn);
                db.Transaction = db.Connection.BeginTransaction();

                try
                {
                    dbBook dBook = (from d in db.GetTable<dbBook>() where d.id == book.id select d).First();
                    dBook.deleted = 1;
                    db.SubmitChanges();
                    db.Transaction.Commit();
                }
                finally
                {
                    db.Transaction.Rollback();
                }
            }

            foreach (dbBook b in m_EBook)
            {
                if(b.id == book.id)
                {
                    m_EBook.Remove(b);
                    m_EBookDeleted.Add(b);
                    break;
                }
            }
        }

        public Book getByID(int book_id)
        {
            foreach(Book b in m_EBook)
            {
                if(b.id == book_id)
                {
                    return b;
                }
            }

            throw new KeyNotFoundException("не удалось найти книгу");
        }
    }
}
