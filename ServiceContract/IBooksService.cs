using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ServiceContract.Entity;

namespace ServiceContract
{
    [ServiceContract]
    public interface IBooksService
    {
        [OperationContract]
        Author addNewAuthor(string name);
        [OperationContract]
        Author[] listAuthors();

        [OperationContract]
        Genre addNewGenre(string name);
        [OperationContract]
        Genre[] listGenres();

        [OperationContract]
        Book addNewBook(string name, int count, float price, Author[] authors, Genre genre);
        [OperationContract]
        void saveBook(Book book);
        [OperationContract]
        void deleteBook(Book book);
        [OperationContract]
        Book[] listBooks();

        [OperationContract]
        User loginUser(string name);
        [OperationContract]
        void logout(User user);

        [OperationContract]
        BasketShortInfo addBookToBasket(User user, Book book, int count);
        [OperationContract]
        BasketInfo getUserBasket(User user);
        [OperationContract]
        BasketShortInfo getUserBasketShort(User user);
        [OperationContract]
        void deleteBookFromBasket(User user, BasketItem book);
        [OperationContract]
        void buyBasket(User user);
    }
}
