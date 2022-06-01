using FileUploading.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploading.Backend.Core.Interfaces
{
    public interface IUploaderService
    {
        /// <summary>
        /// save file to database.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<bool> UploadFileAsync(FileUploaded file);
    }
}
