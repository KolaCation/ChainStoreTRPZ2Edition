using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Helpers;
using ChainStoreTRPZ2Edition.Messages;
using ChainStoreTRPZ2Edition.ViewModels.ClientOperations;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.Account
{
    public sealed class ProfileViewModel : ViewModelBase, IRefreshableAsync, ICleanable
    {
        #region Properties

        private readonly IAuthenticator _authenticator;
        private readonly IClientRepository _clientRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IBookRepository _bookRepository;

        public Guid ClientId
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }

        public string ClientName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public double ClientBalance
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        public ObservableCollection<PurchaseInfo> ClientPurchases { get; set; }
        public ObservableCollection<BookInfo> ClientBooks { get; set; }

        public string SearchProduct
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        #endregion

        #region Commands

        public ICommand ChangeName { get; set; }
        public ICommand ReplenishBalance { get; set; }
        public ICommand NavigateToPurchase { get; set; }
        public ICommand Filter { get; set; }
        public ICommand ClearFilter { get; set; }

        #endregion

        #region Constructor

        public ProfileViewModel(IAuthenticator authenticator, IClientRepository clientRepository,
            IPurchaseRepository purchaseRepository, IBookRepository bookRepository)
        {
            _authenticator = authenticator;
            _clientRepository = clientRepository;
            _purchaseRepository = purchaseRepository;
            _bookRepository = bookRepository;
            Messenger.Default.Register<RefreshDataMessage>(this, RefreshDataAsync);
            NavigateToPurchase = new RelayCommand(productId =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(PurchaseViewModel), (Guid) productId));
                ClearData();
            });
            ClientBooks = new ObservableCollection<BookInfo>();
            ClientPurchases = new ObservableCollection<PurchaseInfo>();
            Filter = new RelayCommand(HandleFiltering);
            ClearFilter = new RelayCommand(HandleCleaning);
        }

        #endregion

        #region Methods

        public async void RefreshDataAsync(RefreshDataMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName) && refreshDataMessage.ItemId != null &&
                _authenticator.IsLoggedIn())
            {
                var clientId = _authenticator.GetCurrentUser().ClientId;
                var client = await _clientRepository.GetOne(clientId);
                ClientId = clientId;
                ClientName = client.Name;
                ClientBalance = client.Balance;
                var clientBooks = await _bookRepository.GetClientBooks(clientId);
                var clientBookedProducts = await _bookRepository.GetClientBookedProducts(clientId);
                var bookInfos = (from clientBook in clientBooks
                    join clientBookedProduct in clientBookedProducts on clientBook.ProductId equals
                        clientBookedProduct.Id
                    select new BookInfo(clientBookedProduct, clientBook)).ToList();
                ClientBooks.Clear();
                foreach (var bookInfo in bookInfos)
                {
                    ClientBooks.Add(bookInfo);
                }

                var clientPurchases = await _purchaseRepository.GetClientPurchases(clientId);
                var clientPurchasedProducts = await _purchaseRepository.GetClientPurchasedProducts(clientId);
                var purchaseInfos = (from clientPurchase in clientPurchases
                    join clientPurchasedProduct in clientPurchasedProducts on clientPurchase.ProductId equals
                        clientPurchasedProduct.Id
                    select new PurchaseInfo(clientPurchasedProduct, clientPurchase)).ToList();
                ClientPurchases.Clear();
                foreach (var purchaseInfo in purchaseInfos)
                {
                    ClientPurchases.Add(purchaseInfo);
                }
            }
        }

        public void ClearData()
        {
            ClientId = Guid.Empty;
            ClientName = string.Empty;
            ClientBalance = 0;
            SearchProduct = string.Empty;
            ClientBooks.Clear();
            ClientPurchases.Clear();
        }

        #endregion

        #region Handlers

        private void HandleFiltering()
        {
            if (!string.IsNullOrEmpty(SearchProduct))
            {
                var purchaseInfosToDisplay = ClientPurchases.Where(purchaseInfo =>
                    purchaseInfo.ProductName.ToLower().Contains(SearchProduct.ToLower())).ToList();
                ClientPurchases.Clear();
                foreach (var purchaseInfo in purchaseInfosToDisplay)
                {
                    ClientPurchases.Add(purchaseInfo);
                }
            }
            else
            {
                RefreshDataAsync(new RefreshDataMessage(nameof(ProfileViewModel), ClientId));
            }
        }

        private void HandleCleaning()
        {
            SearchProduct = string.Empty;
            RefreshDataAsync(new RefreshDataMessage(nameof(ProfileViewModel), ClientId));
        }

        #endregion
    }
}