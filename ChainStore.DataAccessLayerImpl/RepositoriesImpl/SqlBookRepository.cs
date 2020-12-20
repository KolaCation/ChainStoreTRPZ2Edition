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
        private readonly BookMapper _bookMapper;
        private readonly DbContextOptions<MyDbContext> _options;
        private readonly ProductMapper _productMapper;

        public SqlBookRepository(OptionsBuilderService<MyDbContext> optionsBuilder)
        {
            _bookMapper = new BookMapper();
            _productMapper = new ProductMapper();
            _options = optionsBuilder.BuildOptions();
        }

        public async Task<List<Book>> GetClientBooks(Guid clientId)
        {
            CustomValidator.ValidateId(clientId);
            await using var context = new MyDbContext(_options);
            var bookDbModels = await context.Books.Where(b => b.ClientId.Equals(clientId)).ToListAsync();
            var books = (from bookDbModel in bookDbModels select _bookMapper.DbToDomain(bookDbModel)).ToList();
            return books;
        }

        public async Task<List<Product>> GetClientBookedProducts(Guid clientId)
        {
            CustomValidator.ValidateId(clientId);
            await using var context = new MyDbContext(_options);
            var bookedProductDbModels = await (from productDbModel in context.Products
                join bookDbModel in context.Books on productDbModel.Id equals bookDbModel.ProductId
                select productDbModel).ToListAsync();
            var bookedProducts = (from bookedProductDbModel in bookedProductDbModels
                select _productMapper.DbToDomain(bookedProductDbModel)).ToList();
            return bookedProducts;
        }

        public async Task CheckBooksForExpiration()
        {
            await using var context = new MyDbContext(_options);
            var bookDbModels = await context.Books.ToListAsync();
            var books = (from bookDbModel in bookDbModels select _bookMapper.DbToDomain(bookDbModel)).ToList();
            var booksToRemove = new List<Book>();
            foreach (var book in books)
            {
                var isExpired = book.IsExpired();
                if (!isExpired && context.Products.Any(e => e.Id.Equals(book.ProductId)))
                {
                    continue;
                }

                var productDbModel = await context.Products.FindAsync(book.ProductId);
                if (productDbModel != null)
                {
                    var product = _productMapper.DbToDomain(productDbModel);
                    product.ChangeStatus(ProductStatus.OnSale);
                    DetachService.Detach<ProductDbModel>(context, product.Id);
                    var enState = context.Products.Update(_productMapper.DomainToDb(product));
                    enState.State = EntityState.Modified;
                }

                booksToRemove.Add(book);
            }

            foreach (var book in booksToRemove)
            {
                DetachService.Detach<BookDbModel>(context, book.Id);
                context.Books.Remove(_bookMapper.DomainToDb(book));
            }

            await context.SaveChangesAsync();
        }

        public async Task AddOne(Book item)
        {
            CustomValidator.ValidateObject(item);
            await using var context = new MyDbContext(_options);
            if (!Exists(item.Id))
            {
                var enState = await context.Books.AddAsync(_bookMapper.DomainToDb(item));
                enState.State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteOne(Guid id)
        {
            CustomValidator.ValidateId(id);
            await using var context = new MyDbContext(_options);
            if (Exists(id))
            {
                var bookDbModel = await context.Books.FindAsync(id);
                var enState = context.Books.Remove(bookDbModel);
                enState.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        public bool Exists(Guid id)
        {
            CustomValidator.ValidateId(id);
            using var context = new MyDbContext(_options);
            return context.Books.Any(item => item.Id.Equals(id));
        }
    }
}