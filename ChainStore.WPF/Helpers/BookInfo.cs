using System;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;

namespace ChainStoreTRPZ2Edition.Helpers
{
    public sealed class BookInfo
    {
        public BookInfo(Product product, Book book)
        {
            CustomValidator.ValidateObject(product);
            CustomValidator.ValidateObject(book);
            ProductId = product.Id;
            ProductName = product.Name;
            BookCreationTime = book.CreationTime.ToLocalTime();
            BookExpirationTime = book.ExpirationTime.ToLocalTime();
            BookReserveDaysCount = book.ReserveDaysCount;
        }

        public Guid ProductId { get; }
        public string ProductName { get; }
        public DateTimeOffset BookCreationTime { get; }
        public DateTimeOffset BookExpirationTime { get; }
        public int BookReserveDaysCount { get; }
    }
}