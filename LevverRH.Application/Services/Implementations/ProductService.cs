using LevverRH.Application.DTOs.Common;
using LevverRH.Application.DTOs.Product;
using LevverRH.Application.Services.Interfaces;
using LevverRH.Domain.Interfaces;

namespace LevverRH.Application.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductCatalogRepository _productCatalogRepository;
    private readonly ITenantProductRepository _tenantProductRepository;

    public ProductService(
        IProductCatalogRepository productCatalogRepository,
        ITenantProductRepository tenantProductRepository)
    {
        _productCatalogRepository = productCatalogRepository;
        _tenantProductRepository = tenantProductRepository;
    }

    public async Task<ResultDTO<IEnumerable<ProductDTO>>> GetAllProductsAsync()
    {
        try
        {
            var products = await _productCatalogRepository.GetAtivosAsync();

            var productsDTO = products
                .OrderBy(p => p.OrdemExibicao)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    ProdutoNome = p.ProdutoNome,
                    Descricao = p.Descricao,
                    Categoria = p.Categoria,
                    Icone = p.Icone,
                    CorPrimaria = p.CorPrimaria,
                    RotaBase = p.RotaBase,
                    OrdemExibicao = p.OrdemExibicao,
                    Lancado = p.Lancado,
                    Ativo = p.Ativo
                });

            return new ResultDTO<IEnumerable<ProductDTO>>
            {
                Success = true,
                Data = productsDTO
            };
        }
        catch (Exception ex)
        {
            return new ResultDTO<IEnumerable<ProductDTO>>
            {
                Success = false,
                Message = "Erro ao buscar produtos.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResultDTO<IEnumerable<TenantProductDTO>>> GetTenantProductsAsync(Guid tenantId)
    {
        try
        {
            var tenantProducts = await _tenantProductRepository.GetByTenantIdAsync(tenantId);

            var tenantProductsDTO = tenantProducts
                .OrderBy(tp => tp.Product.OrdemExibicao)
                .Select(tp => new TenantProductDTO
                {
                    ProductId = tp.ProductId,
                    ProdutoNome = tp.Product.ProdutoNome,
                    Descricao = tp.Product.Descricao,
                    Categoria = tp.Product.Categoria,
                    Icone = tp.Product.Icone,
                    CorPrimaria = tp.Product.CorPrimaria,
                    RotaBase = tp.Product.RotaBase,
                    OrdemExibicao = tp.Product.OrdemExibicao,
                    Lancado = tp.Product.Lancado,
                    AcessoAtivo = tp.Ativo,
                    DataAtivacao = tp.DataAtivacao
                });

            return new ResultDTO<IEnumerable<TenantProductDTO>>
            {
                Success = true,
                Data = tenantProductsDTO
            };
        }
        catch (Exception ex)
        {
            return new ResultDTO<IEnumerable<TenantProductDTO>>
            {
                Success = false,
                Message = "Erro ao buscar produtos do tenant.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ResultDTO<bool>> HasAccessToProductAsync(Guid tenantId, Guid productId)
    {
        try
        {
            var hasAccess = await _tenantProductRepository.HasAccessToProductAsync(tenantId, productId);

            return new ResultDTO<bool>
            {
                Success = true,
                Data = hasAccess
            };
        }
        catch (Exception ex)
        {
            return new ResultDTO<bool>
            {
                Success = false,
                Message = "Erro ao verificar acesso ao produto.",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
