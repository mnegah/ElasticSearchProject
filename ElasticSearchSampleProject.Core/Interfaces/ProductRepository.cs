using ElasticSearchSampleProject.Core.Entities;

namespace ElasticSearchSampleProject.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Products>> GetAllProductsAsync(CancellationToken cancellationToken);
        Task<List<Products>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Products> GetProductByIdAsync(int id, CancellationToken cancellationToken);
        Task AddProductAsync(Products product, CancellationToken cancellationToken);
        Task UpdateProductPriceAsync(int categoryId, decimal price, CancellationToken cancellationToken);
    }
}
