using System;
using System.Collections.Generic;
using System.Text;
using ChainStore.Domain.DomainCore;
using ChainStore.Shared.Util;

namespace ChainStoreTRPZ2Edition.Helpers
{
    public sealed class BookInfo
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public DateTimeOffset BookCreationTime { get; private set; }
        public DateTimeOffset BookExpirationTime { get; private set; }
        public int BookReserveDaysCount { get; private set; }

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
    }
}
