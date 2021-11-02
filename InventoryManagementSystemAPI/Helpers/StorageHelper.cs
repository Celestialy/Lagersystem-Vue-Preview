using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using InventoryManagementSystemAPI.Database;
using InventoryManagementSystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.Helpers
{
    public class StorageHelper
    {
        public bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private BlobServiceClient _client;

        public StorageHelper(BlobServiceClient client)
        {
            _client = client;
        }

        private BlobClient blobClient;

        public async Task<Uri> UploadFileToStorage(string fileName, string contentType, string containerName)
        {

            var storageFileName = Guid.NewGuid() + DateTime.Now.ToString("dd-MM-yyyy") + '.' + fileName.Split('.').Last();


            BlobContainerClient bob = _client.GetBlobContainerClient(containerName);
            bob.CreateIfNotExistsAsync().Wait();
            blobClient = bob.GetBlobClient(storageFileName);


            await Task.Delay(1);

            return blobClient.Uri;
        }

        public async Task UploadImage(Stream fileStream)
        {
            // Upload the file
            await blobClient.UploadAsync(fileStream, true);
        }

        public async Task<bool> DeleteBlob(Uri uri)
        {
            blobClient = new BlobClient(uri, new DefaultAzureCredential());
            await blobClient.DeleteIfExistsAsync();

            return true;
        }
    }
}