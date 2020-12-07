using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
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

        public Store Store { get=>GetValue<Store>(); set=>SetValue(value); }
        public string SearchProduct { get=>GetValue<string>(); set=>SetValue(value); }
        private readonly IAuthenticator _authenticator;
        private readonly IStoreRepository _storeRepository;

        #endregion

        #region Commands
        #endregion

        #region Contsructor
        public StoreDetailsViewModel(IAuthenticator authenticator, IStoreRepository storeRepository)
        {
            _authenticator = authenticator;
            _storeRepository = storeRepository;
            Messenger.Default.Register<RefreshDataMessage>(this, RefreshDataAsync);
        }
        #endregion

        #region Methods
        public async void RefreshDataAsync(RefreshDataMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName) && refreshDataMessage.ItemId != null)
            {
                var store = await _storeRepository.GetOne(refreshDataMessage.ItemId.Value);
                Store = store;
            }
        }

        public void ClearData()
        {
        }
        #endregion

        #region Handlers
        #endregion


    }
}
