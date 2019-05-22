using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveVault.Services
{
    public class SharePointService
    {
        public SharePointService()
        {

        }

        public async Task<Stream> GetFile(
            string fullPath,
            string sharePointSiteCollectionUrl,
            string clientId,
            string clientSecret)
        {
            var pnpAuthenticationManager = new AuthenticationManager();
            using (ClientContext clientContext = pnpAuthenticationManager
                    .GetAppOnlyAuthenticatedContext(sharePointSiteCollectionUrl, clientId, clientSecret))
            {
                var web = clientContext.Web;
                var file = web.GetFileByUrl(fullPath);

                var fileStream = file.OpenBinaryStream();

                clientContext.Load(file);

                await clientContext.ExecuteQueryRetryAsync();

                // This next line is actually required, otherwise the stream is null
                //var length = fileStream.Value.Length;
                //Console.WriteLine("Retrieved file length: " + length);

                return fileStream.Value;
            }
        }
    }
}
