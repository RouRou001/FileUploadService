using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileUploadService.Utilities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadBufferController : ControllerBase
    {
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> OnPostUploadAsyncBuffer(List<IFormFile> files)
        {
            string storeDestination = "AzureBlobs";
            string container = "all-files";

            DateTime now = DateTime.Now;
            string nowAsString = now.ToString("yyyy-MM-dd hh-mm-ss");

            long size = files.Sum(f => f.Length);

            foreach (IFormFile formFile in files)
            {
                if (formFile.Length > 0)
                {
                    if(FileValidator.validateFile(formFile) == false)
                    {
                        return BadRequest(new { message = "Invalid File" });
                    }


                    string fileName = nowAsString + " " + formFile.FileName;
                    if (storeDestination == "local")
                    {
                        var filePath = Path.Combine("StoredFiles", fileName);
                        filePath = System.IO.Directory.GetCurrentDirectory() + "\\" + filePath;

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                    else if (storeDestination == "AzureBlobs")
                    {
                        using (Stream stream = formFile.OpenReadStream())
                        {
                            BlobsHelpers blobsHelper = new BlobsHelpers();
                            await blobsHelper.uploadFile(container, fileName, stream);
                        }
                    }
                }
            }

            return Ok(new { count = files.Count, size });
        }

        [HttpGet]
        public string Get()
        {
            return "ABC";
        }
    }
}