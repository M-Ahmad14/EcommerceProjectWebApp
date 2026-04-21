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
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddToCartAsync(int userId, AddToCartDto dto)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ProductId == dto.ProductId);

            if (item != null)
            {
                item.Quantity += dto.Quantity;
            }
            else
            {
                item = new CartItem
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };

                _context.CartItems.Add(item);
            }

            await _context.SaveChangesAsync();

            return "Added to cart";
        }

        public async Task<IEnumerable<CartDto>> GetCartAsync(int userId)
        {
            var items = await _context.CartItems
                .Include(x => x.Product)
                    .ThenInclude(p => p.Images)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return items.Select(x => new CartDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Price = x.Product.Price,
                Quantity = x.Quantity,
                Images = x.Product.Images
                    .Select(i => i.ImageUrl)
                    .ToList()
            });
        }

        public async Task<string> RemoveAsync(int userId, int cartItemId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.Id == cartItemId &&
                    x.UserId == userId);

            if (item == null)
                return "Item not found";

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();

            return "Removed";
        }

        public async Task<string> UpdateQtyAsync(int userId, int cartItemId, int qty)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.Id == cartItemId &&
                    x.UserId == userId);

            if (item == null)
                return "Item not found";

            item.Quantity = qty;

            await _context.SaveChangesAsync();

            return "Updated";
        }
    }
}
