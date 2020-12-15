using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.Domain.DomainCore;
using ChainStoreTRPZ2Edition.Admin.UserControls.Dialogs;
using ChainStoreTRPZ2Edition.Admin.ViewModels.Dialogs;
using ChainStoreTRPZ2Edition.DataInterfaces;
using ChainStoreTRPZ2Edition.Messages;
using ChainStoreTRPZ2Edition.ViewModels;
using DevExpress.Mvvm;
using MaterialDesignThemes.Wpf;

namespace ChainStoreTRPZ2Edition.Admin.ViewModels
{
    public sealed class CategoriesViewModel : ViewModelBase, IRefreshableAsync, ICleanable
    {
        #region Constructor

        public CategoriesViewModel(IAuthenticator authenticator, ICategoryRepository categoryRepository)
        {
            _authenticator = authenticator;
            _categoryRepository = categoryRepository;
            Categories = new ObservableCollection<Category>();
            Messenger.Default.Register<RefreshDataMessage>(this, RefreshDataAsync);
            CreateCategoryCommand = new RelayCommand(CreateCategoryHandler);
            EditCategoryCommand = new RelayCommand(categoryId => EditCategoryHandler((Guid) categoryId));
            DeleteCategoryCommand = new RelayCommand(categoryId => DeleteCategoryHandler((Guid) categoryId));
        }

        #endregion

        #region Properties

        private readonly IAuthenticator _authenticator;
        private readonly ICategoryRepository _categoryRepository;
        public ObservableCollection<Category> Categories { get; set; }

        #endregion

        #region Commands

        public ICommand CreateCategoryCommand { get; set; }
        public ICommand EditCategoryCommand { get; set; }
        public ICommand DeleteCategoryCommand { get; set; }

        #endregion

        #region Methods

        public async void RefreshDataAsync(RefreshDataMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName) && _authenticator.IsLoggedIn() &&
                await _authenticator.CurrentUserIsInRole("Admin"))
            {
                var categories = await _categoryRepository.GetAll();
                Categories.Clear();
                foreach (var category in categories) Categories.Add(category);
            }
        }

        public void ClearData()
        {
            Categories.Clear();
        }

        #endregion

        #region Handlers

        #endregion

        #region Dialogs

        private async void CreateCategoryHandler()
        {
            var view = new CreateEditCategoryDialog
            {
                DataContext = new CreateEditCategoryViewModel()
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is CreateEditCategoryViewModel data)
            {
                var createdCategory = new Category(Guid.NewGuid(), data.Name);
                await _categoryRepository.AddOne(createdCategory);
                Categories.Add(createdCategory);
            }
        }

        private async void EditCategoryHandler(Guid categoryId)
        {
            var categoryToEdit = Categories.First(e => e.Id.Equals(categoryId));
            var view = new CreateEditCategoryDialog
            {
                DataContext = new CreateEditCategoryViewModel(categoryToEdit)
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is CreateEditCategoryViewModel data)
            {
                var editedCategory = new Category(data.Id, data.Name);
                await _categoryRepository.UpdateOne(editedCategory);
                var categoryToReplace = Categories.First(e => e.Id.Equals(data.Id));
                Categories.Remove(categoryToReplace);
                Categories.Add(editedCategory);
            }
        }

        private async void DeleteCategoryHandler(Guid categoryId)
        {
            var categoryToDelete = Categories.First(e => e.Id.Equals(categoryId));
            var view = new RemoveItemDialog
            {
                DataContext = new RemoveItemViewModel(categoryToDelete.Id, categoryToDelete.Name)
            };

            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if (result is RemoveItemViewModel data)
            {
                await _categoryRepository.DeleteOne(data.ItemId);
                Categories.Remove(categoryToDelete);
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            var dialogHost = (DialogHost) sender;
            var dialogSession = dialogHost.CurrentSession;
            if (dialogSession.Content.GetType() == typeof(CreateEditCategoryDialog) &&
                eventArgs.Parameter is CreateEditCategoryViewModel dialogViewModel)
            {
                if (!dialogViewModel.IsValid())
                {
                    eventArgs.Cancel();
                }
                else if (Categories.Any(e =>
                    e.Name.ToLower().Equals(dialogViewModel.Name.ToLower()) && !e.Id.Equals(dialogViewModel.Id)))
                {
                    dialogViewModel.ErrorMessage = "Category already exists.";
                    eventArgs.Cancel();
                }
            }
        }

        #endregion
    }
}