using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.Entity;
using BooksService.Data;
using System.Data.SqlClient;
using System.Data;

namespace BooksService.Data.Elements
{
    class Books
    {
        private readonly List<Book> m_EBook = new List<Book>(), m_EBookDeleted = new List<Book>();
        //                          book_id, []author_id
        private readonly Dictionary<int, List<int>> m_LinkToAuthor = new Dictionary<int, List<int>>();
        private readonly Authors m_Authors;
        private readonly Genres m_Genres;

        public Books(Authors authors, Genres genres)
        {
            m_Authors = authors;
            m_Genres = genres;

            SqlCommand cmd = new SqlCommand("SELECT * FROM author_book_link", DBAccess.getConnection());
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int key = (int)reader.GetValue(0);

                        if (!m_LinkToAuthor.ContainsKey(key))
                        {
                            m_LinkToAuthor.Add(key, new List<int>());
                        }

                        m_LinkToAuthor[key].Add((int)reader.GetValue(1));
                    }
                }
            }
            finally
            {
                reader.Close();
            }

            cmd = new SqlCommand("SELECT * FROM book", DBAccess.getConnection());
            reader = cmd.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Book loadedBook = new Book(
                            (int)reader.GetValue(0),
                            (string)reader.GetValue(1),
                            (int)reader.GetValue(2),
                            (float)reader.GetValue(3),
                            (m_LinkToAuthor.ContainsKey((int)reader.GetValue(0)) ? authors.getForIDs(m_LinkToAuthor[(int)reader.GetValue(0)]) : new Author[0]),
                            genres.getForID((int)reader.GetValue(4)));

                        if (((int)reader.GetValue(5)) == 0)
                        {
                            m_EBook.Add(loadedBook);
                        }
                        else
                        {
                            m_EBookDeleted.Add(loadedBook);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                reader.Close();
            }
        }

        public Book[] list()
        {
            return m_EBook.ToArray();
        }

        public Book add(string name, int count, float price, Author[] authors, Genre genre)
        {
            Book newBook = null;
            SqlTransaction tr = DBAccess.getConnection().BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO book (name, count, price, genre_id) VALUES (@name, @count, @price, @genre_id);SELECT SCOPE_IDENTITY();", DBAccess.getConnection());
                cmd.Transaction = tr;
                cmd.Parameters.Add(new SqlParameter("@name", name));
                cmd.Parameters.Add(new SqlParameter("@count", count));
                cmd.Parameters.Add(new SqlParameter("@price", price));
                cmd.Parameters.Add(new SqlParameter("@genre_id", genre.id));
                int book_id = decimal.ToInt32((decimal)cmd.ExecuteScalar());

                List<int> authors_ids = new List<int>();
                DataTable dt = new DataTable("author_book_link");

                dt.Columns.Add(new DataColumn("book_id", typeof(int)));
                dt.Columns.Add(new DataColumn("author_id", typeof(int)));

                foreach (Author a in authors)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["book_id"] = book_id;
                    newRow["author_id"] = a.id;
                    dt.Rows.Add(newRow);
                    authors_ids.Add(a.id);
                }

                using (SqlBulkCopy sbk = new SqlBulkCopy(DBAccess.getConnection(), SqlBulkCopyOptions.KeepIdentity, tr))
                {
                    sbk.BatchSize = 10;
                    sbk.DestinationTableName = "author_book_link";
                    sbk.WriteToServer(dt);
                }

                m_LinkToAuthor.Add(book_id, authors_ids);
                newBook = new Book(book_id, name, count, price, m_Authors.getForIDs(authors_ids), m_Genres.getForID(genre.id));
                m_EBook.Add(newBook);

                tr.Commit();
            }
            catch(Exception e)
            {
                tr.Rollback();
            }

            return newBook;
        }

        public void save(Book book)
        {
            SqlTransaction tr = DBAccess.getConnection().BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE book SET name=@name, count=@count, price=@price, genre_id=@genre_id WHERE id=@id", DBAccess.getConnection());
                cmd.Transaction = tr;
                cmd.Parameters.Add(new SqlParameter("@id", book.id));
                cmd.Parameters.Add(new SqlParameter("@name", book.name));
                cmd.Parameters.Add(new SqlParameter("@count", book.count));
                cmd.Parameters.Add(new SqlParameter("@price", book.price));
                cmd.Parameters.Add(new SqlParameter("@genre_id", book.genre.id));
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("DELETE FROM author_book_link WHERE book_id=@book_id", DBAccess.getConnection());
                cmd.Transaction = tr;
                cmd.Parameters.Add(new SqlParameter("@book_id", book.id));
                cmd.ExecuteNonQuery();

                List<int> authors_ids = new List<int>();
                DataTable dt = new DataTable("author_book_link");

                dt.Columns.Add(new DataColumn("book_id", typeof(int)));
                dt.Columns.Add(new DataColumn("author_id", typeof(int)));

                foreach (Author a in book.authors)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["book_id"] = book.id;
                    newRow["author_id"] = a.id;
                    dt.Rows.Add(newRow);
                    authors_ids.Add(a.id);
                }

                using (SqlBulkCopy sbk = new SqlBulkCopy(DBAccess.getConnection(), SqlBulkCopyOptions.KeepIdentity, tr))
                {
                    sbk.BatchSize = 10;
                    sbk.DestinationTableName = "author_book_link";
                    sbk.WriteToServer(dt);
                }

                foreach(Book bk in m_EBook)
                {
                    if(bk.id == book.id)
                    {
                        m_EBook[m_EBook.IndexOf(bk)] = book;
                        break;
                    }
                }

                m_LinkToAuthor[book.id] = authors_ids;

                tr.Commit();
            }
            catch (Exception e)
            {
                tr.Rollback();
            }
        }

        public void delete(Book book)
        {
            SqlCommand cmd = new SqlCommand("UPDATE book SET deleted=@deleted WHERE id=@id", DBAccess.getConnection());
            cmd.Parameters.Add(new SqlParameter("@id", book.id));
            cmd.Parameters.Add(new SqlParameter("@deleted", 1));
            cmd.ExecuteNonQuery();

            foreach(Book b in m_EBook)
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
