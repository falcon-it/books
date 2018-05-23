using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using scBook = ServiceContract.Entity.Book;
using scGenre = ServiceContract.Entity.Genre;
using scAuthor = ServiceContract.Entity.Author;

namespace BooksService.Data.Elements.Entity
{
    [Table(Name = "book")]
    class Book
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int id;
        [Column(DbType = "varchar(100)")]
        public string name;
        [Column]
        public int count;
        [Column]
        public float price;
        [Column]
        public int deleted;
        [Column]
        public int genre_id;

        public Genre genre;
        public List<Author> authors;

        public bool isDeleted { get { return deleted != 0; } }

        public static explicit operator scBook(Book b)
        {
            return new scBook(b.id, b.name, b.count, b.price, 
                b.authors.ConvertAll<scAuthor>(delegate(Author a) { return (scAuthor)a; }).ToArray(), 
                (scGenre)b.genre);
        }
    }
}
