using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ChainStore.Actions.ApplicationServices;
using ChainStore.ActionsImpl.ApplicationServicesImpl;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl;
using ChainStore.DataAccessLayerImpl.Helpers;
using ChainStore.DataAccessLayerImpl.Identity;
using ChainStore.DataAccessLayerImpl.RepositoriesImpl;
using ChainStore.Domain.DomainCore;
using ChainStore.Domain.Identity;
using ChainStoreTRPZ2Edition.Admin.ViewModels;
using ChainStoreTRPZ2Edition.ViewModels;
using ChainStoreTRPZ2Edition.ViewModels.Account;
using ChainStoreTRPZ2Edition.ViewModels.ClientOperations;
using ChainStoreTRPZ2Edition.ViewModels.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChainStoreTRPZ2Edition
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        public static IConfiguration Config { get; private set; }

        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args = null)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(c =>
                {
                    c.AddJsonFile("appsettings.json");
                    c.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    Config = context.Configuration;
                    services.AddDbContext<MyDbContext>(opt =>
                    {
                        opt.UseSqlServer(context.Configuration.GetConnectionString("ChainStoreDBTRPZ2"));
                    });
                    services.AddSingleton<OptionsBuilderService<MyDbContext>>();
                    services.AddSingleton<IBookRepository, SqlBookRepository>();
                    services.AddSingleton<ICategoryRepository, SqlCategoryRepository>();
                    services.AddSingleton<IClientRepository, SqlClientRepository>();
                    services.AddSingleton<IProductRepository, SqlProductRepository>();
                    services.AddSingleton<IPurchaseRepository, SqlPurchaseRepository>();
                    services.AddSingleton<IStoreRepository, SqlStoreRepository>();
                    services.AddSingleton<ICustomUserManager, CustomUserManager>();
                    services.AddSingleton<ICustomRoleManager, CustomRoleManager>();
                    services.AddSingleton<IAuthenticationService, AuthenticationService>();
                    services.AddSingleton<IAuthenticator, Authenticator>();
                    services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
                    services.AddSingleton<IPurchaseService, PurchaseService>();
                    services.AddSingleton<IReservationService, ReservationService>();
                    services.AddSingleton<RegisterViewModel>();
                    services.AddSingleton<LoginViewModel>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<StoresViewModel>();
                    services.AddSingleton<StoreDetailsViewModel>();
                    services.AddSingleton<PurchaseViewModel>();
                    services.AddSingleton<BookViewModel>();
                    services.AddSingleton<ProfileViewModel>();
                    services.AddSingleton<CategoriesViewModel>();
                });
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            await MyDbContextSeedData.Initialize(_host.Services, Config);
            //await AddData(_host.Services.GetRequiredService<IProductRepository>(),
            //    _host.Services.GetRequiredService<IStoreRepository>(),
            //_host.Services.GetRequiredService<ICategoryRepository>());
            var bookRepository = _host.Services.GetRequiredService<IBookRepository>();
            var threadStart = new ThreadStart(async () => await CheckBooksForExpiration(bookRepository));
            var thread = new Thread(threadStart);
            thread.Start();
            var window = _host.Services.GetRequiredService<MainWindow>();
            window.DataContext = _host.Services.GetRequiredService<MainViewModel>();
            window.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }

        private async Task CheckBooksForExpiration(IBookRepository bookRepository)
        {
            while (true)
            {
                await bookRepository.CheckBooksForExpiration();
                Thread.Sleep(5000);
            }
        }

        private async Task AddData(IProductRepository productRepository, IStoreRepository storeRepository,
            ICategoryRepository categoryRepository)
        {
            var category1Id = new Guid("080917f2-e2fa-4581-a7c2-743b259852ef");
            var category2Id = new Guid("1696b27d-8452-458a-994b-fdeef9cff690");
            var store1 = new Store(Guid.NewGuid(), "Shields and Weapons 2", "10 Pandora Street 2", 0);
            var storeCatRel1 = new StoreCategoryRelation(store1.Id, category1Id);
            var storeCatRel2 = new StoreCategoryRelation(store1.Id, category2Id);
            var product1 = new Product(Guid.NewGuid(), "HP 450 G1", 20_000, ProductStatus.OnSale, category1Id);
            var product11 = new Product(Guid.NewGuid(), "HP 450 G1", 20_000, ProductStatus.OnSale, category1Id);
            var product111 = new Product(Guid.NewGuid(), "HP 450 G1", 20_000, ProductStatus.OnSale, category1Id);
            var product1111 = new Product(Guid.NewGuid(), "HP 450 G1", 20_000, ProductStatus.OnSale, category1Id);
            var product11111 = new Product(Guid.NewGuid(), "HP 450 G1", 20_000, ProductStatus.OnSale, category1Id);
            var product2 = new Product(Guid.NewGuid(), "HP 450 G2", 30_000, ProductStatus.OnSale, category1Id);
            var product3 = new Product(Guid.NewGuid(), "HP 450 G3", 40_000, ProductStatus.OnSale, category1Id);
            var product4 = new Product(Guid.NewGuid(), "HP 450 G4", 50_000, ProductStatus.OnSale, category1Id);
            var product5 = new Product(Guid.NewGuid(), "HP 850 G5", 60_000, ProductStatus.OnSale, category1Id);
            var product6 = new Product(Guid.NewGuid(), "LogTech G12", 1000, ProductStatus.OnSale, category2Id);
            var product7 = new Product(Guid.NewGuid(), "X7", 2000, ProductStatus.OnSale, category2Id);
            var stPrRel1 = new StoreProductRelation(store1.Id, product1.Id);
            var stPrRel11 = new StoreProductRelation(store1.Id, product11.Id);
            var stPrRel111 = new StoreProductRelation(store1.Id, product111.Id);
            var stPrRel1111 = new StoreProductRelation(store1.Id, product1111.Id);
            var stPrRel11111 = new StoreProductRelation(store1.Id, product11111.Id);
            var stPrRel2 = new StoreProductRelation(store1.Id, product2.Id);
            var stPrRel3 = new StoreProductRelation(store1.Id, product3.Id);
            var stPrRel4 = new StoreProductRelation(store1.Id, product4.Id);
            var stPrRel5 = new StoreProductRelation(store1.Id, product5.Id);
            var stPrRel6 = new StoreProductRelation(store1.Id, product6.Id);
            var stPrRel7 = new StoreProductRelation(store1.Id, product7.Id);
            await storeRepository.AddOne(store1);
            await categoryRepository.AddCategoryToStore(category1Id, store1.Id);
            await categoryRepository.AddCategoryToStore(category2Id, store1.Id);
            await productRepository.AddProductToStore(product1, store1.Id);
            await productRepository.AddProductToStore(product11, store1.Id);
            await productRepository.AddProductToStore(product111, store1.Id);
            await productRepository.AddProductToStore(product1111, store1.Id);
            await productRepository.AddProductToStore(product11111, store1.Id);
            await productRepository.AddProductToStore(product2, store1.Id);
            await productRepository.AddProductToStore(product3, store1.Id);
            await productRepository.AddProductToStore(product4, store1.Id);
            await productRepository.AddProductToStore(product5, store1.Id);
            await productRepository.AddProductToStore(product6, store1.Id);
            await productRepository.AddProductToStore(product7, store1.Id);
        }
    }
}