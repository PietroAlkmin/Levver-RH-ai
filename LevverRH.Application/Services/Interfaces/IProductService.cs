using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Product;

namespace LevverRH.Application.Services.Interfaces;

public interface IProductService
{
    Task<ResultDTO<IEnumerable<ProductDTO>>> GetAllProductsAsync();
    Task<ResultDTO<IEnumerable<TenantProductDTO>>> GetTenantProductsAsync(Guid tenantId);
    Task<ResultDTO<bool>> HasAccessToProductAsync(Guid tenantId, Guid productId);
}
