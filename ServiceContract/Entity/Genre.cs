using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract.Entity
{
    [Serializable]
    public class Genre
    {
        public readonly int id;
        public readonly string name;

        public Genre(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
