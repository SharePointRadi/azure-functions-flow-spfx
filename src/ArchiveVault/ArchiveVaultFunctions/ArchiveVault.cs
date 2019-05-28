namespace ArchiveVaultFunctions
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using ArchiveVaultFunctions.Services;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Newtonsoft.Json;

    public static class ArchiveVault
    {
        private static BlobStorageService blobStorageService = new BlobStorageService();
        private static SharePointService sharePointService = new SharePointService();

        // Sample payload:
        //{
        //    "spFilePath":"https://{tenant}.sharepoint.com/teams/{siteCollection}/Shared%20Documents/Document.docx",
        //    "confidentialityLevel":"Secure",
        //    "retentionPeriod":"3"
        //}

        [FunctionName("ArchiveVault")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("ArchiveVault HTTP trigger function incoming request.");

            // Get current claims
            //foreach (Claim claim in ClaimsPrincipal.Current.Claims)
            //{
            //}

            try
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                var spFilePath = data?.spFilePath?.Value;
                var confidentialityLevel = data?.confidentialityLevel?.Value;
                var retentionPeriod = data?.retentionPeriod?.Value;

                var createdFileGuid = await Archive.ArchiveDocument(
                    log,
                    blobStorageService,
                    sharePointService,
                    spFilePath,
                    confidentialityLevel,
                    retentionPeriod);

                return req.CreateResponse($"File created: {createdFileGuid}");
            }
            catch (Exception ex)
            {
                log.Error("Error:", ex);
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [FunctionName("GetArchiveVaultDocuments")]
        public static async Task<HttpResponseMessage> GetArchivedDocuments(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("ArchiveVault HTTP trigger function incoming request.");

            try
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                var spFilePath = data?.spFilePath?.Value;
                var confidentialityLevel = data?.confidentialityLevel?.Value;
                var retentionPeriod = data?.retentionPeriod?.Value;

                var docs = await blobStorageService.GetDocuments();

                var jsonToReturn = JsonConvert.SerializeObject(docs);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                log.Error("Error:", ex);
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
