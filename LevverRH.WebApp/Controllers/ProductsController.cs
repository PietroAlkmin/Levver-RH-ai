using LevverRH.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevverRH.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Retorna todos os produtos disponíveis no catálogo
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var result = await _productService.GetAllProductsAsync();

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Retorna os produtos que o tenant logado tem acesso
    /// </summary>
    [HttpGet("my-products")]
    public async Task<IActionResult> GetMyProducts()
    {
        // Extrair tenant_id do token JWT
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;

        if (string.IsNullOrEmpty(tenantIdClaim))
            return Unauthorized("Tenant não identificado");

        var tenantId = Guid.Parse(tenantIdClaim);

        var result = await _productService.GetTenantProductsAsync(tenantId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Verifica se o tenant tem acesso a um produto específico
    /// </summary>
    [HttpGet("has-access/{productId}")]
    public async Task<IActionResult> HasAccessToProduct(Guid productId)
    {
        // Extrair tenant_id do token JWT
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;

        if (string.IsNullOrEmpty(tenantIdClaim))
            return Unauthorized("Tenant não identificado");

        var tenantId = Guid.Parse(tenantIdClaim);

        var result = await _productService.HasAccessToProductAsync(tenantId, productId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
