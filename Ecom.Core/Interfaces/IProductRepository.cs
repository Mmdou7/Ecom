using Ecom.Core.DTOs;
using Ecom.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<bool> AddAsync(AddProductDTO product);
        Task<bool> UpdateAsync(UpdateProductDTO product);
        Task DeleteAsync(Product product);

    }
}
