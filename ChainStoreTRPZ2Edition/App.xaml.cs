using System.Windows;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl;
using ChainStore.DataAccessLayerImpl.Identity;
using ChainStore.DataAccessLayerImpl.RepositoriesImpl;
using ChainStore.Domain.Identity;
using ChainStoreTRPZ2Edition.ViewModels;
using ChainStoreTRPZ2Edition.ViewModels.Account;
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
                        opt.UseSqlServer(context.Configuration.GetConnectionString("ChainStoreDBTRPZ2")));
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
                    services.AddSingleton<RegisterViewModel>();
                    services.AddSingleton<LoginViewModel>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<StoreViewModel>();
                });
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            await MyDbContextSeedData.Initialize(_host.Services, Config);
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
    }
}