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
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDto>> GetByProductIdAsync(int productId)
        {
            var reviews = await _context.Reviews
                .Include(x => x.User)
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Comment = r.Comment,
                UserName = r.User.Name,
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<string> CreateAsync(int userId, CreateReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return "Rating must be between 1 and 5";

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.ProductId == dto.ProductId);

            if (alreadyReviewed)
                return "You already reviewed this product";

            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return "Review added";
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            var ratings = await _context.Reviews
                .Where(x => x.ProductId == productId)
                .Select(x => x.Rating)
                .ToListAsync();

            if (!ratings.Any()) return 0;

            return ratings.Average();
        }
    }
}
