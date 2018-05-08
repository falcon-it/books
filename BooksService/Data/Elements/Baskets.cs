using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.Entity;
using System.Data.SqlClient;

namespace BooksService.Data.Elements
{
    class Baskets
    {
        //                          user_id, BasketItem
        private readonly Dictionary<int, List<BasketItem>> m_EBaskets = new Dictionary<int, List<BasketItem>>();
        private readonly Books m_Books;

        public Baskets(Books books)
        {
            m_Books = books;
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

                SqlTransaction tr = DBAccess.getConnection().BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO buy (date, user_id, price) VALUES (@date, @user_id, @price);SELECT SCOPE_IDENTITY();", DBAccess.getConnection());
                    cmd.Transaction = tr;
                    cmd.Parameters.Add(new SqlParameter("@date", DateTime.Now));
                    cmd.Parameters.Add(new SqlParameter("@user_id", user.id));
                    cmd.Parameters.Add(new SqlParameter("@price", cbr.price));
                    int buy_id = decimal.ToInt32((decimal)cmd.ExecuteScalar());

                    foreach(BasketItem bi in m_EBaskets[user.id])
                    {
                        bi.book.count -= bi.count;

                        cmd = new SqlCommand("UPDATE book SET count=@count WHERE id=@id", DBAccess.getConnection());
                        cmd.Transaction = tr;
                        cmd.Parameters.Add(new SqlParameter("@count", bi.book.count));
                        cmd.Parameters.Add(new SqlParameter("@id", bi.book.id));
                        cmd.ExecuteNonQuery();

                        cmd = new SqlCommand("INSERT INTO buy_item (book_id, count, buy_id) VALUES (@book_id, @count, @buy_id)", DBAccess.getConnection());
                        cmd.Transaction = tr;
                        cmd.Parameters.Add(new SqlParameter("@book_id", bi.book.id));
                        cmd.Parameters.Add(new SqlParameter("@count", bi.count));
                        cmd.Parameters.Add(new SqlParameter("@buy_id", buy_id));
                        cmd.ExecuteNonQuery();
                    }

                    m_EBaskets.Remove(user.id);

                    tr.Commit();
                }
                catch(Exception e)
                {
                    tr.Rollback();
                }
            }
        }
    }
}
