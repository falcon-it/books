using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Entity
{
    [Serializable]
    public class BasketShortInfo
    {
        public readonly double price;
        public readonly int count;

        public BasketShortInfo(double price, int count)
        {
            this.count = count;
            this.price = price;
        }

        public BasketShortInfo(CalulateUserBasketPriceResult cubpr)
        {
            count = cubpr.count;
            price = cubpr.price;
        }

        public override string ToString()
        {
            return string.Format("{0} книг за {1} руб", new object[] { count, price });
        }
    }
}
