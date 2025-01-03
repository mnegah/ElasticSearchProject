using Elastic.Clients.Elasticsearch;
using ElasticSearchSampleProject.Infrastructure.Repositories;

namespace ElasticSearchSampleProject.Infrastructure.Services
{
    public class ProductIndexingService
    {
        private readonly ElasticsearchClient _elasticClient;
        private readonly ProductRepository _productRepository;

        public ProductIndexingService(ElasticsearchClient elasticClient, ProductRepository productRepository)
        {
            _elasticClient = elasticClient;
            _productRepository = productRepository;
        }

        public async Task IndexProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync(CancellationToken.None);

            foreach (var product in products)
            {
                await _elasticClient.IndexAsync(product, idx => idx.Index("products"));
            }
        }
    }
}
