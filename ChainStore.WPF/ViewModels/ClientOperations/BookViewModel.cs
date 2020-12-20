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
    public sealed class BookViewModel : ViewModelBase, IRefreshableAsync, ICleanable
    {
        #region Contructor

        public BookViewModel(IAuthenticator authenticator, IClientRepository clientRepository,
            IProductRepository productRepository, IReservationService reservationService,
            IBookRepository bookRepository)
        {
            _authenticator = authenticator;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
            _reservationService = reservationService;
            _bookRepository = bookRepository;
            Messenger.Default.Register<RefreshDataMessage>(this, RefreshData);
            Submit = new RelayCommand(productId => HandleOperation((Guid)productId));
            Cancel = new RelayCommand(storeId =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(StoreDetailsViewModel), (Guid)storeId));
                ClearData();
            });
        }

        #endregion

        #region Handlers

        private async void HandleOperation(Guid productId)
        {
            var result =
                await _reservationService.HandleOperation(_authenticator.GetCurrentUser().ClientId, productId,
                    DaysCount);
            if (result == ReservationOperationResult.Success)
            {
                Messenger.Default.Send(new NavigationMessage(nameof(StoreDetailsViewModel), StoreId));
                ClearData();
            }
            else if (result == ReservationOperationResult.InvalidParameters)
            {
                ErrorMessage = "Min: 1 day. Max value: 7 days.";
            }
            else if (result == ReservationOperationResult.LimitExceeded)
            {
                ErrorMessage = "Books limit is exceeded: buy reserved products or wait limit till it ends.";
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
        private readonly IReservationService _reservationService;
        private readonly IBookRepository _bookRepository;

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

        public int ClientBooksCount
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public int DaysCount
        {
            get => GetValue<int>();
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
                var clientBooks = await _bookRepository.GetClientBooks(client.Id);
                var product = await _productRepository.GetOne(refreshDataMessage.ItemId.Value);
                var store = await _productRepository.GetStoreOfSpecificProduct(product.Id);
                ProductId = product.Id;
                StoreId = store.Id;
                ProductName = product.Name;
                StoreName = store.Name;
                StoreLocation = store.Location;
                ClientBooksCount = clientBooks.Count;
            }
        }

        public void ClearData()
        {
            ProductId = Guid.Empty;
            StoreId = Guid.Empty;
            ProductName = string.Empty;
            StoreName = string.Empty;
            StoreLocation = string.Empty;
            ClientBooksCount = 0;
            DaysCount = 0;
            ErrorMessage = string.Empty;
        }

        #endregion
    }
}