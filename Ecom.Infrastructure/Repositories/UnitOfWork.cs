using AutoMapper;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageManagementService _imageManagementService;

        public ICategoryRepository CategoryRepository { get; }

        public IPhotoRepository PhotoRepository { get; }

        public IProductRepository ProductRepository { get; }

        public UnitOfWork(AppDbContext context, IImageManagementService imageManagementService, IMapper mapper)
        {
            _context = context;
            _imageManagementService = imageManagementService;
            _mapper = mapper;
            CategoryRepository = new CategoryRepository(_context);
            PhotoRepository = new PhotoRepository(_context);
            ProductRepository = new ProductRepository(_context, _mapper , _imageManagementService);
            
        }

    }
}
