using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using ElasticSearchSampleProject.Application.Features;
using ElasticSearchSampleProject.Application.Services;
using ElasticSearchSampleProject.Core.Interfaces;
using ElasticSearchSampleProject.Infrastructure;
using ElasticSearchSampleProject.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var elasticSettings = new ElasticsearchClientSettings(new Uri(builder.Configuration["Elasticsearch:Url"]))
    .Authentication(new BasicAuthentication(builder.Configuration["Elasticsearch:username"], builder.Configuration["Elasticsearch:Password"]))
    .ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);  // Bypass SSL validation for development

// Register the Elasticsearch client as a singleton
builder.Services.AddSingleton(new ElasticsearchClient(elasticSettings));
builder.Services.AddScoped<SearchProductsHandler>();


builder.Services.AddScoped<IProductService, ProductService>();


// Configure DbContext for the Northwind database
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register ProductRepository
builder.Services.AddScoped<IProductRepository,ProductRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
