using System;
using System.Linq;
using ChainStore.Actions.ApplicationServices;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl;
using ChainStore.DataAccessLayerImpl.Helpers;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;
using Microsoft.Extensions.Configuration;

namespace ChainStore.ActionsImpl.ApplicationServicesImpl
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IBookRepository _bookRepository;

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

        public void HandleOperation(Guid clientId, Guid productId)
        {
            CustomValidator.ValidateId(clientId);
            CustomValidator.ValidateId(productId);
            var client = _clientRepository.GetOne(clientId);
            var product = _productRepository.GetOne(productId);
            if (client != null && product != null)
            {
                var books = _bookRepository.GetClientBooks(client.Id);
                var bookToDel = books.FirstOrDefault(b => b.ProductId.Equals(product.Id));
                if (product.ProductStatus.Equals(ProductStatus.Booked) && bookToDel != null)
                    _bookRepository.DeleteOne(bookToDel.Id);
                var res = client.Charge(product.PriceInUAH);
                if (!res) return;
                var store = _productRepository.GetStoreOfSpecificProduct(product.Id);
                store.GetProfit(product.PriceInUAH);
                product.ChangeStatus(ProductStatus.Purchased);
                var purchase = new Purchase(Guid.NewGuid(), clientId, productId, product.PriceInUAH);
                _clientRepository.UpdateOne(client);
                _storeRepository.UpdateOne(store);
                _productRepository.UpdateOne(product);
                _purchaseRepository.AddOne(purchase);
            }
        }
    }
}