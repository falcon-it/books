using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Entity
{
    [Serializable]
    public class CalulateUserBasketPriceResult
    {
        public readonly int count;
        public readonly double price;
        public readonly string discount;

        public CalulateUserBasketPriceResult(int count, double price, string discount)
        {
            this.count = count;
            this.price = price;
            this.discount = discount;
        }

        public override string ToString()
        {
            return string.Format("{0} книг за {1} руб [{2}]", new object[] { count, price, discount });
        }
    }
}
