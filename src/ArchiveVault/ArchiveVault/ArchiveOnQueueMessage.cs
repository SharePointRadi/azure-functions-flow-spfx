using System;
using System.Threading.Tasks;
using ArchiveVault.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ArchiveVault
{
    public static class ArchiveOnQueueMessage
    {
        // instantiated here following Functions guidance for multiple executions
        private static BlobStorageService blobStorageService = new BlobStorageService();
        private static SharePointService sharePointService = new SharePointService();

        [FunctionName("ArchiveOnQueueMessage")]
        public async static void Run([QueueTrigger("archive-queue", Connection = "")]string myQueueItem, ILogger log)
        {
            log.LogInformation("ArchiveOnQueueMessage called.");

            // Get file path from message
            string spFilePath = myQueueItem;

            // Get file from SP
            var spFile = await sharePointService.GetFile(spFilePath, "", "", ""); // TODO

            // Save file to blob storage
            var createdFileGuid = await blobStorageService.AddFileAsync("", "", null); // TODO
        }
    }
}
