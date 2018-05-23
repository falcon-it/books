using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Data.Linq;
using dbBuy = BooksService.Data.Elements.Entity.Buy;
using dbBook = BooksService.Data.Elements.Entity.Book;
using dbBuyItem = BooksService.Data.Elements.Entity.BuyItem;

namespace BooksService.Data.Elements
{
    class Baskets
    {
        //                          user_id, BasketItem
        private readonly Dictionary<int, List<BasketItem>> m_EBaskets = new Dictionary<int, List<BasketItem>>();
        private readonly Books m_Books;
        private readonly Users m_Users;

        public Baskets(Books books, Users users)
        {
            m_Books = books;
            m_Users = users;
        }

        public void logout(User user)
        {
            if (m_EBaskets.ContainsKey(user.id))
            {
                m_EBaskets.Remove(user.id);
            }
        }

        public BasketShortInfo add(User user, Book book, int count)
        {
            lock (m_Books)
            {
                if (!m_EBaskets.ContainsKey(user.id))
                {
                    m_EBaskets.Add(user.id, new List<BasketItem>());
                }

                bool exist = false;
                foreach (BasketItem bi in m_EBaskets[user.id])
                {
                    if (bi.book.id == book.id)
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    Book ebook = m_Books.getByID(book.id);
                    m_EBaskets[user.id].Add(new BasketItem(ebook, count));
                }

                return new BasketShortInfo(calculateBasket(user));
            }
        }

        private CalulateUserBasketPriceResult calculateBasket(User user)
        {
            lock (m_Books)
            {
                int count = 0;
                List<int> genre_ids = new List<int>();
                double price = 0;
                string msg = string.Empty;

                foreach (BasketItem bi in m_EBaskets[user.id])
                {
                    count += bi.count;
                    if (!genre_ids.Contains(bi.book.genre.id)) { genre_ids.Add(bi.book.genre.id); }
                    price += bi.book.price * bi.count;
                }

                if (genre_ids.Count >= 3)
                {
                    price *= 0.9;
                    msg = "Скидка 10% за 3 жанра";
                }
                else if (count >= 10)
                {
                    price *= 0.95;
                    msg = "Скидка 5% за 10 книг";
                }

                return new CalulateUserBasketPriceResult(count, price, msg);
            }
        }

        public BasketInfo getUserBasket(User user)
        {
            lock (m_Books)
            {
                if (m_EBaskets.ContainsKey(user.id))
                {
                    return new BasketInfo(calculateBasket(user), m_EBaskets[user.id].ToArray());
                }

                return new BasketInfo(new CalulateUserBasketPriceResult(0, 0, string.Empty), new BasketItem[0]);
            }
        }

        public void deleteBookFromBasket(User user, BasketItem book)
        {
            if (m_EBaskets.ContainsKey(user.id))
            {
                foreach(BasketItem bi in m_EBaskets[user.id])
                {
                    if(bi.book.id == book.book.id)
                    {
                        m_EBaskets[user.id].Remove(bi);
                        break;
                    }
                }
            }
        }

        public BasketShortInfo getUserBasketShort(User user)
        {
            if (m_EBaskets.ContainsKey(user.id))
            {
                return new BasketShortInfo(calculateBasket(user));
            }

            return new BasketShortInfo(0, 0);
        }

        public void buyBasket(User user)
        {
            lock (m_Books)
            {
                if (m_EBaskets.ContainsKey(user.id))
                {
                    List<BasketItem> delbi = new List<BasketItem>();
                    foreach (BasketItem bi in m_EBaskets[user.id])
                    {
                        try
                        {
                            m_Books.getByID(bi.book.id);//бросит исключение если её удалили пока она лежала в корзине

                            if(bi.book.count == 0)
                            {
                                delbi.Add(bi);// вдруг они закончились
                                continue;
                            }

                            if(bi.count > bi.book.count)
                            {
                                bi.count = bi.book.count;//просто уменьшим - вдруг книг убавилось
                            }

                        }
                        catch(KeyNotFoundException e)
                        {
                            delbi.Add(bi);
                        }
                    }
                }

                CalulateUserBasketPriceResult cbr = calculateBasket(user);
                if(cbr.count == 0) { return; }

                using (SqlConnection conn = DBAccess.getConnection())
                {
                    DataContext db = new DataContext(conn);
                    db.Transaction = conn.BeginTransaction();

                    try
                    {
                        dbBuy nb = new dbBuy();
                        nb.date = DateTime.Now;
                        nb.user_id = user.id;
                        nb.price = cbr.price;
                        db.GetTable<dbBuy>().InsertOnSubmit(nb);
                        db.SubmitChanges();

                        List<int> books_id = new List<int>(m_EBaskets[user.id].Count),
                            books_count = new List<int>(m_EBaskets[user.id].Count);
                        Table<dbBuyItem> b_item_table = db.GetTable<dbBuyItem>();
                        foreach (BasketItem bi in m_EBaskets[user.id])
                        {
                            bi.book.count -= bi.count;
                            books_id.Add(bi.book.id);
                            books_count.Add(bi.book.count);
                            dbBuyItem b_item = new dbBuyItem();
                            b_item.book_id = bi.book.id;
                            b_item.buy_id = nb.id;
                            b_item.count = bi.count;
                            b_item_table.InsertOnSubmit(b_item);
                        }

                        var updated_books = from b in db.GetTable<dbBook>() where books_id.Contains(b.id) select b;
                        foreach(dbBook updated_book in updated_books)
                        {
                            updated_book.count = books_count[books_id.IndexOf(updated_book.id)];
                        }

                        db.SubmitChanges();

                        m_EBaskets.Remove(user.id);

                        db.Transaction.Commit();
                    }
                    catch(Exception e)
                    {
                        db.Transaction.Rollback();
                        throw e;
                    }
                }
            }
        }

        public Buy[] listBays(User user)
        {
            using (SqlConnection conn = DBAccess.getConnection())
            {
                DataContext db = new DataContext(conn);

                List<Buy> buys = new List<Buy>();
                var list = from b in db.GetTable<dbBuy>() join bi in db.GetTable<dbBuyItem>() on b.id equals bi.buy_id orderby b.date descending, b.id ascending
                           select new
                           {
                               buy_id = b.id,
                               buy_date = b.date,
                               buy_price = b.price,
                               buy_user = b.user_id,
                               book_count = bi.count,
                               book_id = bi.book_id
                           };
                if (user != null) { list = list.Where(b => b.buy_user == user.id); }

                bool first = true;
                int buy_id = 0, user_id = 0;
                DateTime dt = DateTime.Now;
                double price = 0;
                List<Buy.BuyItem> books = new List<Buy.BuyItem>();
                foreach (var list_item in list)
                {
                    int _buy_id = list_item.buy_id;

                    if(!first && (_buy_id != buy_id))
                    {
                        buys.Add(new Buy(buy_id, dt, m_Users.getByID(user_id), price, books.ToArray()));
                        books.Clear();
                    }

                    buy_id = _buy_id;
                    dt = list_item.buy_date;
                    user_id = list_item.buy_user;
                    price = list_item.buy_price;
                    books.Add(new Buy.BuyItem(list_item.book_count, m_Books.getByID(list_item.book_id)));

                    if (first) { first = false; }
                }

                if (!first)
                {
                    buys.Add(new Buy(buy_id, dt, m_Users.getByID(user_id), price, books.ToArray()));
                }

                return buys.ToArray();
            }
        }
    }
}
