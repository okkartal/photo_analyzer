using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Photos;
using Photos.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Photos;
public static class PhotosSearch
{
    [FunctionName("PhotosSearch")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        [CosmosDB("photos", "metadata", Connection = Literals.CosmosDBConnectionString)] DocumentClient documentClient,
        ILogger log)
    {
        log?.LogInformation("Searching...");

        var searchTerm = req.Query["searchTerm"];

        if (string.IsNullOrEmpty(searchTerm))
        {
            return new NotFoundResult();
        }

        var collectionUri = UriFactory.CreateDocumentCollectionUri("photo", "metadata");

        var query = documentClient.CreateDocumentQuery<PhotoUploadModel>(collectionUri, new FeedOptions
        {
            EnableCrossPartitionQuery = true
        }).Where(p => p.Description.Contains(searchTerm))
        .AsDocumentQuery();

        var results = new List<dynamic>();

        while (query.HasMoreResults)
        {
            foreach (var result in await query.ExecuteNextAsync())
            {
                results.Add(result);
            }
        }

        return new OkObjectResult(results);
    }
}

