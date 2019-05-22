using System;
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

        [FunctionName("ArchiveVault")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse SpFilePath query parameter
            string spFilePath = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "SpFilePath", true) == 0)
                .Value;

            string siteCollection = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "siteCollection", true) == 0)
                .Value;

            if (spFilePath == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                spFilePath = data?.spFilePath;
            }

            if (siteCollection == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>(); // TODO: fix duplicate code
                siteCollection = data?.siteCollection;
            }

            try
            {
                log.Info("ArchiveDocument called via HTTP");

                // DEBUG
                spFilePath = "https://techmikael.sharepoint.com/teams/CollabSummit2019/Shared%20Documents/Document.docx";
                siteCollection = "https://techmikael.sharepoint.com/teams/CollabSummit2019";
                
                // Get file from SP
                var spFile = await sharePointService.GetFile(spFilePath, siteCollection); // TODO

                // Save file to blob storage
                var createdFileGuid = await blobStorageService.AddFileAsync("", "", null); // TODO

                return req.CreateResponse($"File created: {createdFileGuid}");
            }
            catch (Exception ex)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
