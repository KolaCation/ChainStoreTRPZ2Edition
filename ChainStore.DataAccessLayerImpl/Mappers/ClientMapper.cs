using System;
using System.Collections.Generic;
using System.Text;
using ChainStore.DataAccessLayerImpl.DbModels;
using ChainStore.DataAccessLayerImpl.Helpers;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChainStore.DataAccessLayerImpl.Mappers
{
    internal sealed class ClientMapper : IMapper<Client, ClientDbModel>
    {
        public ClientMapper()
        {
            
        }
        public ClientDbModel DomainToDb(Client item)
        {
            CustomValidator.ValidateObject(item);
            return new ClientDbModel(item.Id, item.Name, item.Balance);
        }

        public Client DbToDomain(ClientDbModel item)
        {
            CustomValidator.ValidateObject(item);
            return new Client(item.Id, item.Name, item.Balance);
        }
    }
}