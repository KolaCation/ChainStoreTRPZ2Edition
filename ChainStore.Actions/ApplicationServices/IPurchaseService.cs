using System;
using System.Threading.Tasks;

namespace ChainStore.Actions.ApplicationServices
{
    public interface IPurchaseService
    {
        Task HandleOperation(Guid clientId, Guid productId);
    }
}
