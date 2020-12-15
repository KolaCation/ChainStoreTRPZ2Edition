using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.Admin.UserControls.Dialogs;
using ChainStoreTRPZ2Edition.Admin.ViewModels;
using ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Messages;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;

namespace ChainStoreTRPZ2Edition.ViewModels.Stores
{
    public sealed class StoresViewModel : ViewModelBase, IRefreshableAsync, ICleanable
    {
        #region Constructor

        public StoresViewModel(IAuthenticator authenticator, IStoreRepository storeRepository)
        {
            _authenticator = authenticator;
            _storeRepository = storeRepository;
            Stores = new ObservableCollection<Store>();
            Messenger.Default.Register<RefreshDataMessage>(this, RefreshData);
            Filter = new RelayCommand(FilterHandler);
            ClearFilter = new RelayCommand(CleanerHandler);
            ViewStoreDetails = new RelayCommand(id =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(StoreDetailsViewModel), (Guid) id));
                ClearData();
            });
            ViewCategories = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(CategoriesViewModel)));
                ClearData();
            });
            CreateStoreCommand = new RelayCommand(CreateStoreHandler);
            AdminButtonsVisibility = "Collapsed";
        }

        #endregion

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

        public string AdminButtonsVisibility
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

        #region Methods

        private async Task CurrentUserIsAdmin()
        {
            var isAdmin = await _authenticator.CurrentUserIsInRole("Admin");
            AdminButtonsVisibility = isAdmin ? "Visible" : "Collapsed";
        }

        public async void RefreshData(RefreshDataMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName))
            {
                Stores.Clear();
                var storesDomain = await _storeRepository.GetAll();
                foreach (var store in storesDomain) Stores.Add(store);
                await CurrentUserIsAdmin();
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

        private async void FilterHandler()
        {
            var stores = await _storeRepository.GetAll();
            var storesToDisplay = new List<Store>();
            if (!string.IsNullOrEmpty(SearchStore))
            {
                storesToDisplay.AddRange(stores.Where(st => st.Name.Contains(SearchStore, StringComparison.OrdinalIgnoreCase))
                    .ToList());
            }

            if (!string.IsNullOrEmpty(SearchProduct))
            {
                if (string.IsNullOrEmpty(SearchStore)) storesToDisplay = stores.ToList();

                var storesToIterate = new List<Store>(storesToDisplay);
                foreach (var store in storesToIterate)
                {
                    var success = false;
                    foreach (var category in store.Categories)
                    {
                        foreach (var product in category.Products)
                        {
                            if (product.Name.Contains(SearchProduct, StringComparison.OrdinalIgnoreCase))
                            {
                                success = true;
                                break;
                            }
                            else
                            {
                                success = false;
                            }
                        }

                        if (success) break;
                    }

                    if (!success) storesToDisplay.Remove(store);
                }
            }

            if (string.IsNullOrEmpty(SearchStore) && string.IsNullOrEmpty(SearchProduct))
                storesToDisplay = stores.ToList();

            Stores.Clear();
            foreach (var store in storesToDisplay) Stores.Add(store);
        }

        private void CleanerHandler()
        {
            SearchStore = string.Empty;
            SearchProduct = string.Empty;
            RefreshData(new RefreshDataMessage(GetType().Name));
        }

        #endregion

        #region Dialogs

        public ICommand CreateStoreCommand { get; set; }

        private async void CreateStoreHandler()
        {
            var view = new CreateEditStoreDialog
            {
                DataContext = new CreateEditStoreViewModel()
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is CreateEditStoreViewModel data)
            {
                var createdStore = new Store(Guid.NewGuid(), data.Name, data.Location, 0);
                await _storeRepository.AddOne(createdStore);
                Stores.Add(createdStore);
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            var dialogHost = (DialogHost) sender;
            var dialogSession = dialogHost.CurrentSession;
            if (dialogSession.Content.GetType() == typeof(CreateEditStoreDialog) &&
                eventArgs.Parameter is CreateEditStoreViewModel dialogViewModel)
            {
                if (!dialogViewModel.IsValid())
                {
                    eventArgs.Cancel();
                }
                else if (Stores.Any(e =>
                    e.Name.Equals(dialogViewModel.Name, StringComparison.OrdinalIgnoreCase) &&
                    e.Location.Equals(dialogViewModel.Location, StringComparison.OrdinalIgnoreCase) &&
                    !e.Id.Equals(dialogViewModel.Id)))
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