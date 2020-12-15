using System;
using System.Threading.Tasks;

namespace ChainStore.Actions.ApplicationServices
{
    public interface IReservationService
    {
        Task<ReservationOperationResult> HandleOperation(Guid clientId, Guid productId, int reserveDaysCount);
    }
}