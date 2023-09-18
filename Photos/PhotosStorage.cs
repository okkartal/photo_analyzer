using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Photos.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Photos;
public class PhotosStorage
{
    [FunctionName("PhotosStorage")]
    public async Task<byte[]> Run(
        [ActivityTrigger] PhotoUploadModel request,
        [Blob("photos", FileAccess.ReadWrite, Connection = Literals.StorageConnectionString)] CloudBlobContainer blobContainer,
        [CosmosDB("photos", "metadata", Connection = Literals.CosmosDBConnectionString, CreateIfNotExists = true)] IAsyncCollector<dynamic> items,
        ILogger log)
    {

        var newId = Guid.NewGuid();
        var blobName = $"{newId}.jpg";

        await blobContainer.CreateIfNotExistsAsync();

        var cloudBlockBlob = blobContainer.GetBlockBlobReference(blobName);

        var photoBytes = Convert.FromBase64String(request.Photo);

        await cloudBlockBlob.UploadFromByteArrayAsync(photoBytes, 0, photoBytes.Length);


        var item = new
        {
            id = newId,
            name = request.Name,
            description = request.Description,
            tags = request.Tags
        };

        await items.AddAsync(item);

        log?.LogInformation($"Successfully uploaded {newId}.jpg file and its metadata");

        return photoBytes;
    }
}

