using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        private int UserId =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        [HttpPost]
        public async Task<IActionResult> Add(AddToCartDto dto)
        {
            return Ok(await _service.AddToCartAsync(UserId, dto));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.GetCartAsync(UserId));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            return Ok(await _service.RemoveAsync(UserId, id));
        }

        [HttpPut("{id}/{qty}")]
        public async Task<IActionResult> UpdateQty(int id, int qty)
        {
            return Ok(await _service.UpdateQtyAsync(UserId, id, qty));
        }
    }
}

