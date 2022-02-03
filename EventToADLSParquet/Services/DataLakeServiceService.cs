using Azure.Storage;
using Azure.Storage.Files.DataLake;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EventToADLSParquet.Services
{
    internal class DataLakeServiceService
    {
        public static void GetDataLakeServiceClient(ref Azure.Storage.Files.DataLake.DataLakeServiceClient dataLakeServiceClient, string accountName, string accountKey)
        {
            StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);

            string dfsUri = "https://" + accountName + ".dfs.core.windows.net";
            dataLakeServiceClient = new DataLakeServiceClient(new Uri(dfsUri), sharedKeyCredential);
        }

        public async Task<DataLakeFileSystemClient> CreateFileSystem(DataLakeServiceClient serviceClient, string folderName)
        {
            return await serviceClient.CreateFileSystemAsync(folderName);
        }

        public async Task<DataLakeDirectoryClient> CreateDirectory(DataLakeServiceClient serviceClient, string fileSystemName, string folderName, DateTime processDateTime)
        {
            DataLakeFileSystemClient fileSystemClient = serviceClient.GetFileSystemClient(fileSystemName);
            DataLakeDirectoryClient directoryClient = await fileSystemClient.CreateDirectoryAsync(folderName);

            var eventDay = processDateTime.Date.ToString();
            return await directoryClient.CreateSubDirectoryAsync(eventDay);
        }

        public async Task UploadFile(DataLakeFileSystemClient fileSystemClient, string eventData, string eventSubject, string fileName)
        {
            DataLakeDirectoryClient directoryClient = fileSystemClient.GetDirectoryClient(eventSubject);
            DataLakeFileClient fileClient = await directoryClient.CreateFileAsync(fileName);
            Stream fileStream = GenerateStreamFromString(eventData);

            long fileSize = fileStream.Length;
            await fileClient.AppendAsync(fileStream, offset: 0);
            await fileClient.FlushAsync(position: fileSize);
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
