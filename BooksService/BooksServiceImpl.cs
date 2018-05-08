using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract;
using ServiceContract.Entity;

namespace BooksService
{
    class BooksServiceImpl : IBooksService
    {
        public Author addNewAuthor(string name)
        {
            return App.instance.addAuthor(name);
        }
        public Author[] listAuthors()
        {
            return App.instance.listAuthors();
        }

        public Genre addNewGenre(string name)
        {
            return App.instance.addGenre(name);
        }
        public Genre[] listGenres()
        {
            return App.instance.listGenres();
        }


        public Book addNewBook(string name, int count, float price, Author[] authors, Genre genre)
        {
            return App.instance.addNewBook(name, count, price, authors, genre);
        }

        public Book[] listBooks()
        {
            return App.instance.listBooks();
        }

        public void saveBook(Book book)
        {
            App.instance.saveBook(book);
        }

        public void deleteBook(Book book)
        {
            App.instance.deleteBook(book);
        }

        public User loginUser(string name)
        {
            return App.instance.login(name);
        }

        public void logout(User user)
        {
            App.instance.logout(user);
        }

        public BasketShortInfo addBookToBasket(User user, Book book, int count)
        {
            return App.instance.addBookToBasket(user, book, count);
        }

        public BasketInfo getUserBasket(User user)
        {
            return App.instance.getUserBasket(user);
        }

        public void deleteBookFromBasket(User user, BasketItem book)
        {
            App.instance.deleteBookFromBasket(user, book);
        }

        public BasketShortInfo getUserBasketShort(User user)
        {
            return App.instance.getUserBasketShort(user);
        }

        public void buyBasket(User user)
        {
            App.instance.buyBasket(user);
        }
    }
}
