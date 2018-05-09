using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Entity
{
    [Serializable]
    public class Buy
    {
        [Serializable]
        public struct BuyItem
        {
            public readonly int count;
            public readonly Book book;

            public BuyItem(int count, Book book)
            {
                this.count = count;
                this.book = book;
            }
        }
        public readonly int id;
        public readonly DateTime date;
        public readonly User user;
        public readonly double price;
        public readonly BuyItem[] books;

        public Buy(int id, DateTime date, User user, double price, BuyItem[] books)
        {
            this.id = id;
            this.date = date;
            this.user = user;
            this.price = price;
            this.books = books;
        }
    }
}
