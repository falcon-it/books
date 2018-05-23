using System;
using System.Data.Linq.Mapping;
using scGenre = ServiceContract.Entity.Genre;

namespace BooksService.Data.Elements.Entity
{
    [Table(Name = "genre")]
    class Genre
    {
        [Column(IsDbGenerated = true,IsPrimaryKey = true)]
        public int id;
        [Column(DbType = "varchar(50)")]
        public string name;

        public static explicit operator scGenre(Genre g)
        {
            return new scGenre(g.id, g.name);
        }
    }
}
