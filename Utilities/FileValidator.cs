
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace FileUploadService.Utilities
{
    class FileValidator
    {
        private static string[] permittedExtensions = { ".txt", ".pdf", ".mp3", ".zip", ".7z", ".rar", ".csv", ".xml", ".gif", ".jpeg", ".jpg", ".png",
        ".key", ".odp", ".pps", ".ppt", ".pptx", ".ods", ".xls", ".xlsm", ".xlsx", ".avi", ".h264", ".mkv", ".mov", ".mp4", ".mpg", ".mpeg", ".wmv",
        ".doc", ".docx", ".odt", ".pdf", ".txt", ".wpd"};

        //private const long fileSizeLimit = 1048576000; // 1GB
        private const long fileSizeLimit = 268435456;

        public static bool validateFile(IFormFile file)
        {
            if(validateFileExtension(file) && validateFileSize(file))
            {
                return (true);
            }
            return (false);
        }

        public static bool validateFileExtension(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return (false);
            }
            return (true);
        }

        public static bool validateFileSize(IFormFile file)
        {
            if (file.Length > fileSizeLimit)
            {
                return (false);
            }
            return (true);
        }
    }
}