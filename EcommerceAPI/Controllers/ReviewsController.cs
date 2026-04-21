using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewsController(IReviewService service)
        {
            _service = service;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(int productId)
        {
            return Ok(await _service.GetByProductIdAsync(productId));
        }

        [HttpGet("average/{productId}")]
        public async Task<IActionResult> Average(int productId)
        {
            return Ok(await _service.GetAverageRatingAsync(productId));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateReviewDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier).Value
            );

            var result = await _service.CreateAsync(userId, dto);

            return Ok(result);
        }
    }
}
