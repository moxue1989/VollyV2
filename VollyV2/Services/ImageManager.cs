﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace VollyV2.Services
{
    public class ImageManager : IImageManager
    {
        private static readonly string StorageName = "vollyblobstorage";
        private static readonly string StorageApiKey = Environment.GetEnvironmentVariable("storage_api");

        public async Task<string> UploadImageAsync(IFormFile image, string imageName)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    StorageName,StorageApiKey), true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("images");
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(imageName);

            await blob.DeleteIfExistsAsync();
            CloudBlobStream blobStream = blob.OpenWriteAsync().Result;

            await image.CopyToAsync(blobStream);
            await blobStream.CommitAsync();

            return blob.Uri.ToString();
        }
    }
}
