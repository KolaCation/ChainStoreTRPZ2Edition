using System;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.Actions.ApplicationServices;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;

namespace ChainStore.ActionsImpl.ApplicationServicesImpl
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IStoreRepository _storeRepository;

        public PurchaseService(IClientRepository clientRepository, IProductRepository productRepository,
            IPurchaseRepository purchaseRepository, IStoreRepository storeRepository,
            IBookRepository bookRepository)
        {
            _clientRepository = clientRepository;
            _productRepository = productRepository;
            _purchaseRepository = purchaseRepository;
            _storeRepository = storeRepository;
            _bookRepository = bookRepository;
        }

        public async Task<PurchaseOperationResult> HandleOperation(Guid clientId, Guid productId)
        {
            CustomValidator.ValidateId(clientId);
            CustomValidator.ValidateId(productId);
            var client = await _clientRepository.GetOne(clientId);
            var product = await _productRepository.GetOne(productId);
            if (client != null && product != null)
            {
                var clientBooks = await _bookRepository.GetClientBooks(client.Id);
                var currentClientProductBook = clientBooks.FirstOrDefault(b => b.ProductId.Equals(product.Id));
                if (product.ProductStatus.Equals(ProductStatus.Booked) && currentClientProductBook != null)
                    await _bookRepository.DeleteOne(currentClientProductBook.Id);
                var operationSucceed = client.Charge(product.PriceInUAH);
                if (!operationSucceed) return PurchaseOperationResult.InsufficientFunds;
                var store = await _productRepository.GetStoreOfSpecificProduct(product.Id);
                store.GetProfit(product.PriceInUAH);
                product.ChangeStatus(ProductStatus.Purchased);
                var purchase = new Purchase(Guid.NewGuid(), clientId, productId, product.PriceInUAH);
                await _clientRepository.UpdateOne(client);
                await _storeRepository.UpdateOne(store);
                await _productRepository.UpdateOne(product);
                await _purchaseRepository.AddOne(purchase);
                return PurchaseOperationResult.Success;
            }

            return PurchaseOperationResult.Fail;
        }
    }
}