using System;
using System.Threading.Tasks;
using ChainStore.Actions.ApplicationServices;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;

namespace ChainStore.ActionsImpl.ApplicationServicesImpl
{
    public class ReservationService : IReservationService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;

        public ReservationService(IProductRepository productRepository, IBookRepository bookRepository, IClientRepository clientRepository)
        {
            _productRepository = productRepository;
            _bookRepository = bookRepository;
            _clientRepository = clientRepository;
        }

        public async Task<ReservationOperationResult> HandleOperation(Guid clientId, Guid productId,
            int reserveDaysCount)
        {
            CustomValidator.ValidateId(clientId);
            CustomValidator.ValidateId(productId);
            if (reserveDaysCount > 7 || reserveDaysCount < 1)
            {
                return ReservationOperationResult.InvalidParameters;
            }

            var clientExists = _clientRepository.Exists(clientId);
            var productExists = _productRepository.Exists(productId);

            if (clientExists && productExists)
            {
                var product = await _productRepository.GetOne(productId);
                var clientBooksLimitCount = await _bookRepository.GetClientBooks(clientId);
                if (clientBooksLimitCount.Count >= 3)
                {
                    return ReservationOperationResult.LimitExceeded;
                }

                product.ChangeStatus(ProductStatus.Booked);
                var book = new Book(Guid.NewGuid(), clientId, productId, reserveDaysCount);
                await _productRepository.UpdateOne(product);
                await _bookRepository.AddOne(book);
                return ReservationOperationResult.Success;
            }

            return ReservationOperationResult.Fail;
        }
    }
}