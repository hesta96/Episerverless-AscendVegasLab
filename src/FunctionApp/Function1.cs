using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Shared.Models;

namespace FunctionApp
{
    [StorageAccount("my-storage-connection")]
    public static class Function1
    {
        [FunctionName("Function1")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] ProcessingRequest request,
            [Blob("%input-container%/{FileId}")]                CloudBlockBlob outBlob,
            [Queue("to-ascii")]                                 out CloudQueueMessage message,
            TraceWriter                                         log)
        {
            log.Info("(Fun1) Received image for processing...");

            AsyncHelper.RunSync(async () =>
            {
                await outBlob.UploadFromByteArrayAsync(request.Content, 0, request.Content.Length);
            });

            var analysisRequest = new AnalysisReq
            {
                BlobRef = outBlob.Name,
                Width = request.Width,
                ImageUrl = request.ImageUrl
            };

            message = analysisRequest.AsQueueItem();

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(outBlob.Name)
            };
        }
    }
}
