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

        public async Task<bool> UploadFileToAzurAsync(string id, MultipartFormDataContent fileUploaded)
        {
            try
            {
                var response = await _client.PostAsync($"https://localhost:7123/api/FileManage/upload-blob-file?id={id}", fileUploaded);
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

        public async Task<IReadOnlyList<FileUploaded>> GetFileUploadedsAsync()
        {
            try
            {
                var response = await _client.GetFromJsonAsync<IReadOnlyList<FileUploaded>>("https://localhost:7123/api/FileManage/list-files");
                return response!;
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(500, "Client Call API Error"), e, "Error occured while calling [list-files]");
                return new List<FileUploaded>();
            }
        }
    }
}
