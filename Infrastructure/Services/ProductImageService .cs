using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Services
{
    public class ProductImageService: IProductImageService
    {
        private readonly IRepository<ProductImage> _repo;
        private readonly IWebHostEnvironment _env;

        public ProductImageService(
            IRepository<ProductImage> repo,
            IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }
        public async Task<List<string>> UploadAsync(int productId, List<IFormFile> files)
        {
            var result = new List<string>();

            var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var folder = Path.Combine(root, "images");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                    var filePath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var image = new ProductImage
                    {
                        ProductId = productId,
                        ImageUrl = "/images/" + fileName
                    };

                    await _repo.AddAsync(image);

                    result.Add(image.ImageUrl);
                }
            }

            return result;
        }

        public async Task<IEnumerable<ProductImageDto>> GetByProductIdAsync(int productId)
        {
            var images = await _repo.FindAsync(x => x.ProductId == productId);

            return images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var image = await _repo.GetByIdAsync(id);

            if (image == null) return false;

            _repo.Delete(image);

            return true;
        }
    }
}
