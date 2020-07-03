using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : ControllerBase
    {

        [DisableCors]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        {
            System.Console.WriteLine("DEBUG XXX : OnPostUploadAsync called");
            System.Console.WriteLine("DEBUG XXX count : " + files.Count);
            long size = files.Sum(f => f.Length);

            System.Console.WriteLine("DEBUG XXX size : " + size);
            foreach (var formFile in files)
            {
                System.Console.WriteLine("DEBUG XXX : formFileLength " + formFile.Length);
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine("StoredFiles",
                        Path.GetRandomFileName());

                    System.Console.WriteLine("DEBUG XXX filePath : " + filePath);

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