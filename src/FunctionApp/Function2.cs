using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage.Queue;
using Shared.Models;

namespace FunctionApp
{
    [StorageAccount("my-storage-connection")]
    public static class Function2
    {
        [FunctionName("Function2")]
        [return: Queue("to-ascii-conversion")]
        public static CloudQueueMessage Run(
            [QueueTrigger("to-ascii")]                              AnalysisReq request,
            [Blob("%input-container%/{BlobRef}", FileAccess.Read)]  Stream inBlob,
                                                                    TraceWriter log)
        {
            log.Info("(Fun2) Running image analysis...");

            var asciiArtRequest = new AsciiArtRequest
            {
                BlobRef = request.BlobRef,
                Width = request.Width,
                Description = "Made up description",
                Tags = new string[] { "tag1 tag 2" }
            };

            log.Info("(Fun2) Finished image analysis.");

            return asciiArtRequest.AsQueueItem();
        }
    }
}
