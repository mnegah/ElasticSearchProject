using ElasticSearchSampleProject.Core.Entities;
using ElasticSearchSampleProject.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElasticSearchSampleProject.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _context;

        public ProductRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Products>> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            return await _context.Products.AsNoTracking() // AsNoTracking
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Products>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.Products.AsNoTracking() // AsNoTracking
                .Skip((pageNumber - 1) * pageSize) // Optimized Pagination 11
                .Take(pageSize) //from 11 to 20
                .ToListAsync(cancellationToken);
        }

        // 3. Compiled Query
        private static readonly Func<ApplicationDBContext, int, CancellationToken, Task<Products>> _compiledGetByIdQuery =
            EF.CompileAsyncQuery((ApplicationDBContext ctx, int id, CancellationToken cancellationToken) =>
                ctx.Products.AsNoTracking().FirstOrDefault(p => p.ProductID == id));

        public async Task<Products> GetProductByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _compiledGetByIdQuery(_context, id, cancellationToken);
        }

        // 5. Batching with ExecuteSqlRaw
        public async Task UpdateProductPriceAsync(int categoryId, decimal increaseAmount, CancellationToken cancellationToken)
        {
            await _context.Products
                .Where(p => p.CategoryID == categoryId)
                .ExecuteUpdateAsync(p => p.SetProperty(p => p.Price, p => p.Price + increaseAmount), cancellationToken);
        }

        // Add Product Method
        public async Task AddProductAsync(Products product, CancellationToken cancellationToken)
        {
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
