using System;
using System.Configuration;
using System.IO;
using System.Windows;
using ChainStore.DataAccessLayer.Identity;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl;
using ChainStore.DataAccessLayerImpl.Identity;
using ChainStore.DataAccessLayerImpl.RepositoriesImpl;
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
                    services.AddScoped<IBookRepository, SqlBookRepository>();
                    services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
                    services.AddScoped<IClientRepository, SqlClientRepository>();
                    services.AddScoped<IProductRepository, SqlProductRepository>();
                    services.AddScoped<IPurchaseRepository, SqlPurchaseRepository>();
                    services.AddScoped<IStoreRepository, SqlStoreRepository>();
                    services.AddScoped<ICustomUserManager, CustomUserManager>();
                    services.AddScoped<ICustomRoleManager, CustomRoleManager>();
                });
        }


        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            await MyDbContextSeedData.Initialize(_host.Services, Config);
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