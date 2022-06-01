using FileUploading.Shared.Models;
using FIleUploading.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;

namespace FIleUploading.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] IJSRuntime? jS { get; set; }
        [Inject] ServerService? server { get; set; }

        private List<FileUploaded> files = new();

        private async Task OnInputFileChange(InputFileChangeEventArgs args)
        {
            try
            {
                foreach (var file in args.GetMultipleFiles(3))
                {
                    var buffer = new byte[file.Size];
                    await file.OpenReadStream(50000000).ReadAsync(buffer);
                    string sourceBase64 = Convert.ToBase64String(buffer);

                    FileUploaded newUpload = new()
                    {
                        Data = sourceBase64,
                        Type = file.ContentType,
                        Name = file.Name,
                        Taille = file.Size
                    };
                    files.Add(newUpload);

                    await server.UploadFileAsync(newUpload);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }

        private async void OnVideoSelected(string videoId)
        {
            var toWatch = files.FirstOrDefault(x => x.Id == videoId);
            if (toWatch is not null)
            {
                await jS.InvokeVoidAsync("loadVideo", toWatch.Data, toWatch.Type);
            }
        }

        private async Task OnProgressAsync(ProgressEventArgs args)
        {
            //_logger.LogInformation($"{args.Loaded}");
        }

    }
}
