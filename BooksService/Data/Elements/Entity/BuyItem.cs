using System;
using System.Data.Linq.Mapping;

namespace BooksService.Data.Elements.Entity
{
    [Table(Name = "buy_item")]
    class BuyItem
    {
        [Column(IsPrimaryKey = true)]
        public int book_id;
        [Column]
        public int count;
        [Column(IsPrimaryKey = true)]
        public int buy_id;
    }
}
