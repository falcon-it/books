using System;
using System.Data.Linq.Mapping;
using scAuthor = ServiceContract.Entity.Author;

namespace BooksService.Data.Elements.Entity
{
    [Table(Name = "author")]
    class Author
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int id;
        [Column(DbType = "varchar(100)")]
        public string name;

        public static explicit operator scAuthor(Author a)
        {
            return new scAuthor(a.id, a.name);
        }
    }
}
