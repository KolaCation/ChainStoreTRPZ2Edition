﻿using System;
using System.Threading.Tasks;

namespace ChainStore.DataAccessLayer.Repositories
{
    public interface ICreateDeleteRepository<T>
    {
        Task AddOne(T item);
        Task DeleteOne(Guid id);
        bool Exists(Guid id);
    }
}