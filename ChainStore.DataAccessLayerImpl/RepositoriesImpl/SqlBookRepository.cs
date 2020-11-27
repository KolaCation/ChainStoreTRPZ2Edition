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
    public sealed class SqlBookRepository : IBookRepository
    {
        private readonly MyDbContext _context;
        private readonly BookMapper _bookMapper;
        private readonly ProductMapper _productMapper;

        public SqlBookRepository(MyDbContext context)
        {
            _context = context;
            _bookMapper = new BookMapper();
            _productMapper = new ProductMapper();
        }

        public async Task<List<Book>> GetClientBooks(Guid clientId)
        {
            CustomValidator.ValidateId(clientId);
            var bookDbModels = await _context.Books.Where(b => b.ClientId.Equals(clientId)).ToListAsync();
            var books = (from bookDbModel in bookDbModels select _bookMapper.DbToDomain(bookDbModel)).ToList();
            return books;
        }

        public async Task CheckBooksForExpiration()
        {
            var bookDbModels = await _context.Books.ToListAsync();
            var books = (from bookDbModel in bookDbModels select _bookMapper.DbToDomain(bookDbModel)).ToList();
            var booksToRemove = new List<Book>();
            foreach (var book in books)
            {
                var isExpired = book.IsExpired();
                if (!isExpired) return;
                var productDbModel = await _context.Products.FindAsync(book.ProductId);
                var product = _productMapper.DbToDomain(productDbModel);
                product.ChangeStatus(ProductStatus.OnSale);
                DetachService.Detach<ProductDbModel>(_context, product.Id);
                var enState = _context.Products.Update(_productMapper.DomainToDb(product));
                enState.State = EntityState.Modified;
                booksToRemove.Add(book);
            }

            foreach (var book in booksToRemove)
            {
                DetachService.Detach<BookDbModel>(_context, book.Id);
                _context.Books.Remove(_bookMapper.DomainToDb(book));
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddOne(Book item)
        {
            CustomValidator.ValidateObject(item);
            if (!Exists(item.Id))
            {
                var enState = await _context.Books.AddAsync(_bookMapper.DomainToDb(item));
                enState.State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            if (Exists(id))
            {
                var bookDbModel = await _context.Books.FindAsync(id);
                var enState = _context.Books.Remove(bookDbModel);
                enState.State = EntityState.Deleted;
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            return _context.Books.Any(item => item.Id.Equals(id));
        }
    }
}