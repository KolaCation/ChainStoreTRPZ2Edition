using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.Stores
{
    public sealed class StoresViewModel : ViewModelBase
    {
        #region Properties

        private readonly IAuthenticator _authenticator;
        private readonly IStoreRepository _storeRepository;

        #endregion

        #region Commands

        #endregion

        #region Constructor
        public StoresViewModel(IAuthenticator authenticator, IStoreRepository storeRepository)
        {
            _authenticator = authenticator;
            _storeRepository = storeRepository;
        }

        #endregion

        #region Methods



        #endregion

        #region Handlers

        

        #endregion
    }
}
