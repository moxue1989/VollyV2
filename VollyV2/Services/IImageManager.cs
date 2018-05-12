using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace VollyV2.Services
{
    public interface IImageManager
    {
        Task<string> UploadImageAsync(IFormFile image, string imageName);
    }
}
