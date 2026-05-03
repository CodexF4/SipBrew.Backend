using Microsoft.AspNetCore.Http;
using SipBrew.Core.Common;
using SipBrew.Core.DTO;
using SipBrew.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SipBrew.Core.Contracts
{
    public interface IProductsBL
    {
        Task<PageResponseDTO<ProductsModel>> GetProducts(ProductFilterDTO filter, PageRequestDTO pageRequest);
        Task<ProductsModel> CreateProduct(AddProductDTO dto);
        Task<ProductsModel> UpdateProduct(ProductsModel model);
        Task<bool> DeleteProduct(int id);
    }
}
