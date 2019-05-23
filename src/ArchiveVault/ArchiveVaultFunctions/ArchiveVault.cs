using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ArchiveVaultFunctions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace ArchiveVaultFunctions
{
    public static class ArchiveVault
    {
        private static BlobStorageService blobStorageService = new BlobStorageService();
        private static SharePointService sharePointService = new SharePointService();


        // Sample payload:
        //{
        //    "spFilePath":"https://techmikael.sharepoint.com/teams/CollabSummit2019/Shared%20Documents/Document.docx",
        //    "siteCollection":"https://techmikael.sharepoint.com/teams/CollabSummit2019",
        //    "confidentialityLevel":"Secure",
        //    "retentionPeriod":"3"
        //}

    [FunctionName("ArchiveVault")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("ArchiveVault HTTP trigger function incoming request.");

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();
            var spFilePath = data?.spFilePath?.Value;
            var siteCollection = data?.siteCollection?.Value;
            var confidentialityLevel = data?.confidentialityLevel?.Value;
            var retentionPeriod = data?.retentionPeriod?.Value;

            try
            {
                log.Info($"File url: {spFilePath} confidentiality: {confidentialityLevel} retention: {retentionPeriod}");

                var fileName = Path.GetFileName(spFilePath);

                // Get file from SP
                var spFile = await sharePointService.GetFile(spFilePath, siteCollection);

                // Save file to blob storage
                var createdFileGuid = await blobStorageService.AddFileAsync(fileName, spFilePath, spFile, confidentialityLevel, retentionPeriod);

                spFile.Dispose();

                return req.CreateResponse($"File created: {createdFileGuid}");
            }
            catch (Exception ex)
            {
                log.Error("Error:", ex);
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
