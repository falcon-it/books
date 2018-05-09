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
    class BookServiceClient : IDisposable, IClientCallback
    {
        private BookServiceClient() { }
        public readonly static BookServiceClient instance = new BookServiceClient();
        public delegate void updateBookDelegate();
        public updateBookDelegate updateDelegate = null;

        private ChannelFactory<IBooksService> m_Factory;
        private ChannelFactory<ICallbackService> m_Factory2;
        private IBooksService m_Service;
        private ICallbackService m_Service2;
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
            //BasicHttpBinding binding = new BasicHttpBinding();
            WSDualHttpBinding binding = new WSDualHttpBinding();
            //m_Factory = new ChannelFactory<IBooksService>(binding, address);
            m_Factory = new ChannelFactory<IBooksService>(binding, address);
            m_Service = m_Factory.CreateChannel();

            InstanceContext context = new InstanceContext(this);
            m_Factory2 = new DuplexChannelFactory<ICallbackService>(context, new WSDualHttpBinding(), new EndpointAddress(new Uri(ConfigurationManager.AppSettings["service2"])));
            m_Service2 = m_Factory2.CreateChannel();
            m_Service2.dispatch();

        }

        public void Dispose()
        {
            if (m_Service != null)
            {
                logout();
                m_Service = null;
                m_Service2.close();
                m_Service2 = null;
                m_Factory.Close();
                m_Factory2.Close();
            }
        }

        public void updateBooks()
        {
            if(updateDelegate != null)
            {
                updateDelegate();
            }
        }
    }
}
