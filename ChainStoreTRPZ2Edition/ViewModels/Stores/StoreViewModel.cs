using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStoreTRPZ2Edition.Messages;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.Stores
{
    public sealed class StoreViewModel : ViewModelBase
    {
        private readonly IAuthenticator _authenticator;
        private readonly IStoreRepository _storeRepository;
        public string WelcomeMessage { get; set; }

        public StoreViewModel(IAuthenticator authenticator, IStoreRepository storeRepository)
        {
            _authenticator = authenticator;
            _storeRepository = storeRepository;
            Messenger.Default.Register<RefreshDataMessage>(this, Refresh);
        }

        public void Refresh(RefreshDataMessage refreshDataMessage)
        {
            if (GetType().Name.Equals(refreshDataMessage.ViewModelName))
            {
                WelcomeMessage = _authenticator.GetCurrentUser().UserName;
                MessageBox.Show($"Hello {WelcomeMessage}!");
            }
        }
    }
}
