using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Entity
{
    [Serializable]
    public class BasketItem
    {
        public readonly Book book;
        public int count;

        public BasketItem(Book book, int count)
        {
            this.book = book;
            this.count = count;
        }
    }
}
