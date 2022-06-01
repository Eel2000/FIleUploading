using FileUploading.Backend.Core.Contexts;
using FileUploading.Backend.Core.Interfaces;
using FileUploading.Shared.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploading.Backend.Core.Services
{
    public class UploaderService : IUploaderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UploaderService> _logger;

        public UploaderService(ApplicationDbContext context, ILogger<UploaderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> UploadFileAsync(FileUploaded file)
        {
            try
            {
                await _context.FilesUploaded.AddAsync(file);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(500, "Server Error"), e, "An error occured while processing");
                return false;
            }
        }
    }
}
