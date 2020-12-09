using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ChainStore.DataAccessLayerImpl.Helpers
{
    public sealed class OptionsBuilderService<T> where T : DbContext
    {
        private readonly IConfiguration _configuration;

        public OptionsBuilderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbContextOptions<T> BuildOptions()
        {
            var optBuilder = new DbContextOptionsBuilder<T>();
            optBuilder.UseSqlServer(_configuration.GetConnectionString("ChainStoreDBTRPZ2"));
            return optBuilder.Options;
        }
    }
}
