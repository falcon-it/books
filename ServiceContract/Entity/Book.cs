using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract.Entity
{
    [Serializable]
    public class Book
    {
        public readonly int id;
        public int count;
        public string name;
        public Author[] authors;
        public Genre genre;
        public float price;

        public Book(int id, string name, int count, float price, Author[] authors, Genre genre)
        {
            this.id = id;
            this.count = count;
            this.authors = authors;
            this.genre = genre;
            this.name = name;
            this.price = price;
        }
    }
}
