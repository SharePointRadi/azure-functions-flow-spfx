namespace ArchiveVaultFunctions
{
    using ArchiveVaultFunctions.Services;
    using Microsoft.Azure.WebJobs.Host;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class Archive
    {
        /// <summary>
        /// Retrieves a document from SharePoint Online, then sends it to blob storage
        /// </summary>
        /// <param name="log">A TraceWriter instance for logging</param>
        /// <param name="blobStorageService">A BlobStorageService object to handle blob storage</param>
        /// <param name="sharePointService">A SharePointService instance to handle document extraction.</param>
        /// <param name="spFilePath">The full path to the SharePoint document</param>
        /// <param name="siteCollection"></param>
        /// <param name="confidentialityLevel"></param>
        /// <param name="retentionPeriod"></param>
        /// <returns></returns>
        public async static Task<Guid> ArchiveDocument(
            TraceWriter log, 
            BlobStorageService blobStorageService,
            SharePointService sharePointService,
            string spFilePath,
            string confidentialityLevel,
            string retentionPeriod)
        {
            log.Info($"File url: {spFilePath} confidentiality: {confidentialityLevel} retention: {retentionPeriod}");

            var fileName = Path.GetFileName(spFilePath);

            // Get file from SP
            var spFile = await sharePointService.GetFile(spFilePath);

            // Save file to blob storage
            var createdFileGuid = await blobStorageService.AddFileAsync(fileName, spFilePath, spFile, confidentialityLevel, retentionPeriod);

            spFile.Dispose();

            return createdFileGuid;
        }
    }
}
