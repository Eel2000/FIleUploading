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
using Azure.Storage.Blobs.Specialized;

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
            try
            {
                if (file is null) return false;

                if (string.IsNullOrWhiteSpace(conn)) return false;

                _logger.LogInformation("starting to upload to azur....");

                var container = new BlobContainerClient(conn, Constants.CONTAINER_NAME);
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(PublicAccessType.Blob);

                using var fileStream = file.OpenReadStream();
                string uri = default!;

                var fileIsUnder10Mb = file.Length / Constants.M_CONVERT_UNIT;
                if (fileIsUnder10Mb <= 9)
                {
                    _logger.LogInformation("the file's size is under 9 Mb it will be uploaded without being chunked");
                    var name = Util.RemoveStringWhiteSpacesAndBrackets(file.FileName);
                    //TODO: remove all special chars from the name.
                    var blob = container.GetBlobClient(name);
                    await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                    await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });
                    uri = blob.Uri.ToString();
                    _logger.LogInformation("Uploading to azur finished. saving info to database...");
                }

                else if (fileIsUnder10Mb >= 10)
                {
                    _logger.LogInformation("The file's size is above 10 MB it will be chunked and uploaded");
                    var name = Util.RemoveStringWhiteSpacesAndBrackets(file.FileName);
                    //TODO: remove all special chars from the name.
                    BlockBlobClient blockBlob = new BlockBlobClient(conn, Constants.CONTAINER_NAME, name);
                    var chunks = new List<string>();
                    double percent = 0;
                    while (true)
                    {
                        byte[] chunkLength = new byte[Constants.M_CONVERT_UNIT];
                        var chunk = await fileStream.ReadAsync(chunkLength, 0, (int)Constants.M_CONVERT_UNIT);
                        if (chunk == 0) break;

                        var chunkId = Guid.NewGuid().ToString();
                        var base64CvrtdChunkId = Convert.ToBase64String(Encoding.UTF8.GetBytes(chunkId));

                        await blockBlob.StageBlockAsync(base64CvrtdChunkId, new MemoryStream(chunkLength, true),
                            null, null,
                            new Progress<long>(p =>
                            {
                                var value = (p * 100) / file.Length;
                                percent += Convert.ToDouble(value);
                                Console.Write($"\r{percent}%");
                            }));
                        chunks.Add(base64CvrtdChunkId);
                    }
                    await blockBlob.CommitBlockListAsync(chunks);
                    _logger.LogInformation("Committing data");
                    uri = blockBlob.Uri.ToString();
                }


                var newFile = new FileUploaded
                {
                    Data = uri,
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
            catch (Exception e)
            {
                _logger.LogError(new EventId(500, "Server Error"), e, "An error occured while uploading file to azure");
                return false;
            }
        }
    }
}
