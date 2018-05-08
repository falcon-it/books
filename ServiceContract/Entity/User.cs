﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceContract.Entity
{
    [Serializable]
    public class User
    {
        public readonly int id;
        public readonly string name;

        public User(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
