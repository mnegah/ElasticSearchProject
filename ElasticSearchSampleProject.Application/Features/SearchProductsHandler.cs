using Elastic.Clients.Elasticsearch;
using ElasticSearchSampleProject.Core.Models;

namespace ElasticSearchSampleProject.Application.Features
{
    // ApplicationLayer/Features/SearchProductsHandler.cs

    public class SearchProductsHandler
    {
        private readonly ElasticsearchClient _elasticClient;

        public SearchProductsHandler(ElasticsearchClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<List<ProductSearchResult>> SearchProductsAsync(string searchTerm)
        {
            var response = await _elasticClient.SearchAsync<ProductSearchResult>(s => s
                .Index("products")
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(new[] { "productName","categoryName" }) // Correct usage of Fields
                        .Query(searchTerm)
                    )
                )
            );

            if (!response.IsValidResponse)
            {
                return new List<ProductSearchResult>();//throw new Exception("Failed to search products in ElasticSearch.");
            }

            return response.Documents.ToList();
        }
    }

}
