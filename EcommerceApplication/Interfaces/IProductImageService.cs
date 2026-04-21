using Application.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductImageService
    {
        Task<List<string>> UploadAsync(int productId, List<IFormFile> files);
        Task<IEnumerable<ProductImageDto>> GetByProductIdAsync(int productId);
        Task<bool> DeleteAsync(int id);

    }
}
