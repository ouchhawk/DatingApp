using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interface;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        // servisi alıyor configüre edip geri döndürüyor
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // bu kısmı startup.cs ten aldık
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options => 
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}