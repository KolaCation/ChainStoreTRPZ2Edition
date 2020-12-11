using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.Admin.ViewModels;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Messages;
using DevExpress.Mvvm;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ChainStoreTRPZ2Edition.ViewModels.Stores
{
    public sealed class StoresViewModel : ViewModelBase, IRefreshableAsync, ICleanable
    {
        #region Properties

        private readonly IAuthenticator _authenticator;
        private readonly IStoreRepository _storeRepository;
        public ObservableCollection<Store> Stores { get; set; }

        public string SearchProduct
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string SearchStore
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        #endregion

        #region Commands

        public ICommand Filter { get; set; }
        public ICommand ClearFilter { get; set; }
        public ICommand ViewStoreDetails { get; set; }
        public ICommand ViewCategories { get; set; }

        #endregion

        #region Constructor

        public StoresViewModel(IAuthenticator authenticator, IStoreRepository storeRepository)
        {
            _authenticator = authenticator;
            _storeRepository = storeRepository;
            Stores = new ObservableCollection<Store>();
            Messenger.Default.Register<RefreshDataMessage>(this, RefreshDataAsync);
            Filter = new RelayCommand(HandleFiltering);
            ClearFilter = new RelayCommand(HandleCleaning);
            ViewStoreDetails = new RelayCommand(id =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(StoreDetailsViewModel), (Guid)id));
                ClearData();
            });
            ViewCategories = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(CategoriesViewModel)));
                ClearData();
            });
        }

        #endregion

        #region Methods

        public async void RefreshDataAsync(RefreshDataMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName))
            {
                Stores.Clear();
                var storesDomain = await _storeRepository.GetAll();
                foreach (var store in storesDomain)
                {
                    Stores.Add(store);
                }
            }
        }

        public void ClearData()
        {
            Stores.Clear();
            SearchStore = string.Empty;
            SearchProduct = string.Empty;
        }
        #endregion

        #region Handlers

        private async void HandleFiltering()
        {
            var stores = await _storeRepository.GetAll();
            var storesToDisplay = new List<Store>();
            if (!string.IsNullOrEmpty(SearchStore))
            {
                storesToDisplay.AddRange(stores.Where(st => st.Name.ToLower().Contains(SearchStore.ToLower()))
                    .ToList());
            }

            if (!string.IsNullOrEmpty(SearchProduct))
            {
                if (string.IsNullOrEmpty(SearchStore))
                {
                    storesToDisplay = stores.ToList();
                }

                var storesToIterate = new List<Store>(storesToDisplay);
                foreach (var store in storesToIterate)
                {
                    bool success = false;
                    foreach (var category in store.Categories)
                    {
                        foreach (var product in category.Products)
                        {
                            if (product.Name.ToLower().Contains(SearchProduct.ToLower()))
                            {
                                success = true;
                                break;
                            }
                            else
                            {
                                success = false;
                            }
                        }

                        if (success)
                        {
                            break;
                        }
                    }

                    if (!success) storesToDisplay.Remove(store);
                }
            }

            if (string.IsNullOrEmpty(SearchStore) && string.IsNullOrEmpty(SearchProduct))
            {
                storesToDisplay = stores.ToList();
            }

            Stores.Clear();
            foreach (var store in storesToDisplay)
            {
                Stores.Add(store);
            }
        }

        private void HandleCleaning()
        {
            SearchStore = string.Empty;
            SearchProduct = string.Empty;
            RefreshDataAsync(new RefreshDataMessage(GetType().Name));
        }

        #endregion

    }
}