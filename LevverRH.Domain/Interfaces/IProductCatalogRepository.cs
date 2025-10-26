using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevverRH.Domain.Entities;
using LevverRH.Domain.Enums;

namespace LevverRH.Domain.Interfaces
{
    public interface IProductCatalogRepository : IRepository<ProductCatalog>
    {
        Task<IEnumerable<ProductCatalog>> GetAtivosPorCategoriaAsync(string categoria);
        Task<ProductCatalog?> GetByNomeAsync(string nome);
        Task<IEnumerable<ProductCatalog>> GetByModeloCobrancaAsync(ModeloCobranca modelo);
        Task<IEnumerable<ProductCatalog>> GetAtivosAsync();
    }
}
