using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Azure.Storage.Blobs.Specialized;
using BlazorBlobApp.Shared;
using System.Net.Mime;

namespace BlazorBlobApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainersController : ControllerBase
    {
        private readonly string containerName;
        private readonly BlobContainerClient blobContainerClient;

        public ContainersController(BlobServiceClient blobServiceClient)
        {
            this.containerName = "testcontainer";
            blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = blobContainerClient.GetBlobs().Select(b => new BlobObject() { Name = b.Name, Size = b.Properties.ContentLength});
            return Ok(result);
        }

        /*
        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            blobContainerClient.UploadBlob(file.FileName,file.OpenReadStream());
            return NoContent();
        }
        

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        public async Task<ActionResult> PostAsync([FromForm]FileRequestObject fileRequestObject)
        {
            //var blobClient = blobContainerClient.GetBlobClient(fileRequestObject.File.Name);
            var blockBlobClient = blobContainerClient.GetBlockBlobClient(fileRequestObject.File.FileName);
            HashSet<string> blocklist = new HashSet<string>();
            var file = fileRequestObject.File;
            const int pageSizeInBytes = 10485760;
            long preLastByte = 0;
            long bytesRemain = file.Length;

            byte[] bytes;

            using (MemoryStream ms = new MemoryStream())
            {
                var fileStream = file.OpenReadStream();
                await fileStream.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            do
            {
                long bytesToCopy = Math.Min(bytesRemain, pageSizeInBytes);
                byte[] bytesToSend = new byte[bytesToCopy];
                Array.Copy(bytes, preLastByte, bytesToSend, 0, bytesToCopy);
                preLastByte += bytesToCopy;
                bytesRemain -= bytesToCopy;

                string blockId = Guid.NewGuid().ToString();
                string base64BlockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId));

                await blockBlobClient.StageBlockAsync(base64BlockId, new MemoryStream(bytesToSend, true));
                blocklist.Add(base64BlockId);
                //await blockBlobClient.UploadAsync(base64BlockId, new MemoryStream(bytesToSend, true), null)
            } while (bytesRemain > 0);

            await blockBlobClient.CommitBlockListAsync(blocklist);

            return Ok();
            }
        

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        public async Task<ActionResult> PostAsync2([FromForm] FileRequestObject fileRequestObject)
        {
            var file = fileRequestObject.File;
            var blockBlobClient = blobContainerClient.GetBlockBlobClient(fileRequestObject.File.FileName);
            using (var fileStream = file.OpenReadStream())
            {
                await blockBlobClient.UploadAsync(fileStream);
            }
            return Ok();
        }
        
        [HttpPost]
        public async Task<ActionResult> PostAsync3()
        {
            var blockBlobClient = blobContainerClient.GetBlockBlobClient(Guid.NewGuid().ToString());
            await blockBlobClient.UploadAsync(Request.Body);
            return Ok();
        }
        */

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        public async Task<ActionResult> PostAsync4()
        {
            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            var blockBlobClient = blobContainerClient.GetBlockBlobClient(file.FileName);
            using (var fileStream = file.OpenReadStream())
            {
                await blockBlobClient.UploadAsync(fileStream);
            }
            return Ok();
        }

        /*
        [HttpGet("{name}")]
        public IActionResult Download(string name)
        {
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);
            BlobDownloadInfo download = blobClient.Download();
            return Ok(download.Content);
        }
        
        [HttpGet("{name}")]
        public async Task<IActionResult> Download2(string name)
        {
            var blockBlobClient = blobContainerClient.GetBlockBlobClient(name);
            using (var fileStream = System.IO.File.OpenWrite(@$"C:\Users\y.nakayama\Downloads\{name}"))
            //using (var fileStream = System.IO.File.OpenWrite(@$".\{name}"))
            {
                await blockBlobClient.DownloadToAsync(fileStream);
            }
            return Ok();
        }
        

        [HttpGet("{name}")]
        public async Task<IActionResult> Download3(string name)
        {
            var blockBlobClient = blobContainerClient.GetBlockBlobClient(name);
            BlobDownloadInfo download = await blockBlobClient.DownloadAsync();

            return File(download.Content, download.ContentType, blockBlobClient.Name);
        }
        */

        [HttpGet("{name}")]
        public async Task<IActionResult> Download4(string name)
        {
            var blockBlobClient = blobContainerClient.GetBlockBlobClient(name);
            BlobDownloadInfo download = await blockBlobClient.DownloadAsync();

            ContentDisposition contentDisposition = new ContentDisposition
            {
                FileName = blockBlobClient.Name, Inline = false
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(download.Content, download.ContentType);
        }

        [HttpDelete("{name}")]
        public IActionResult Remove(string name)
        {
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);
            blobClient.Delete();
            return NoContent();
        }
    }

    public class FileRequestObject
    {
        public IFormFile File { get; set; }
    }
}
