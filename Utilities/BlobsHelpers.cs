using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileUploadService.Utilities
{
    class BlobsHelpers
    {
        const string AZURE_STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=fileaccessapi01;AccountKey=zNadRpKR9FVxBqPNAxhdTyt8AxPV2U1418PeYv9XYMNQL3JKtXsp0AzxNYV3FSI1WEXfBaJ5myf5qHdf/970sg==;EndpointSuffix=core.windows.net";
        const string LOCAL_FILE_DIRECTORY = "../StoredFiles/";

        public Pageable<BlobItem> getBlobItems(string container)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(AZURE_STORAGE_CONNECTION_STRING);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

            Pageable<BlobItem> blobItems = containerClient.GetBlobs();
            return (blobItems);
        }

        public async Task<Response<BlobContentInfo>> uploadFile(string container, string fileName, Stream file)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(AZURE_STORAGE_CONNECTION_STRING);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            return (await blobClient.UploadAsync(file, true));
        }

        public async void downloadFile(string container, string fileName)
        {
            DateTime now = DateTime.Now;
            string nowAsString = now.ToString("yyyy-MM-dd hh-mm-ss");

            string localFilePath = Path.Combine(LOCAL_FILE_DIRECTORY, fileName);
            string downloadFilePath = "Download-" + nowAsString + "-" + localFilePath;

            BlobServiceClient blobServiceClient = new BlobServiceClient(AZURE_STORAGE_CONNECTION_STRING);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
            {
                await download.Content.CopyToAsync(downloadFileStream);
                downloadFileStream.Close();
            }
        }

        public void deleteFile(string container, string fileName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(AZURE_STORAGE_CONNECTION_STRING);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            blobClient.Delete();
        }
    }
}