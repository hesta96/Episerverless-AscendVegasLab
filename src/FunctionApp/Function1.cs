using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Shared.Models;

namespace FunctionApp
{
    [StorageAccount("my-storage-connection")]
    public static class Function1
    {
        [FunctionName("Function1")]
        [return: Queue("to-ascii-conversion")]
        public static CloudQueueMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] ProcessingRequest request,
            [Blob("%input-container%/{FileId}")]                CloudBlockBlob outBlob,
            TraceWriter                                         log)
        {
            log.Info("(Fun1) Received image for processing...");

            AsyncHelper.RunSync(async () =>
            {
                await outBlob.UploadFromByteArrayAsync(request.Content, 0, request.Content.Length);
            });

            // Description and tags are made up since we are not using image processing to figure this out in this lab
            var asciiArtRequest = new AsciiArtRequest
            {
                BlobRef = outBlob.Name,
                Width = request.Width,
                Description = "Made up description",
                Tags = new string[] { "tag1 tag 2" }
            };
            
            return asciiArtRequest.AsQueueItem();
        }
    }
}
