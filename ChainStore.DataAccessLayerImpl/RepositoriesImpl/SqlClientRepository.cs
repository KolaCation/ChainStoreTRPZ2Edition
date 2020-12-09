using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl.DbModels;
using ChainStore.DataAccessLayerImpl.Helpers;
using ChainStore.DataAccessLayerImpl.Mappers;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.DataAccessLayerImpl.RepositoriesImpl
{
    public class SqlClientRepository : IClientRepository
    {
        private readonly MyDbContext _context;
        private readonly ClientMapper _clientMapper;

        public SqlClientRepository(MyDbContext context)
        {
            _context = context;
            _clientMapper = new ClientMapper();
        }

        public async Task AddOne(Client item)
        {
            CustomValidator.ValidateObject(item);
            if (!Exists(item.Id))
            {
                var enState = await _context.Clients.AddAsync(_clientMapper.DomainToDb(item));
                enState.State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Client> GetOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var clientDbModel = await _context.Clients.FindAsync(id);
                return _clientMapper.DbToDomain(clientDbModel);
            }
            else
            {
                return null;
            }
        }

        public async Task<IReadOnlyCollection<Client>> GetAll()
        {
            var clientDbModels = await _context.Clients.ToListAsync();
            var clients = (from clientDbModel in clientDbModels select _clientMapper.DbToDomain(clientDbModel)).ToList();
            return clients.AsReadOnly();
        }

        public async Task UpdateOne(Client item)
        {
            CustomValidator.ValidateObject(item);
            if (Exists(item.Id))
            {
                DetachService.Detach<ClientDbModel>(_context, item.Id);
                var enState = _context.Clients.Update(_clientMapper.DomainToDb(item));
                enState.State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var clientDbModel = await _context.Clients.FindAsync(id);
                var enState = _context.Clients.Remove(clientDbModel);
                enState.State = EntityState.Deleted;
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            return _context.Clients.Any(item => item.Id.Equals(id));
        }
    }
}