using FileUploading.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;

namespace FIleUploading.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] IJSRuntime jS { get; set; }

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

                    files.Add(new()
                    {
                        Data = sourceBase64,
                        Type = file.ContentType,
                        Name = file.Name,
                        Taille = file.Size
                    });
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
