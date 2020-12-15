using System;
using System.Windows.Input;
using ChainStore.Actions.ApplicationServices;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Messages;
using ChainStoreTRPZ2Edition.ViewModels.Stores;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.ClientOperations
{
    public sealed class PurchaseViewModel : ViewModelBase, IRefreshableAsync, ICleanable
    {
        #region Constructor

        public PurchaseViewModel(IAuthenticator authenticator, IClientRepository clientRepository,
            IProductRepository productRepository, IPurchaseService purchaseService)
        {
            _authenticator = authenticator;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
            _purchaseService = purchaseService;
            Messenger.Default.Register<RefreshDataMessage>(this, RefreshData);
            Submit = new RelayCommand(productId => HandleOperation((Guid) productId));
            Cancel = new RelayCommand(storeId =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(StoreDetailsViewModel), (Guid) storeId));
                ClearData();
            });
        }

        #endregion

        #region Handlers

        private async void HandleOperation(Guid productId)
        {
            var result = await _purchaseService.HandleOperation(_authenticator.GetCurrentUser().ClientId, productId);
            if (result == PurchaseOperationResult.Success)
            {
                Messenger.Default.Send(new NavigationMessage(nameof(StoreDetailsViewModel), StoreId));
                ClearData();
            }
            else if (result == PurchaseOperationResult.InsufficientFunds)
            {
                ErrorMessage = "Not enough money. Replenish your balance.";
            }
            else
            {
                ErrorMessage = "Something went wrong. Try to repeat operation later.";
            }
        }

        #endregion

        #region Properties

        private readonly IAuthenticator _authenticator;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseService _purchaseService;

        public Guid ProductId
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }

        public Guid StoreId
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }

        public string ProductName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string StoreName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string StoreLocation
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public double ClientBalance
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        public double ProductPrice
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        public double Total
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        public string ErrorMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        #endregion

        #region Commands

        public ICommand Submit { get; set; }
        public ICommand Cancel { get; set; }

        #endregion

        #region Methods

        public async void RefreshData(RefreshDataMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName) && refreshDataMessage.ItemId != null
                                                                        && _authenticator.IsLoggedIn())
            {
                var client = await _clientRepository.GetOne(_authenticator.GetCurrentUser().ClientId);
                var product = await _productRepository.GetOne(refreshDataMessage.ItemId.Value);
                var store = await _productRepository.GetStoreOfSpecificProduct(product.Id);
                ProductId = product.Id;
                StoreId = store.Id;
                ProductName = product.Name;
                StoreName = store.Name;
                StoreLocation = store.Location;
                ClientBalance = client.Balance;
                ProductPrice = product.PriceInUAH;
                Total = ClientBalance - ProductPrice;
            }
        }

        public void ClearData()
        {
            ProductId = Guid.Empty;
            StoreId = Guid.Empty;
            ProductName = string.Empty;
            StoreName = string.Empty;
            StoreLocation = string.Empty;
            ClientBalance = 0;
            ProductPrice = 0;
            Total = 0;
            ErrorMessage = string.Empty;
        }

        #endregion
    }
}