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
using ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Helpers;
using ChainStoreTRPZ2Edition.Messages;
using ChainStoreTRPZ2Edition.ViewModels.ClientOperations;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;

namespace ChainStoreTRPZ2Edition.ViewModels.Stores
{
    public sealed class StoreDetailsViewModel : ViewModelBase, IRefreshableAsync, ICleanable
    {
        #region Contsructor

        public StoreDetailsViewModel(IAuthenticator authenticator, IStoreRepository storeRepository,
            ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _authenticator = authenticator;
            _storeRepository = storeRepository;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            Categories = new ObservableCollection<Category>();
            Messenger.Default.Register<RefreshDataMessage>(this, RefreshData);
            Filter = new RelayCommand(FilterHandler);
            ClearFilter = new RelayCommand(CleanerHandler);
            NavigateToPurchase = new RelayCommand(id =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(PurchaseViewModel), (Guid)id));
                ClearData();
            });
            NavigateToBook = new RelayCommand(id =>
            {
                Messenger.Default.Send(new NavigationMessage(nameof(BookViewModel), (Guid)id));
                ClearData();
            });
            EditStoreCommand = new RelayCommand(EditStoreHandler);
            AddCategoryToStoreCommand = new RelayCommand(AddCategoryToStoreHandler);
            CreateProductCommand = new RelayCommand(categoryId => CreateProductHandler((Guid)categoryId));
            RemoveCategoryFromStoreCommand =
                new RelayCommand(categoryId => RemoveCategoryFromStoreHandler((Guid)categoryId));
            EditProductCommand = new RelayCommand(productId => EditProductHandler((Guid)productId));
            ReplenishProductCommand = new RelayCommand(productId => ReplenishProductHandler((Guid)productId));
            DeleteStoreCommand = new RelayCommand(DeleteStoreHandler);
            DeleteProductCommand = new RelayCommand(productId => DeleteProductHandler((Guid)productId));
            AdminButtonsVisibility = "Collapsed";
        }

        #endregion

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

        public string AdminButtonsVisibility
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        private readonly IAuthenticator _authenticator;
        private readonly IStoreRepository _storeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        #endregion

        #region Commands

        public ICommand Filter { get; set; }
        public ICommand ClearFilter { get; set; }
        public ICommand NavigateToPurchase { get; set; }
        public ICommand NavigateToBook { get; set; }

        #endregion

        #region Methods

        private async Task CurrentUserIsAdmin()
        {
            var isAdmin = await _authenticator.CurrentUserIsInRole("Admin");
            AdminButtonsVisibility = isAdmin ? "Visible" : "Collapsed";
        }

        public async void RefreshData(RefreshDataMessage refreshDataMessage)
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
                    var categoryToDisplay = new Category(productsToDisplay, category.Id, category.Name);
                    Categories.Add(categoryToDisplay);
                }

                await CurrentUserIsAdmin();
            }
        }

        private List<Product> GetListWithUniqueProducts(Category category)
        {
            var productGroups = category.Products.Where(e => e.ProductStatus.Equals(ProductStatus.OnSale))
                .GroupBy(e => e.Name);
            var listWithUniqueProducts = productGroups.Where(groupOfProducts => groupOfProducts.Any()).Select(groupOfProducts => groupOfProducts.First()).ToList();

            return listWithUniqueProducts.OrderBy(e => e.Name).ToList();
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
                        e.Name.Contains(SearchProduct, StringComparison.OrdinalIgnoreCase) &&
                        e.ProductStatus.Equals(ProductStatus.OnSale))));
                Categories.Clear();
                foreach (var category in categoriesToDisplay)
                {
                    var productsToDisplay = GetListWithUniqueProducts(category)
                        .Where(product => product.Name.Contains(SearchProduct, StringComparison.OrdinalIgnoreCase)).ToList();
                    var result = new Category(productsToDisplay, category.Id, category.Name);
                    Categories.Add(result);
                }
            }
        }

        private void CleanerHandler()
        {
            SearchProduct = string.Empty;
            RefreshData(new RefreshDataMessage(GetType().Name, StoreId));
        }

        #endregion

        #region Dialogs

        public ICommand EditStoreCommand { get; set; }
        public ICommand AddCategoryToStoreCommand { get; set; }
        public ICommand CreateProductCommand { get; set; }
        public ICommand EditProductCommand { get; set; }
        public ICommand ReplenishProductCommand { get; set; }
        public ICommand RemoveCategoryFromStoreCommand { get; set; }
        public ICommand DeleteStoreCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }

        private async void EditStoreHandler()
        {
            var storeToEdit = await _storeRepository.GetOne(StoreId);
            var view = new CreateEditStoreDialog
            {
                DataContext = new CreateEditStoreViewModel(storeToEdit),
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

        private async void AddCategoryToStoreHandler()
        {
            var categories = await _categoryRepository.GetAll();
            var categoriesToAdd = categories.Where(category => !Categories.Any(e => e.Id.Equals(category.Id))).ToList();
            var view = new AddCategoryToStoreDialog
            {
                DataContext = new AddCategoryToStoreViewModel(categoriesToAdd),
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is AddCategoryToStoreViewModel data)
            {
                var addedCategory = data.SelectedCategory;
                await _categoryRepository.AddCategoryToStore(addedCategory.Id, StoreId);
                RefreshData(new RefreshDataMessage(GetType().Name, StoreId));
            }
        }

        private async void CreateProductHandler(Guid categoryId)
        {
            var view = new CreateEditProductDialog
            {
                DataContext = new CreateEditProductViewModel(ProductOperationType.Create, categoryId),
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is CreateEditProductViewModel data && data.Type == ProductOperationType.Create)
            {
                for (var i = 0; i < data.QuantityOfProducts; i++)
                {
                    var createdProduct = new Product(Guid.NewGuid(), data.Name, data.PriceInUAH, data.ProductStatus,
                        data.CategoryId);
                    await _productRepository.AddProductToStore(createdProduct, StoreId);
                }

                RefreshData(new RefreshDataMessage(GetType().Name, StoreId));
            }
        }

        private async void EditProductHandler(Guid productId)
        {
            var productGroupRepresentative = await _productRepository.GetOne(productId);
            var storeProducts = await _storeRepository.GetStoreSpecificProducts(StoreId);
            var productsToUpdate = storeProducts
                .Where(e =>
                    e.Name.Equals(productGroupRepresentative.Name) &&
                    e.ProductStatus == ProductStatus.OnSale &&
                    e.CategoryId.Equals(productGroupRepresentative.CategoryId))
                .ToList();
            var view = new CreateEditProductDialog
            {
                DataContext = new CreateEditProductViewModel(ProductOperationType.Edit, productGroupRepresentative),
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is CreateEditProductViewModel data && data.Type == ProductOperationType.Edit)
            {
                foreach (var productToUpdate in productsToUpdate)
                {
                    var updatedProduct = new Product(productToUpdate.Id, data.Name, data.PriceInUAH, data.ProductStatus,
                        data.CategoryId);
                    await _productRepository.UpdateOne(updatedProduct);
                }

                RefreshData(new RefreshDataMessage(GetType().Name, StoreId));
            }
        }

        private async void ReplenishProductHandler(Guid productId)
        {
            var productGroupRepresentative = await _productRepository.GetOne(productId);
            var view = new CreateEditProductDialog
            {
                DataContext = new CreateEditProductViewModel(ProductOperationType.Replenish, productGroupRepresentative),
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is CreateEditProductViewModel data && data.Type == ProductOperationType.Replenish)
            {
                for (var i = 0; i < data.QuantityOfProducts; i++)
                {
                    var createdProduct = new Product(Guid.NewGuid(), data.Name, data.PriceInUAH, data.ProductStatus,
                        data.CategoryId);
                    await _productRepository.AddProductToStore(createdProduct, StoreId);
                }

                RefreshData(new RefreshDataMessage(GetType().Name, StoreId));
            }
        }

        private async void RemoveCategoryFromStoreHandler(Guid categoryId)
        {
            var categoryToRemove = Categories.First(e => e.Id.Equals(categoryId));
            var view = new RemoveItemDialog
            {
                DataContext = new RemoveItemViewModel(categoryToRemove.Id, categoryToRemove.Name),
            };

            var result = await DialogHost.Show(view, "RootDialog");
            if (result is RemoveItemViewModel data)
            {
                var storeProducts = await _storeRepository.GetStoreSpecificProducts(StoreId);
                var storeProductsOnSale = storeProducts.Where(e => e.ProductStatus == ProductStatus.OnSale).ToList();
                foreach (var storeProduct in storeProductsOnSale)
                {
                    if (storeProduct.CategoryId.Equals(data.ItemId))
                    {
                        await _productRepository.DeleteOne(storeProduct.Id);
                    }
                }

                await _categoryRepository.DeleteCategoryFromStore(data.ItemId, StoreId);
                RefreshData(new RefreshDataMessage(GetType().Name, StoreId));
            }
        }

        private async void DeleteStoreHandler()
        {
            var view = new RemoveItemDialog
            {
                DataContext = new RemoveItemViewModel(StoreId, StoreName),
            };

            var result = await DialogHost.Show(view, "RootDialog");
            if (result is RemoveItemViewModel data)
            {
                var storeProducts = await _storeRepository.GetStoreSpecificProducts(data.ItemId);
                foreach (var storeProduct in storeProducts)
                {
                    if (storeProduct.ProductStatus == ProductStatus.OnSale ||
                        storeProduct.ProductStatus == ProductStatus.Booked)
                    {
                        await _productRepository.DeleteOne(storeProduct.Id);
                    }
                    else
                    {
                        await _productRepository.DeleteProductFromStore(storeProduct, data.ItemId);
                    }
                }

                foreach (var category in Categories)
                {
                    await _categoryRepository.DeleteCategoryFromStore(category.Id, data.ItemId);
                }

                await _storeRepository.DeleteOne(data.ItemId);
                ClearData();
                Messenger.Default.Send(new NavigationMessage(nameof(StoresViewModel)));
            }
        }

        private async void DeleteProductHandler(Guid productId)
        {
            var productGroupRepresentative = await _productRepository.GetOne(productId);
            var view = new RemoveItemDialog
            {
                DataContext = new RemoveItemViewModel(productGroupRepresentative.Id, productGroupRepresentative.Name),
            };

            var result = await DialogHost.Show(view, "RootDialog");
            if (result is RemoveItemViewModel data)
            {
                var storeProducts = await _storeRepository.GetStoreSpecificProducts(StoreId);
                var storeProductsOnSale = storeProducts.Where(e =>
                        e.Name.Equals(data.ItemName) &&
                        e.ProductStatus != ProductStatus.Purchased &&
                        e.CategoryId.Equals(productGroupRepresentative.CategoryId))
                    .ToList();
                foreach (var storeProduct in storeProductsOnSale)
                {
                    await _productRepository.DeleteOne(storeProduct.Id);
                }

                RefreshData(new RefreshDataMessage(GetType().Name, StoreId));
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            var dialogHost = (DialogHost)sender;

            var dialogSession = dialogHost.CurrentSession;
            if (dialogSession.Content.GetType() == typeof(CreateEditStoreDialog) &&
                eventArgs.Parameter is CreateEditStoreViewModel createEditStoreViewModel)
            {
                if (!createEditStoreViewModel.IsValid())
                {
                    eventArgs.Cancel();
                }
                else if (_storeRepository.HasSameNameAndLocation(new Store(createEditStoreViewModel.Id, createEditStoreViewModel.Name, createEditStoreViewModel.Location, 0)))
                {
                    createEditStoreViewModel.ErrorMessage =
                        $"Store with same name already exists on {createEditStoreViewModel.Location}.";
                    eventArgs.Cancel();
                }
            }
            else if (dialogSession.Content.GetType() == typeof(AddCategoryToStoreDialog) &&
                     eventArgs.Parameter is AddCategoryToStoreViewModel addCategoryToStoreViewModel)
            {
                if (!addCategoryToStoreViewModel.IsValid())
                {
                    eventArgs.Cancel();
                }
            }
            else if (dialogSession.Content.GetType() == typeof(CreateEditProductDialog) &&
                     eventArgs.Parameter is CreateEditProductViewModel createEditProductViewModel)
            {
                if (!createEditProductViewModel.IsValid())
                {
                    eventArgs.Cancel();
                }
            }
        }

        #endregion
    }
}