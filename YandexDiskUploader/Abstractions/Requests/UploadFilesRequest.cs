using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskUploader.Abstractions.Requests
{
    public class UploadFilesRequest : IRequest
    {
        public Task DoRequestAsync(HttpClient httpClient, string[] param)
        {
            Console.WriteLine("Начинаю загружать файлы на Яндекс.Диск...");

            List<Task> filesTasks = new List<Task>();

            foreach (FileInfo fileInfo in param)
            {
                byte[] byteArr = File.ReadAllBytes(fileInfo.FullName);
            }
        }

        public Task DoRequestAsync<T>(HttpClient httpClient, T param)
        {
            throw new NotImplementedException();
        }
    }
}
