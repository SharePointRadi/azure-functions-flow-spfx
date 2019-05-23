using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveVaultFunctions.Services
{
    public class SharePointService
    {
        private string Username;
        private string Password;

        public SharePointService()
        {
            Username = System.Environment.GetEnvironmentVariable("SharePointUsername", EnvironmentVariableTarget.Process);
            Password = System.Environment.GetEnvironmentVariable("SharePointPassword", EnvironmentVariableTarget.Process);

            if (string.IsNullOrWhiteSpace(Username)) throw new ArgumentException("You must set SharePointUsername in the application settings.");
            if (string.IsNullOrWhiteSpace(Password)) throw new ArgumentException("You must set SharePointPassword in the application settings.");
        }

        public async Task<Stream> GetFile(
            string fullPath,
            string sharePointSiteCollectionUrl)
        {
            // Process URL's - we get the server relative URL, works both for / and /teams/sitecoll URL's
            Uri fileUri = new Uri(fullPath);
            string serverserverUrl = fileUri.AbsoluteUri.Replace(fileUri.AbsolutePath, string.Empty);
            string serverRelativeUrl = fileUri.AbsolutePath;

            // Get a client context
            var pnpAuthenticationManager = new AuthenticationManager();
            using (ClientContext clientContext = pnpAuthenticationManager
                    .GetSharePointOnlineAuthenticatedContextTenant(serverserverUrl, Username, Password))
            {
                // we use OpenBinaryDirect as it can get a file from anywhere across Site Collections
                FileInformation fileInformation = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, serverRelativeUrl);

                await clientContext.ExecuteQueryRetryAsync();

                // return as memory stream
                var memoryStream = new MemoryStream();
                fileInformation.Stream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
