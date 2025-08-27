using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Ecom.Infrastructure.Repositories;
using Ecom.Infrastructure.Repositories.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure
{
    public static class InfrastructureRegisteration
    {
        public static IServiceCollection inftrastructureConfiguration(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IImageManagementService, ImageManagementService>();

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider
                (Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddDbContext<AppDbContext>(op =>
            {
                op.UseSqlServer(configuration.GetConnectionString("Ecom"));
            });

            return services;
        }
    }
}
