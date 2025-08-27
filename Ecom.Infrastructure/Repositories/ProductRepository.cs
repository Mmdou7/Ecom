using AutoMapper;
using Ecom.Core.DTOs;
using Ecom.Core.Entities.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IImageManagementService imageManagementService;

        public ProductRepository(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageManagementService = imageManagementService;
        }

        public async Task<bool> AddAsync(AddProductDTO productDTO)
        {
            if (productDTO is null)
                return false;
            var product = mapper.Map<Product>(productDTO);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            var ImagePath = await imageManagementService.AddImageAsync(productDTO.Photo, productDTO.Name);

            var Photo = ImagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = product.Id
            }).ToList();
            await context.Photos.AddRangeAsync(Photo);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(UpdateProductDTO updateProductDTO)
        {
            if (updateProductDTO is null)
                return false;
            var findProduct = await context.Products
                .Include(m => m.Category)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.Id == updateProductDTO.Id);

            if (findProduct is null)
                return false;

            mapper.Map(updateProductDTO, findProduct);

            var FindPhoto = await context.Photos.Where(x => x.ProductId == updateProductDTO.Id).ToListAsync();
            foreach(var item in FindPhoto)
            {
                imageManagementService.DeleteImageAsync(item.ImageName);
            }
            context.Photos.RemoveRange(FindPhoto);

            var ImagePath = await imageManagementService.AddImageAsync(updateProductDTO.Photo, updateProductDTO.Name);

            var Photo = ImagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = updateProductDTO.Id
            }).ToList();

            await context.Photos.AddRangeAsync(Photo);
            await context.SaveChangesAsync();
            return true;    
        }
    }
}
