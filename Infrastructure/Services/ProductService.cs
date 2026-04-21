using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;
        private readonly AppDbContext _context;

        public ProductService(
            IRepository<Product> repository,
            AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .ToListAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,

                Images = p.Images
                    .Select(i => i.ImageUrl)
                    .ToList()
            });
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var p = await _context.Products
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
                return null;

            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,

                Images = p.Images
                    .Select(i => i.ImageUrl)
                    .ToList()
            };
        }
        public async Task<ProductDto> CreateAsync(ProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId
            };

            await _repository.AddAsync(product);

            return dto;
        }

        public async Task<bool> UpdateAsync(int id, ProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;

            _repository.Update(product);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return false;

            _repository.Delete(product);
            return true;
        }

        // Searching, filter, Pagination
        public async Task<IEnumerable<ProductDto>> GetFilteredAsync(ProductQueryDto query)
        {
            var products = _context.Products
                .Include(p => p.Images)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(query.Search))
            {
                products = products.Where(x =>
                    x.Name.Contains(query.Search));
            }

            // Category
            if (query.CategoryId.HasValue)
            {
                products = products.Where(x =>
                    x.CategoryId == query.CategoryId.Value);
            }

            // Min Price
            if (query.MinPrice.HasValue)
            {
                products = products.Where(x =>
                    x.Price >= query.MinPrice.Value);
            }

            // Max Price
            if (query.MaxPrice.HasValue)
            {
                products = products.Where(x =>
                    x.Price <= query.MaxPrice.Value);
            }

            // Sorting
            switch (query.Sort)
            {
                case "priceAsc":
                    products = products.OrderBy(x => x.Price);
                    break;

                case "priceDesc":
                    products = products.OrderByDescending(x => x.Price);
                    break;

                case "latest":
                    products = products.OrderByDescending(x => x.Id);
                    break;
            }

            // Pagination
            products = products
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize);

            var result = await products.ToListAsync();

            return result.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                Images = p.Images.Select(i => i.ImageUrl).ToList()
            });
        }
    }
}
