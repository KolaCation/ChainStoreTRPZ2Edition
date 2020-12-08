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
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Messages;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.Stores
{
    public sealed class StoreDetailsViewModel : ViewModelBase, IRefreshableAsync, ICleanable
    {
        #region Properties

        public Store Store
        {
            get => GetValue<Store>();
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
        public ICommand NavigateToBook{ get; set; }

        #endregion

        #region Contsructor

        public StoreDetailsViewModel(IAuthenticator authenticator, IStoreRepository storeRepository)
        {
            _authenticator = authenticator;
            _storeRepository = storeRepository;
            Categories = new ObservableCollection<Category>();
            Messenger.Default.Register<RefreshDataMessage>(this, RefreshDataAsync);
            Filter = new RelayCommand(HandleFiltering);
            ClearFilter = new RelayCommand(HandleCleaning);
            NavigateToPurchase = new RelayCommand(id =>
            {
                Messenger.Default.Send(new NavigationMessage("PurchaseViewModel", (Guid)id));
                ClearData();
            });
            NavigateToBook = new RelayCommand(id =>
            {
                Messenger.Default.Send(new NavigationMessage("BookViewModel", (Guid)id));
                ClearData();
            });
        }

        #endregion

        #region Methods

        public async void RefreshDataAsync(RefreshDataMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName) && refreshDataMessage.ItemId != null)
            {
                Categories.Clear();
                var store = await _storeRepository.GetOne(refreshDataMessage.ItemId.Value);
                Store = store;
                foreach (var category in store.Categories)
                {
                    var productsToDisplay = GetListWithUniqueProducts(category);
                    var categoryToDisplay = new Category(productsToDisplay, category.Id, category.Name);
                    Categories.Add(categoryToDisplay);
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
            Store = null;
        }

        #endregion

        #region Handlers

        private void HandleFiltering()
        {
            RefreshDataAsync(new RefreshDataMessage(GetType().Name, Store.Id));
            var categoriesToDisplay = new List<Category>();
            if (!string.IsNullOrEmpty(SearchProduct))
            {
                categoriesToDisplay.AddRange(Store.Categories.Where(category =>
                    category.Products.Any(e => e.Name.ToLower().Contains(SearchProduct.ToLower()))));
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

        private void HandleCleaning()
        {
            SearchProduct = string.Empty;
            RefreshDataAsync(new RefreshDataMessage(GetType().Name, Store.Id));
        }

        #endregion
    }
}