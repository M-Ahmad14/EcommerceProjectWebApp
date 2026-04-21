using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetByProductIdAsync(int productId);

        Task<string> CreateAsync(int userId, CreateReviewDto dto);

        Task<double> GetAverageRatingAsync(int productId);
    }
}
