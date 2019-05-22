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
            var pnpAuthenticationManager = new AuthenticationManager();

            using (ClientContext clientContext = pnpAuthenticationManager
                    .GetSharePointOnlineAuthenticatedContextTenant(sharePointSiteCollectionUrl, Username, Password))
            {
                var web = clientContext.Web;
                var file = web.GetFileByUrl(fullPath);

                var fileStream = file.OpenBinaryStream();

                clientContext.Load(file);

                await clientContext.ExecuteQueryRetryAsync();

                // Required to read the length so the stream is read and completed
                var length = fileStream.Value.Length;

                return fileStream.Value;
            }
        }
    }
}
