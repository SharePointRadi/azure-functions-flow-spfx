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

        }

        public BlobStorageService(
            string storageAccountName,
            string storageAccountKey,
            string storageContainerName)
        {
            StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageAccountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            // Get a blob client object
            _cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Get the container / folder where we will store files
            _cloudBlobContainer = _cloudBlobClient.GetContainerReference(storageContainerName);

            // Create a new container, if it does not exist
            _cloudBlobContainer.CreateIfNotExistsAsync().Wait();
        }

        public async Task<Guid> AddFileAsync(string fileName, string type, Stream fileData)
        {
            return await this.AddFileAsync(Guid.NewGuid(), fileName, type, fileData);
        }

        public async Task<Guid> AddFileAsync(Guid fileGuid, string fileName, string type, Stream fileData)
        {
            var blob = _cloudBlobContainer.GetBlockBlobReference(fileGuid.ToString());

            string fileExt = null;
            if (fileName != null)
            {
                fileExt = Path.GetExtension(fileName);

                if (fileExt.StartsWith("."))
                {
                    fileExt = fileExt.Remove(0, 1); // remove the point (.)
                }
            }

            var exists = await blob.ExistsAsync();

            if (exists == false)
            {
                if (!string.IsNullOrWhiteSpace(fileExt))
                {
                    blob.Metadata.Add("fileExt", fileExt);
                }

                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    blob.Metadata.Add("fileName", fileName);
                }

                if (!string.IsNullOrWhiteSpace(type))
                {
                    blob.Metadata.Add("type", type);
                }

                //blob.Metadata.Add("sessionGuid", _sessionGuid.ToString());

                await blob.UploadFromStreamAsync(fileData);
            }
            else
            {
                throw new ApplicationException($"File with the specified GUID {fileGuid} already exists in blob storage.");
            }

            return fileGuid;
        }

        public async Task<Guid> AddFileAsync(Guid fileGuid, string fileName, string type, byte[] fileData)
        {
            using (var memStr = new MemoryStream(fileData))
            {
                return await this.AddFileAsync(fileGuid, fileName, type, memStr);
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
