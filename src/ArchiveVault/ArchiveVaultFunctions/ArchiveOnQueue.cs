using System;
using ArchiveVaultFunctions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace ArchiveVaultFunctions
{
    public static class ArchiveOnQueue
    {
        private static BlobStorageService blobStorageService = new BlobStorageService();
        private static SharePointService sharePointService = new SharePointService();

        // Sample payload:
        //{
        //    "spFilePath":"https://techmikael.sharepoint.com/teams/CollabSummit2019/Shared%20Documents/Document.docx",
        //    "confidentialityLevel":"Secure",
        //    "retentionPeriod":"3"
        //}

        [FunctionName("ArchiveOnQueue")]
        public static void Run([QueueTrigger("archivevault-queue", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"ArchiveOnQueue trigger function processed: {myQueueItem}");

            try
            {
                dynamic data = JsonConvert.DeserializeObject(myQueueItem);
                var spFilePath = data?.spFilePath?.Value;
                var confidentialityLevel = data?.confidentialityLevel?.Value;
                var retentionPeriod = data?.retentionPeriod?.Value;

                var createdFileGuid = Archive.ArchiveDocument(
                    log,
                    blobStorageService,
                    sharePointService,
                    spFilePath,
                    confidentialityLevel,
                    retentionPeriod);

                log.Info($"File created: {createdFileGuid}");
            }
            catch (Exception ex)
            {
                log.Error("Error:", ex);
            }
        }
    }
}
