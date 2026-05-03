using SipBrew.Core.Business;
using SipBrew.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SipBrew.Core.Common
{
    public static class QueryableExtension
    {
        public static IQueryable<T> PagedQuery<T>(
        this IQueryable<T> query,
        int size,
        int index)
        {
            return query
                .Skip((index - 1) * size)
                .Take(size);
        }

        public static IQueryable<ProductsModel> QueryToDto(this IQueryable<ProductsModel> query)
        {
            return query.Select(x => new ProductsModel
            {
                Id = x.Id,
                ProductName = x.ProductName,
                Price = x.Price,
                ImageUrl = x.ImageUrl,
                IsAvailable = x.IsAvailable
            });
        }
    }
}
