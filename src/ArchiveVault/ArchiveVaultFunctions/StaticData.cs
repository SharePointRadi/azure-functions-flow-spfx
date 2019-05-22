using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace ArchiveVaultFunctions
{
    public static class StaticData
    {
        [FunctionName("StaticData")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                var list = new List<object> {
                new {
                    Id = 1,
                    OrderDate = new DateTime(2016, 1, 6),
                    Region = "east",
                    Rep = "Jones",
                    Item = "Pencil",
                    Units = 95M,
                    UnitCost = 1.99M,
                    Total = 189.05M
                },
                new {
                    Id = 2,
                    OrderDate = new DateTime(2016, 1, 23),
                    Region = "central",
                    Rep = "Kivell",
                    Item = "Binder",
                    Units = 50M,
                    UnitCost = 19.99M,
                    Total = 999.50M
                },
                new {
                    Id = 3,
                    OrderDate = new DateTime(2016, 2, 9),
                    Region = "central",
                    Rep = "Jardine",
                    Item = "Pencil",
                    Units = 36M,
                    UnitCost = 4.99M,
                    Total = 179.64M
                },
                new {
                    Id = 4,
                    OrderDate = new DateTime(2016, 2, 26),
                    Region = "central",
                    Rep = "Gill",
                    Item = "Pen",
                    Units = 27M,
                    UnitCost = 19.99M,
                    Total = 539.73M
                },
                new {
                    Id = 5,
                    OrderDate = new DateTime(2016, 3, 15),
                    Region = "west",
                    Rep = "Sorvino",
                    Item = "Pencil",
                    Units = 56M,
                    UnitCost = 2.99M,
                    Total = 167.44M
                }
            };

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {

                return req.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }
    }
}
