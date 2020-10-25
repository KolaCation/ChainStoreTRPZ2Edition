﻿using System;
using System.Collections.Generic;
using System.Linq;
using ChainStore.DataAccessLayer.Repositories;
using ChainStore.DataAccessLayerImpl.DbModels;
using ChainStore.DataAccessLayerImpl.Helpers;
using ChainStore.DataAccessLayerImpl.Mappers;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

        public void AddOne(Client item)
        {
            CustomValidator.ValidateObject(item);
            var exists = Exists(item.Id);
            if (!exists)
            {
                var enState = _context.Clients.Add(_clientMapper.DomainToDb(item));
                enState.State = EntityState.Added;
                _context.SaveChanges();
            }
        }

        public Client GetOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            var exists = Exists(id);
            if (exists)
            {
                return _clientMapper.DbToDomain(_context.Clients.Find(id));
            }
            else
            {
                return null;
            }
        }

        public IReadOnlyCollection<Client> GetAll()
        {
            var clientDbModelList = _context.Clients.ToList();
            var clientList = (from clientDbModel in clientDbModelList select _clientMapper.DbToDomain(clientDbModel)).ToList();
            return clientList.AsReadOnly();
        }

        public void UpdateOne(Client item)
        {
            CustomValidator.ValidateObject(item);
            var exists = Exists(item.Id);
            if (exists)
            {
                DetachService.Detach<ClientDbModel>(_context, item.Id);
                var enState = _context.Clients.Update(_clientMapper.DomainToDb(item));
                enState.State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void DeleteOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            var exists = Exists(id);
            if (exists)
            {
                var clientDbModel = _context.Clients.Find(id);
                var enState = _context.Clients.Remove(clientDbModel);
                enState.State = EntityState.Deleted;
                _context.SaveChanges();
            }
        }

        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            return _context.Clients.Any(item => item.Id.Equals(id));
        }
    }
}