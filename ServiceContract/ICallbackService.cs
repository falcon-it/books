using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ServiceContract
{
    [ServiceContract(CallbackContract = typeof(IClientCallback))]
    public interface ICallbackService
    {
        [OperationContract(IsOneWay = true)]
        void dispatch();
        [OperationContract(IsOneWay = true)]
        void close();
    }
}
