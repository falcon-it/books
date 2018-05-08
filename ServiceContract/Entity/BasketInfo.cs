using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.Entity
{
    [Serializable]
    public class BasketInfo
    {
        public readonly CalulateUserBasketPriceResult calcInfo;
        public readonly BasketItem[] items;

        public BasketInfo(CalulateUserBasketPriceResult calcInfo, BasketItem[] items)
        {
            this.calcInfo = calcInfo;
            this.items = items;
        }
    }
}
