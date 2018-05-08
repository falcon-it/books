using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ServiceContract;
using System.Configuration;
using ServiceContract.Entity;

namespace BooksClient
{
    class BookServiceClient : IDisposable
    {
        private BookServiceClient() { }
        public readonly static BookServiceClient instance = new BookServiceClient();

        private ChannelFactory<IBooksService> m_Factory;
        private IBooksService m_Service;
        private User m_User;

        public User user
        {
            get { return m_User; }
        }

        public bool isLogin() { return (m_User != null); }
        public bool login(string name)
        {
            m_User = m_Service.loginUser(name);
            return isLogin();
        }

        public void logout()
        {
            if(m_User != null)
            {
                m_Service.logout(m_User);
                m_User = null;
            }
        }

        public IBooksService Service
        {
            get { return m_Service; }
        }

        public void connect()
        {
            EndpointAddress address = new EndpointAddress(new Uri(ConfigurationManager.AppSettings["service"]));
            BasicHttpBinding bilding = new BasicHttpBinding();
            m_Factory = new ChannelFactory<IBooksService>(bilding, address);
            m_Service = m_Factory.CreateChannel();
        }

        public void Dispose()
        {
            if(m_Service != null)
            {
                logout();
                m_Service = null;
                m_Factory.Close();
            }
        }
    }
}
