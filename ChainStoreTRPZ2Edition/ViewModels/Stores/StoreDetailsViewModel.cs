using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.Admin.UserControls.Dialogs;
using ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Messages;
using ChainStoreTRPZ2Edition.ViewModels.ClientOperations;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;

namespace ChainStoreTRPZ2Edition.ViewModels.Stores
{
    public sealed class StoreDetailsViewModel : ViewModelBase, IRefreshableAsync, ICleanable
    {
        #region Properties

        public Guid StoreId
        {
            get => GetValue<Guid>();
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

        public ObservableCollection<Category> Categories { get; set; }

        public string SearchProduct
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        private readonly IAuthenticator _authenticator;
        private readonly IStoreRepository _storeRepository;

        #endregion

        #region Commands

        public ICommand Filter { get; set; }
        public ICommand ClearFilter { get; set; }
        public ICommand NavigateToPurchase { get; set; }
        public ICommand NavigateToBook { get; set; }

        #endregion

        #region Contsructor

        public StoreDetailsViewModel(IAuthenticator authenticator, IStoreRepository storeRepository)
        {
            _authenticator = authenticator;
            _storeRepository = storeRepository;
            Categories = new ObservableCollection<Category>();
            Messenger.Default.Register<RefreshDataMessage>(this, RefreshDataAsync);
            Filter = new RelayCommand(FilterHandler);
            ClearFilter = new RelayCommand(CleanerHandler);
            NavigateToPurchase = new RelayCommand(id =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(PurchaseViewModel), (Guid) id));
                ClearData();
            });
            NavigateToBook = new RelayCommand(id =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(BookViewModel), (Guid) id));
                ClearData();
            });
            EditStoreCommand = new RelayCommand(EditStoreHandler);
        }

        #endregion

        #region Methods

        public async void RefreshDataAsync(RefreshDataMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName) && refreshDataMessage.ItemId != null)
            {
                var store = await _storeRepository.GetOne(refreshDataMessage.ItemId.Value);
                StoreId = store.Id;
                StoreName = store.Name;
                StoreLocation = store.Location;
                Categories.Clear();
                foreach (var category in store.Categories)
                {
                    var productsToDisplay = GetListWithUniqueProducts(category);
                    if (productsToDisplay.Count != 0)
                    {
                        var categoryToDisplay = new Category(productsToDisplay, category.Id, category.Name);
                        Categories.Add(categoryToDisplay);
                    }
                }
            }
        }

        private List<Product> GetListWithUniqueProducts(Category category)
        {
            var productGroups = category.Products.Where(e => e.ProductStatus.Equals(ProductStatus.OnSale))
                .GroupBy(e => e.Name);
            var listWithUniqueProducts = new List<Product>();
            foreach (var groupOfProducts in productGroups.Where(groupOfProducts => groupOfProducts.Count() != 0))
            {
                listWithUniqueProducts.Add(groupOfProducts.First());
            }

            listWithUniqueProducts = listWithUniqueProducts.OrderBy(e => e.Name).ToList();
            return listWithUniqueProducts;
        }

        public void ClearData()
        {
            Categories.Clear();
            SearchProduct = string.Empty;
            StoreName = string.Empty;
            StoreLocation = string.Empty;
            StoreId = Guid.Empty;
        }

        #endregion

        #region Handlers

        private void FilterHandler()
        {
            var categoriesToDisplay = new List<Category>();
            if (!string.IsNullOrEmpty(SearchProduct))
            {
                categoriesToDisplay.AddRange(Categories.Where(category =>
                    category.Products.Any(e =>
                        e.Name.ToLower().Contains(SearchProduct.ToLower()) &&
                        e.ProductStatus.Equals(ProductStatus.OnSale))));
                Categories.Clear();
                foreach (var category in categoriesToDisplay)
                {
                    var productsToDisplay = GetListWithUniqueProducts(category)
                        .Where(product => product.Name.ToLower().Contains(SearchProduct.ToLower())).ToList();
                    var result = new Category(productsToDisplay, category.Id, category.Name);
                    Categories.Add(result);
                }
            }
        }

        private void CleanerHandler()
        {
            SearchProduct = string.Empty;
            RefreshDataAsync(new RefreshDataMessage(GetType().Name, StoreId));
        }

        #endregion

        #region Dialogs

        public ICommand EditStoreCommand { get; set; }

        private async void EditStoreHandler()
        {
            var storeToEdit = await _storeRepository.GetOne(StoreId);
            var view = new CreateEditStoreDialog
            {
                DataContext = new CreateEditStoreViewModel(storeToEdit)
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is CreateEditStoreViewModel data)
            {
                var editedStore = new Store(data.Id, data.Name, data.Location, storeToEdit.Profit);
                await _storeRepository.UpdateOne(editedStore);
                StoreId = editedStore.Id;
                StoreName = editedStore.Name;
                StoreLocation = editedStore.Location;
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            var dialogHost = (DialogHost)sender;
            var dialogSession = dialogHost.CurrentSession;
            if (dialogSession.Content.GetType() == typeof(CreateEditStoreDialog) &&
                eventArgs.Parameter is CreateEditStoreViewModel dialogViewModel)
            {
                if (!dialogViewModel.IsValid())
                {
                    eventArgs.Cancel();
                }
                else if (_storeRepository.HasSameNameAndLocation(new Store(dialogViewModel.Id, dialogViewModel.Name, dialogViewModel.Location, 0)))
                {
                    dialogViewModel.ErrorMessage =
                        $"Store with same name already exists on {dialogViewModel.Location}.";
                    eventArgs.Cancel();
                }
            }
        }

        #endregion
    }
}