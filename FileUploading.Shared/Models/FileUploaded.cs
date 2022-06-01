using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploading.Shared.Models
{
    public class FileUploaded
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Data { get; set; }
        public string? Name { get; set; }
        public long Taille { get; set; }
        public string? Type { get; set; }
        public DateTime UploadeOn { get; set; } = DateTime.Now;
    }
}
