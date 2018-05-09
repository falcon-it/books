using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;

namespace BooksService.WCF
{
    class ClientTrackerChannelInitializer : IChannelInitializer
    {
        public void Initialize(IClientChannel channel)
        {
            Console.WriteLine("Client {0} initialized", channel.SessionId);
            Console.WriteLine("Client {0} initialized", channel.GetProperty<object>().GetHashCode());
            //channel.GetProperty<BooksServiceImpl>();
            channel.Closed += ClientDisconnected;
            channel.Faulted += ClientDisconnected;
        }

        static void ClientDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Client {0} disconnected", ((IClientChannel)sender).SessionId);
        }
    }
}
