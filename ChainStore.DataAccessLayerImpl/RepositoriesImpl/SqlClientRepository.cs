﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl.Helpers;
using ChainStore.DataAccessLayerImpl.Mappers;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.DataAccessLayerImpl.RepositoriesImpl
{
    public class SqlClientRepository : IClientRepository
    {
        private readonly ClientMapper _clientMapper;
        private readonly DbContextOptions<MyDbContext> _options;

        public SqlClientRepository(OptionsBuilderService<MyDbContext> optionsBuilder)
        {
            _clientMapper = new ClientMapper();
            _options = optionsBuilder.BuildOptions();
        }

        public async Task AddOne(Client item)
        {
            CustomValidator.ValidateObject(item);
            await using var context = new MyDbContext(_options);
            if (!Exists(item.Id))
            {
                var enState = await context.Clients.AddAsync(_clientMapper.DomainToDb(item));
                enState.State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Client> GetOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            await using var context = new MyDbContext(_options);
            if (Exists(id))
            {
                var clientDbModel = await context.Clients.FindAsync(id);
                return _clientMapper.DbToDomain(clientDbModel);
            }

            return null;
        }

        public async Task<IReadOnlyCollection<Client>> GetAll()
        {
            await using var context = new MyDbContext(_options);
            var clientDbModels = await context.Clients.ToListAsync();
            var clients = (from clientDbModel in clientDbModels select _clientMapper.DbToDomain(clientDbModel))
                .ToList();
            return clients.AsReadOnly();
        }

        public async Task UpdateOne(Client item)
        {
            CustomValidator.ValidateObject(item);
            await using var context = new MyDbContext(_options);
            if (Exists(item.Id))
            {
                var enState = context.Clients.Update(_clientMapper.DomainToDb(item));
                enState.State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            await using var context = new MyDbContext(_options);
            if (Exists(id))
            {
                var clientDbModel = await context.Clients.FindAsync(id);
                var enState = context.Clients.Remove(clientDbModel);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            using var context = new MyDbContext(_options);
            return context.Clients.Any(item => item.Id.Equals(id));
        }
    }
}