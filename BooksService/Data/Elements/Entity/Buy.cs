using System;
using System.Data.Linq.Mapping;
using scBuy = ServiceContract.Entity.Buy;

namespace BooksService.Data.Elements.Entity
{
    [Table(Name = "buy")]
    class Buy
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int id;
        [Column]
        public DateTime date;
        [Column]
        public int user_id;
        [Column]
        public double price;
    }
}
