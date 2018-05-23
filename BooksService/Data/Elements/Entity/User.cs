using System;
using System.Data.Linq.Mapping;
using scUser = ServiceContract.Entity.User;

namespace BooksService.Data.Elements.Entity
{
    [Table(Name = "users")]
    class User
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int id;
        [Column(DbType = "varchar(100)")]
        public string name;

        public static explicit operator scUser(User u)
        {
            return new scUser(u.id, u.name);
        }
    }
}
