using Ecom.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories.Service
{
    public class ImageManagementService : IImageManagementService
    {
        private readonly IFileProvider fileProvider;
        public ImageManagementService(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }

        public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
        {
            List<string> SaveImageSrc = new List<string>();

            var ImageDirectory = Path.Combine("wwwroot", "Images", src);

            if (Directory.Exists(ImageDirectory) is not true)
                Directory.CreateDirectory(ImageDirectory);

            foreach(var item in files)
            {
                if(item.Length > 0)
                {
                    var ImageName = item.FileName;
                    var ImageSrc = $"Images/{src}/{ImageName}";

                    var Root = Path.Combine(ImageDirectory, ImageName); //for saving imgs

                    using(FileStream stream = new FileStream(Root , FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }

                    SaveImageSrc.Add(ImageSrc);
                }
            }
            return SaveImageSrc;
        }

        public void DeleteImageAsync(string src)
        {
            var Info = fileProvider.GetFileInfo(src);
            var Root = Info.PhysicalPath;
            File.Delete(Root);
            //File.Delete(fileProvider.GetFileInfo(src).PhysicalPath);
        }
    }
}
