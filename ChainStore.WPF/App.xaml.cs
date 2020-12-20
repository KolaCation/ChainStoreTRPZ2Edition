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
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        public static IConfiguration Config { get; private set; }

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
                    services.AddDbContext<MyDbContext>(opt => opt.UseSqlServer(context.Configuration.GetConnectionString("ChainStoreDBTRPZ2")));
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
    }
}