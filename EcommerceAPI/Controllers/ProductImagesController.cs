using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _service;

        public ProductImagesController(IProductImageService service)
        {
            _service = service;
        }


        // Upload Multiple Images
        [HttpPost("{productId}")]
        public async Task<IActionResult> Upload(
            int productId,
            [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("Please select images.");

            var result = await _service.UploadAsync(productId, files);

            return Ok(result);
        }

        // Get Images By Product Id
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetImages(int productId)
        {
            var images = await _service.GetByProductIdAsync(productId);
            return Ok(images);
        }

        // Delete Image
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return Ok("Image deleted successfully.");
        }
    }
}
