using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace BooksService.Data.Elements.Entity
{
    [Table(Name = "author_book_link")]
    class AuthorBookLink
    {
        [Column(IsPrimaryKey = true)]
        public int book_id;
        [Column(IsPrimaryKey = true)]
        public int author_id;
    }
}
