using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveVaultFunctions.Services
{
    public class BlobStorageService
    {
        private CloudBlobClient _cloudBlobClient;
        private CloudBlobContainer _cloudBlobContainer;

        public BlobStorageService()
        {
            var storageConnectionString = System.Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            var storageContainerName = "archive-vault";
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Get a blob client object
            _cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Get the container / folder where we will store files
            _cloudBlobContainer = _cloudBlobClient.GetContainerReference(storageContainerName);

            // Create a new container, if it does not exist
            _cloudBlobContainer.CreateIfNotExistsAsync().Wait();
        }

        public async Task<Guid> AddFileAsync(string fileName, string originalFilePath, Stream fileData, string confidentialityLevel, string retentionPeriod)
        {
            return await this.AddFileAsync(Guid.NewGuid(), fileName, originalFilePath, fileData, confidentialityLevel, retentionPeriod);
        }

        public async Task<Guid> AddFileAsync(Guid fileGuid, string fileName, string originalFilePath, Stream fileData, string confidentialityLevel, string retentionPeriod)
        {
            var blob = _cloudBlobContainer.GetBlockBlobReference(fileGuid.ToString());

            var exists = await blob.ExistsAsync();

            if (exists == false)
            {
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    blob.Metadata.Add("fileName", fileName);
                }

                if (!string.IsNullOrWhiteSpace(originalFilePath))
                {
                    blob.Metadata.Add("originalFilePath", originalFilePath);
                }

                if (!string.IsNullOrWhiteSpace(confidentialityLevel))
                {
                    blob.Metadata.Add("confidentialityLevel", confidentialityLevel);
                }

                if (!string.IsNullOrWhiteSpace(retentionPeriod))
                {
                    blob.Metadata.Add("retentionPeriod", retentionPeriod);
                }

                await blob.UploadFromStreamAsync(fileData);
            }
            else
            {
                throw new ApplicationException($"File with the specified GUID {fileGuid} already exists in blob storage.");
            }

            return fileGuid;
        }

        public async Task<Guid> AddFileAsync(Guid fileGuid, string fileName, string type, byte[] fileData, string confidentialityLevel, string retentionPeriod)
        {
            using (var memStr = new MemoryStream(fileData))
            {
                return await this.AddFileAsync(fileGuid, fileName, type, memStr, confidentialityLevel, retentionPeriod);
            }
        }

        public async Task<bool> FileExists(Guid fileGuid)
        {
            bool fileExists = false;

            if (fileGuid != Guid.Empty)
            {
                var blob = _cloudBlobContainer.GetBlockBlobReference(fileGuid.ToString());
                fileExists = await blob.ExistsAsync();
            }

            return fileExists;
        }

        public async Task<string> GetFileMetadataAsync(Guid fileGuid, string key)
        {
            var blob = _cloudBlobContainer.GetBlockBlobReference(fileGuid.ToString());

            var exists = await blob.ExistsAsync();

            if (exists)
            {
                if (blob.Metadata.ContainsKey(key) == true)
                {
                    return blob.Metadata[key];
                }
                return string.Empty;
            }
            else
            {
                throw new Exception("File not found.");
            }
        }

        public async Task<long> GetFileSize(Guid fileId)
        {
            var blob = _cloudBlobContainer.GetBlockBlobReference(fileId.ToString());
            var exists = await blob.ExistsAsync();
            if (exists)
            {
                await blob.FetchAttributesAsync();
                return blob.Properties.Length;
            }
            else
            {
                throw new Exception("File not found.");
            }
        }

        public async Task DeleteFileAsync(Guid fileId)
        {
            var blob = _cloudBlobContainer.GetBlockBlobReference(fileId.ToString());
            await blob.DeleteIfExistsAsync();
        }

        public async Task<byte[]> ReadAllBytes(Guid fileId)
        {
            var blob = _cloudBlobContainer.GetBlockBlobReference(fileId.ToString());
            var exists = await blob.ExistsAsync();
            if (exists)
            {
                MemoryStream data = new MemoryStream();
                await blob.DownloadToStreamAsync(data);

                return data.ToArray();
            }
            else
            {
                throw new Exception("File not found.");
            }
        }

        public async Task ReplaceFile(Guid fileGuid, Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var blob = _cloudBlobContainer.GetBlockBlobReference(fileGuid.ToString());
            var exists = await blob.ExistsAsync();
            if (exists)
            {
                using (stream)
                {
                    await blob.UploadFromStreamAsync(stream);
                }
            }
            else
            {
                throw new Exception("File not found.");
            }
        }

    }
}
