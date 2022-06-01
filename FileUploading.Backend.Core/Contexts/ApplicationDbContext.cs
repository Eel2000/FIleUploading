using FileUploading.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploading.Backend.Core.Contexts
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> optionsBuilder) : base(optionsBuilder)
        {

        }

        public DbSet<FileUploaded>? FilesUploaded { get; set; }
    }
}
