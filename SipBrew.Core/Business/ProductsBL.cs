using FluentValidation;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SipBrew.Core.Common;
using SipBrew.Core.Contracts;
using SipBrew.Core.DTO;
using SipBrew.Core.Models;
using SipBrew.Core.Validators;

namespace SipBrew.Core.Business
{
    public class ProductsBL : IProductsBL
    {
        private readonly AppDbContext _ctx;
        private readonly int _maxPageSize;

        public ProductsBL(AppDbContext ctx, IOptions<AppSettings> options)
        {
            _ctx = ctx;
            _maxPageSize = options.Value.MaxPageSize;
        }

        public async Task<PageResponseDTO<ProductsModel>> GetProducts(ProductFilterDTO filter, PageRequestDTO pageRequest)
        {
            var pageSize = pageRequest.PageSize > _maxPageSize
                ? _maxPageSize
                : pageRequest.PageSize;

            var query = _ctx.Products.AsQueryable();

            query = FilteredEntities(filter, query);

            var totalCount = await query.CountAsync();

            query = OrderEntities(query, pageRequest.sortBy, pageRequest.isAscending);

            var data = await query
                .Skip((pageRequest.PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResponseDTO<ProductsModel>
            {
                Data = data,
                TotalCount = totalCount,
                PageNumber = pageRequest.PageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductsModel> CreateProduct(AddProductDTO dto)
        {
            var imageUrl = await UploadImage(dto.File);

            var entity = new ProductsModel
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = imageUrl,
                IsAvailable = true
            };

            _ctx.Products.Add(entity);
            await _ctx.SaveChangesAsync();

            return entity;
        }

        public async Task<ProductsModel> UpdateProduct(ProductsModel model)
        {
            var existing = await _ctx.Products.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (existing == null)
                throw new Exception("Product not found.");

            var validator = new UpdateProductValidator(_ctx);
            var validation = validator.Validate(model);

            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            existing.ProductName = model.ProductName;
            existing.Description = model.Description;
            existing.Price = model.Price;
            existing.ImageUrl = model.ImageUrl;
            existing.IsAvailable = model.IsAvailable;

            await _ctx.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var existing = await _ctx.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return false;

            _ctx.Products.Remove(existing);
            await _ctx.SaveChangesAsync();

            return true;
        }

        #region HELPERS
        public async Task<string?> UploadImage(IFormFile? file)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.FileName))
                return null;

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var folderPath = Path.Combine("wwwroot", "images");

            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{fileName}";
        }

        public IQueryable<ProductsModel> FilteredEntities(ProductFilterDTO filter, IQueryable<ProductsModel> query)
        {
            var predicate = PredicateBuilder.New<ProductsModel>(true);

            if (filter.Id.HasValue && filter.Id != 0)
                predicate = predicate.And(x => x.Id == filter.Id);

            if (!string.IsNullOrEmpty(filter.ProductName))
                predicate = predicate.And(x => x.ProductName!.ToLower().Contains(filter.ProductName.ToLower()));

            if (filter.Price.HasValue)
                predicate = predicate.And(x => x.Price == filter.Price);

            if (filter.IsAvailable.HasValue)
                predicate = predicate.And(x => x.IsAvailable == filter.IsAvailable);

            return query.Where(predicate);
        }

        public static IQueryable<ProductsModel> OrderEntities(IQueryable<ProductsModel> query, string? sortOrder, bool isAscending)
        {
            if (string.IsNullOrEmpty(sortOrder))
                return query.OrderBy(x => x.Id);

            switch (sortOrder.ToUpper())
            {
                case "ID":
                    return isAscending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);

                case "PRODUCTNAME":
                    return isAscending ? query.OrderBy(x => x.ProductName) : query.OrderByDescending(x => x.ProductName);

                case "PRICE":
                    return isAscending ? query.OrderBy(x => x.Price) : query.OrderByDescending(x => x.Price);

                default:
                    return query.OrderBy(x => x.Id);
            }
        }
        #endregion
    }
}
