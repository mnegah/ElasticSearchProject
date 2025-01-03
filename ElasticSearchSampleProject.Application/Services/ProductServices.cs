using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using ElasticSearchSampleProject.Application.Features;
using ElasticSearchSampleProject.Core.Entities;
using ElasticSearchSampleProject.Core.Interfaces;
using ElasticSearchSampleProject.Core.Models;

namespace ElasticSearchSampleProject.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly SearchProductsHandler _serachProducthandler;

        private readonly ElasticsearchClient _elasticClient;

        public ProductService(IProductRepository productRepository,SearchProductsHandler serachProducthandler ,ElasticsearchClient elasticClient)
        {
            _productRepository = productRepository;
            _serachProducthandler = serachProducthandler;
            _elasticClient = elasticClient;
        }

        // Optimized pagination is now implemented in the repository
        public async Task<List<Products>> GetPaginatedProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductsAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task UpdateProductPricesAsync(int categoryId, decimal increaseAmount, CancellationToken cancellationToken)
        {
            await _productRepository.UpdateProductPriceAsync(categoryId, increaseAmount, cancellationToken);
        }

        public async Task<Products> GetProductByIdAsync(int productId, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductByIdAsync(productId, cancellationToken);
        }

        public async Task AddproductAsync(Products product, CancellationToken cancellationToken)
        {
            await _productRepository.AddProductAsync(product, cancellationToken);
        }

        public async Task SyncProductsToElasticsearch()
        {
            var products = await _productRepository.GetAllProductsAsync(CancellationToken.None);

        }

        public async Task<List<ProductSearchResult>> IndexProductsAsync(string searchTerm)
        {

            // First search in ElasticSearch
            var results = await _serachProducthandler.SearchProductsAsync(searchTerm);
            // If no results are found in ElasticSearch, fall back to the database
            if (!results.Any())
            {
                //Index data into the Elastic Database
                var products = await _productRepository.GetAllProductsAsync(CancellationToken.None);

                var bulkRequest = new BulkRequest("products")
                {
                    Operations = new BulkOperationsCollection(
                                     products.Select(p => new BulkIndexOperation<Products>(p)).ToList()
                )
                };
                await _elasticClient.BulkAsync(bulkRequest);

                //Read and search data from the original database
                return products
                    .Where(p => p.ProductName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .Select(p => new ProductSearchResult
                    {
                        ProductName = p.ProductName,
                    })
                    .ToList();
            }

            return results;
        }
    }
}
