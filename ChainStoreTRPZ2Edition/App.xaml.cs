using System;
using System.IO;
using System.Windows;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl;
using ChainStore.DataAccessLayerImpl.RepositoriesImpl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChainStoreTRPZ2Edition
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IConfiguration Configuration { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            Configuration = builder.Build();
            var serviceProvider = CreateServiceProvider();
            base.OnStartup(e);
        }

        private IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddDbContext<MyDbContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("ChainStoreDBTRPZ2")));
            services.AddScoped<IBookRepository, SqlBookRepository>();
            services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
            services.AddScoped<IClientRepository, SqlClientRepository>();
            services.AddScoped<IProductRepository, SqlProductRepository>();
            services.AddScoped<IPurchaseRepository, SqlPurchaseRepository>();
            services.AddScoped<IStoreRepository, SqlStoreRepository>();
            return services.BuildServiceProvider();
        }
    }
}
