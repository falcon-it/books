using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Configuration;
using ServiceContract;

namespace BooksService
{
    //netsh http add urlacl url=http://+:9001/BooksService user=mars\ilya
    class WCFService
    {
        private ServiceHost m_Host;

        public void Start()
        {
            if(m_Host != null)
            {
                m_Host.Close();
            }

            m_Host = new ServiceHost(typeof(BooksServiceImpl), new Uri(ConfigurationManager.AppSettings["service"]));
            BasicHttpBinding bilding = new BasicHttpBinding();
            m_Host.AddServiceEndpoint(typeof(IBooksService), bilding, ConfigurationManager.AppSettings["service"]);
            m_Host.Open();
        }

        public void Stop()
        {
            if(m_Host != null)
            {
                m_Host.Close();
                m_Host = null;
            }
        }
    }
}
