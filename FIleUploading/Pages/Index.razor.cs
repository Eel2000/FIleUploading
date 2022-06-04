using FileUploading.Shared.Models;
using FIleUploading.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;

namespace FIleUploading.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] IJSRuntime? jS { get; set; }
        [Inject] ServerService? server { get; set; }

        private bool isUploading = false;
        private bool isLoading = false;

        private long baseUnit = 1024 * 1014;

        private ObservableCollection<FileUploaded> files = new();

        protected override async Task OnInitializedAsync()
        {
            isUploading = !isUploading;
            //TODO: check first if there is data in the storage before calling the api.
            //TODO: Add sqlite to the application to allow fast data loading⚠
            var data = await server.GetFileUploadedsAsync();
            files = new ObservableCollection<FileUploaded>(data.OrderByDescending(x => x.UploadeOn));
            isUploading = !isUploading;
        }


        private async Task OnInputFileChange(InputFileChangeEventArgs args)
        {
            isUploading = !isUploading;
            try
            {
                foreach (var file in args.GetMultipleFiles(3))
                {
                    var buffer = new byte[file.Size];
                    using var fileStream = file.OpenReadStream(file.Size);
                    //string sourceBase64 = Convert.ToBase64String(buffer);
                    using var content = new MultipartFormDataContent();

                    var fileContent = new StreamContent(fileStream);

                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                    content.Add(content: fileContent, name: "\"file\"", fileName: file.Name);

                    await server.UploadFileToAzurAsync(Guid.NewGuid().ToString(), content);
                }
                isUploading = !isUploading;
                await ReloadVideoListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                isUploading = !isUploading;
            }

        }

        private async Task ReloadVideoListAsync()
        {
            isUploading = !isUploading;
            var data = await server.GetFileUploadedsAsync();
            files = new ObservableCollection<FileUploaded>(data.OrderByDescending(x => x.UploadeOn));
            isUploading = !isUploading;
        }

        private async void OnVideoSelected(string videoId)
        {
            isLoading = !isLoading;
            var toWatch = files.FirstOrDefault(x => x.Id == videoId);
            if (toWatch is not null)
            {
                //await jS.InvokeVoidAsync("loadVideo", toWatch.Data, toWatch.Type);
                await jS.InvokeVoidAsync("readVideo", toWatch.Data, toWatch.Type);
            }
            isLoading = !isLoading;
        }

    }
}
