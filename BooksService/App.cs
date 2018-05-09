using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksService.Data.Elements;
using ServiceContract.Entity;

namespace BooksService
{
    class App
    {
        public static readonly App instance = new App();

        private readonly Authors m_Authors = new Authors();
        private readonly Genres m_Genres = new Genres();
        private readonly Books m_Books;
        private readonly Users m_Users = new Users();
        private readonly Baskets m_Baskets;

        private App()
        {
            m_Books = new Books(m_Authors, m_Genres);
            m_Baskets = new Baskets(m_Books, m_Users);
        }

        public Author[] listAuthors()
        {
            lock(m_Authors)
            {
                return m_Authors.list();
            }
        }

        public Author addAuthor(string name)
        {
            lock (m_Authors)
            {
                return m_Authors.add(name);
            }
        }

        public Author[] getAuthorForIDs(List<int> ids)
        {
            lock (m_Authors)
            {
                return m_Authors.getForIDs(ids);
            }
        }

        public Genre addGenre(string name)
        {
            lock(m_Genres)
            {
                return m_Genres.add(name);
            }
        }

        public Genre[] listGenres()
        {
            lock (m_Genres)
            {
                return m_Genres.list();
            }
        }

        public Genre getGenreForID(int id)
        {
            lock (m_Genres)
            {
                return m_Genres.getForID(id);
            }
        }

        public Book[] listBooks()
        {
            lock(m_Books)
            {
                return m_Books.list();
            }
        }

        public Book addNewBook(string name, int count, float price, Author[] authors, Genre genre)
        {
            lock (m_Books)
            {
                return m_Books.add(name, count, price, authors, genre);
            }
        }

        public void saveBook(Book book)
        {
            lock (m_Books)
            {
                m_Books.save(book);
            }
        }

        public void deleteBook(Book book)
        {
            lock (m_Books)
            {
                m_Books.delete(book);
            }
        }

        public User login(string name)
        {
            lock (m_Users)
            {
                return m_Users.login(name);
            }
        }

        public void logout(User user)
        {
            lock (m_Baskets)
            {
                m_Baskets.logout(user);
            }
        }

        public BasketShortInfo addBookToBasket(User user, Book book, int count)
        {
            lock (m_Baskets)
            {
                return m_Baskets.add(user, book, count);
            }
        }

        public BasketInfo getUserBasket(User user)
        {
            lock (m_Baskets)
            {
                return m_Baskets.getUserBasket(user);
            }
        }

        public void deleteBookFromBasket(User user, BasketItem book)
        {
            lock (m_Baskets)
            {
                m_Baskets.deleteBookFromBasket(user, book);
            }
        }

        public BasketShortInfo getUserBasketShort(User user)
        {
            lock (m_Baskets)
            {
                return m_Baskets.getUserBasketShort(user);
            }
        }

        public void buyBasket(User user)
        {
            lock (m_Baskets)
            {
                m_Baskets.buyBasket(user);
            }
        }

        public Buy[] listBays(User user)
        {
            lock (m_Baskets)
            {
                return m_Baskets.listBays(user);
            }
        }
    }
}
