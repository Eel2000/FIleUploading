using FileUploading.Backend.Core.Contexts;
using FileUploading.Backend.Core.Interfaces;
using FileUploading.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileUploading.Shared.Utils;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

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

        public async Task<IReadOnlyList<FileUploaded>> GetUploadedFilesAsync()
        {
            var data = from d in _context.FilesUploaded
                       select d;
            await Task.Delay(2);
            return data.ToList();
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

        public async Task<bool> UploadFileToAzureAsync(string id, IFormFile file, string conn)
        {
            if (file is null) return false;

            if (string.IsNullOrWhiteSpace(conn)) return false;

            _logger.LogInformation("starting to upload to azur....");

            var container = new BlobContainerClient(conn, Constants.CONTAINER_NAME);
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);

            var blob = container.GetBlobClient(file.FileName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            using var fileStream = file.OpenReadStream();
            await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });

            _logger.LogInformation("Uploading to azur finished. saving info to database...");

            var newFile = new FileUploaded
            {
                Data = blob.Uri.ToString(),
                Id = id,
                Name = file.FileName,
                Type = file.ContentType,
                Taille = file.Length
            };

            await _context.FilesUploaded.AddAsync(newFile);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Info saved saccessfully");

            return true;
        }
    }
}
