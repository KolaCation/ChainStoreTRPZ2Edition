using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Helpers;
using ChainStoreTRPZ2Edition.Messages;
using ChainStoreTRPZ2Edition.UserControls.ClientOperations;
using ChainStoreTRPZ2Edition.UserControls.Dialogs;
using ChainStoreTRPZ2Edition.ViewModels.ClientOperations;
using ChainStoreTRPZ2Edition.ViewModels.Dialogs;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;

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

        #region Dialogs

        public ICommand ChangeNameCommand => new RelayCommand(ChangeNameHandler);
        public ICommand ReplenishBalanceCommand => new RelayCommand(ReplenishBalanceHandler);

        private async void ChangeNameHandler(object o)
        {
            var view = new ChangeNameDialog
            {
                DataContext = new ChangeNameViewModel(ClientName)
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is string)
            {
                var client = await _clientRepository.GetOne(_authenticator.GetCurrentUser().ClientId);
                client.UpdateName(result.ToString());
                await _clientRepository.UpdateOne(client);
                ClientName = client.Name;
            }
        }

        private async void ReplenishBalanceHandler(object o)
        {
            var view = new ReplenishBalanceDialog
            {
                DataContext = new ReplenishBalanceViewModel()
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is double)
            {
                var client = await _clientRepository.GetOne(_authenticator.GetCurrentUser().ClientId);
                client.ReplenishBalance(double.Parse(result.ToString()));
                await _clientRepository.UpdateOne(client);
                ClientBalance = client.Balance;
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            var dialogHost = (DialogHost) sender;
            var dialogSession = dialogHost.CurrentSession;
            if (dialogSession.Content.GetType() == typeof(ChangeNameDialog) && eventArgs.Parameter is string)
            {
                var dialogDataContext = ((ChangeNameDialog) dialogSession.Content).DataContext;
                var dialogViewModel = (ChangeNameViewModel) dialogDataContext;
                if (!dialogViewModel.IsValid())
                {
                    eventArgs.Cancel();
                }
            }
            else if (dialogSession.Content.GetType() == typeof(ReplenishBalanceDialog) &&
                     eventArgs.Parameter is double)
            {
                var dialogDataContext = ((ReplenishBalanceDialog)dialogSession.Content).DataContext;
                var dialogViewModel = (ReplenishBalanceViewModel)dialogDataContext;
                if (!dialogViewModel.IsValid())
                {
                    eventArgs.Cancel();
                }
            }
        }

        #endregion
    }
}