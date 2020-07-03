using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        {
            System.Console.WriteLine("DEBUG XXX : OnPostUploadAsync called");
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine("StoredFiles",
                        Path.GetRandomFileName());

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            System.Console.WriteLine("DEBUG XXX : OnPostUploadAsync return");
            return Ok(new { count = files.Count, size });
        }

        // [HttpPost]
        // public async Task<string> Post(FileUploadAPI objFile)
        // {
        //     try
        //     {
        //         if (objFile.files.Length > 0)
        //         {
        //             if (!Directory.Exists(_environment.WebRootPath + "\\Upload\\"))
        //             {
        //                 Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
        //             }
        //             using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Upload\\" + objFile.files.FileName))
        //             {
        //                 objFile.files.CopyTo(fileStream);
        //                 fileStream.Flush();
        //                 return "\\Upload\\" + objFile.files.FileName;
        //             }
        //         }
        //         else
        //         {
        //             return "Failed";
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return ex.Message.ToString();
        //     }
        // }

        [HttpGet]
        public string Get()
        {
            return "myValue AA";
        }
    }
}