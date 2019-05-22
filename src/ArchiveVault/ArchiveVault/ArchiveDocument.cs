using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ArchiveVault.Services;

namespace ArchiveVault
{
    public static class ArchiveDocument
    {
        // instantiated here following Functions guidance for multiple executions
        private static BlobStorageService blobStorageService = new BlobStorageService();
        private static SharePointService sharePointService = new SharePointService();

        [FunctionName("ArchiveDocument")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("ArchiveDocument called via HTTP");

            // Get file path from message
            string spFilePath = req.Query["SpFilePath"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            spFilePath = spFilePath ?? data?.spFilePath;

            // Get file from SP
            var spFile = await sharePointService.GetFile(spFilePath, "", "", ""); // TODO

            // Save file to blob storage
            var createdFileGuid = await blobStorageService.AddFileAsync("", "", null); // TODO

            return new OkObjectResult($"File created: {createdFileGuid}");
        }
    }
}
