using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract;
using System.ServiceModel;

namespace BooksService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    class CallbackServiceImpl : ICallbackService
    {
        IClientCallback clientCallback;
        public CallbackServiceImpl()
        {
            //Console.WriteLine(GetHashCode());
            clientCallback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
            App.instance.addClientCallBack(clientCallback);
        }

        public void close()
        {
            App.instance.delClientCallBack(clientCallback);
            //throw new NotImplementedException();
        }

        public void dispatch()
        {
            //throw new NotImplementedException();
        }
    }
}
