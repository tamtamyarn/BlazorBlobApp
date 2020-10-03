using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using System.Linq;

namespace BlazorBlobApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobsController : ControllerBase
    {
        private readonly string connectionString;

        public BlobsController()
        {
            this.connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
        }

        [HttpGet]
        public IActionResult Get()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainers = blobServiceClient.GetBlobContainers();
            var containerNames = blobContainers.Select(c => c.Name);
            return Ok(containerNames);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContainerAsync(string containerName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            await blobServiceClient.CreateBlobContainerAsync(containerName);
            return Ok(containerName);
        }
    }
}
