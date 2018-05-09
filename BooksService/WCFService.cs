using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;
using ServiceContract;
using BooksService.WCF;

namespace BooksService
{
    //netsh http add urlacl url=http://+:9001/BooksService user=mars\ilya
    //netsh http add urlacl url=http://+:9002/CallbackService user=mars\ilya

    class WCFService
    {
        private ServiceHost m_Host, m_Host2;

        public void Start()
        {
            if(m_Host != null)
            {
                m_Host.Close();
            }

            m_Host = new ServiceHost(typeof(BooksServiceImpl), new Uri(ConfigurationManager.AppSettings["service"]));
            m_Host2 = new ServiceHost(typeof(CallbackServiceImpl), new Uri(ConfigurationManager.AppSettings["service2"]));
            //BasicHttpBinding binding = new BasicHttpBinding();
            WSDualHttpBinding binding = new WSDualHttpBinding();
            WSDualHttpBinding binding2 = new WSDualHttpBinding();
            //binding.ReliableSession.Enabled = true;
            m_Host.AddServiceEndpoint(typeof(IBooksService), binding, ConfigurationManager.AppSettings["service"]);
            m_Host2.AddServiceEndpoint(typeof(ICallbackService), binding2, ConfigurationManager.AppSettings["service2"]);
            m_Host2.Open();
            //endpoint.EndpointBehaviors.Add(new ClientTrackerEndpointBehavior());
            m_Host.Open();
        }

        public void Stop()
        {
            if(m_Host != null)
            {
                m_Host.Close();
                m_Host.Close();
                m_Host = null;
            }
        }
    }
}
