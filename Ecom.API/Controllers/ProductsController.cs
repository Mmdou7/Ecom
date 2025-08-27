using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTOs;
using Ecom.Core.Entities.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await work.ProductRepository.GetAllAsync(x=>x.Category ,x=>x.Photos);
                var result = mapper.Map<List<ProductDTO>>(products);
                if (products is null)
                    return NotFound(new ResponseAPI(400));
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseAPI(404,message:"Error"));
            }
        }
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await work.ProductRepository.GetByIdAsync(id, x=>x.Category,x=>x.Photos);
                var result = mapper.Map<ProductDTO>(product);
                if (result is null)
                    return BadRequest(new ResponseAPI(400));
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct(AddProductDTO product)
        {
            try
            {
                await work.ProductRepository.AddAsync(product);
                return Ok( new ResponseAPI(200));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct(UpdateProductDTO updateProductDTO)
        {
            try
            {
                await work.ProductRepository.UpdateAsync(updateProductDTO);
                return Ok(new ResponseAPI(200));

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
