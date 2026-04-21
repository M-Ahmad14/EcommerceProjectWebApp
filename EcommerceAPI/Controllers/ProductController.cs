using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
           
            _service = service;

        }

        //GET ALL PRODUCTS
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var product = await _service.GetAllAsync();
            return Ok(product);
        }


        // GET BY ID
       // [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            
            return Ok(product);
        }

        //ADD PRODUCT
       // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            var product = await _service.CreateAsync(dto);
            return Ok(product);
        }

        //UPDATE PRODUCT
       // [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto dto)
        {
            var product = await _service.UpdateAsync(id, dto);
            
            return Ok(product);
        }

        // DELETE
       // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            
            return Ok("Deleted");
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] ProductQueryDto query)
        {
            var products = await _service.GetFilteredAsync(query);

            return Ok(products);
        }
    }
}
