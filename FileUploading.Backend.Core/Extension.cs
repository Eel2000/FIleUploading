using FileUploading.Backend.Core.Contexts;
using FileUploading.Backend.Core.Interfaces;
using FileUploading.Backend.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploading.Backend.Core
{
    public static class Extension
    {
        public static void AddFileUploadingCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
#pragma warning disable CS8604 // Possible null reference argument.
                option.UseSqlServer(connectionString: configuration.GetConnectionString("Default"));
#pragma warning restore CS8604 // Possible null reference argument.
            });

            services.AddTransient<IUploaderService, UploaderService>();
        }
    }
}
