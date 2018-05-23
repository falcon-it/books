using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.Entity;
using System.Data.SqlClient;
using BooksService.Data;
using System.Data.Linq;
using dbUser = BooksService.Data.Elements.Entity.User;

namespace BooksService.Data.Elements
{
    class Users
    {
        private readonly List<dbUser> m_EUsers = new List<dbUser>();

        public Users()
        {
            using (SqlConnection conn = DBAccess.getConnection())
            {
                DataContext db = new DataContext(conn);

                Table<dbUser> users = db.GetTable<dbUser>();
                foreach (dbUser u in users)
                {
                    m_EUsers.Add(u);
                }
            }
        }

        public User login(string name)
        {
            foreach (dbUser u in m_EUsers)
            {
                if(u.name == name) { return (User)u; }
            }

            using (SqlConnection conn = DBAccess.getConnection())
            {
                dbUser nu = new dbUser();
                nu.name = name;
                DataContext db = new DataContext(conn);
                db.GetTable<dbUser>().InsertOnSubmit(nu);
                db.SubmitChanges();
                m_EUsers.Add(nu);
                return (User)nu;
            }
        }

        public User getByID(int user_id)
        {
            foreach(User u in m_EUsers)
            {
                if(u.id == user_id) { return u; }
            }

            throw new KeyNotFoundException("не удалось найти пользователя");
        }
    }
}
