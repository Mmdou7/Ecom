using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTOs;
using Ecom.Core.Entities.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Ecom.API.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await work.CategoryRepository.GetAllAsync();
                if(categories is null)
                    return NotFound( new ResponseAPI(404));
                return Ok(categories);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-by-Id/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var category = await work.CategoryRepository.GetByIdAsync(id);
                if(category is null)
                    return NotFound(new ResponseAPI(404,$"no category with id = {id}"));
                return Ok(category);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory(CategoryDTO categoryDTO)
        {
            try
            {
                //var category = new Category()
                //{
                //    Name = categoryDTO.Name,
                //    Description = categoryDTO.Description
                //};
                var category = mapper.Map<Category>(categoryDTO);
                await work.CategoryRepository.AddAsync(category);

                return Ok( new ResponseAPI(200,"item has beeb added"));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPut("update-category")]
        public async Task<IActionResult> UpdateCategoryDTO(UpdateCategoryDTO categoryDTO)
        {
            try
            {
                //var category = new Category()
                //{
                //    Name = categoryDTO.Name,
                //    Description = categoryDTO.Description,
                //    Id = categoryDTO.id
                //};
                var category = mapper.Map<Category>(categoryDTO);
                await work.CategoryRepository.UpdateAsync(category);
                return Ok(new ResponseAPI(200, "item has beeb updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("delete-category/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await work.CategoryRepository.DeleteAsync(id);
                return Ok(new ResponseAPI(200, "item has beeb Deleted"));
                //cmnt
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
