using FileUploading.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace FIleUploading.Service
{
    public class ServerService
    {
        private readonly HttpClient _client;
        private readonly ILogger<ServerService> _logger;

        public ServerService(HttpClient client, ILogger<ServerService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<bool> UploadFileAsync(FileUploaded fileUploaded)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("https://localhost:7123/api/FileManage/upload-file", fileUploaded);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<bool>(responseJson);
                }
                _logger.LogWarning(await response.Content.ReadAsStringAsync());
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(500, "CLien Call APi Error"), e, "An error accured while calling the api");
                return false;
            }
        }
    }
}
