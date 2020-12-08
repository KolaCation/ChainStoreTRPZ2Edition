using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using ChainStore.Actions.ApplicationServices;
using ChainStore.DataAccessLayer.Repositories;
using DevExpress.Mvvm;

namespace ChainStoreTRPZ2Edition.ViewModels.ClientOperations
{
    public sealed class PurchaseViewModel : ViewModelBase
    {
        #region Properties
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseService _purchaseService;
        #endregion

        #region Commands

        public ICommand Submit { get; set; }
        public ICommand Cancel { get; set; }
        #endregion

        #region Constructor

        public PurchaseViewModel(IClientRepository clientRepository, IProductRepository productRepository, IPurchaseService purchaseService)
        {
            _clientRepository = clientRepository;
            _productRepository = productRepository;
            _purchaseService = purchaseService;
        }
        #endregion

        #region Methods
        #endregion

        #region Handlers
        #endregion
    }
}
