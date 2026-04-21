using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<string> AddToCartAsync(int userId, AddToCartDto dto);

        Task<IEnumerable<CartDto>> GetCartAsync(int userId);

        Task<string> RemoveAsync(int userId, int cartItemId);

        Task<string> UpdateQtyAsync(int userId, int cartItemId, int qty);
    }
}
