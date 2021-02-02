using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using YandexDiskUploader.Abstractions.Handlers;
using YandexDiskUploader.Abstractions.Handlers.Factories;
using YandexDiskUploader.Abstractions.POCO;

namespace YandexDiskUploader.Abstractions.Requests
{
    public class UploadFilesRequest : IRequest
    {
        private readonly IEnumerable<FileInfo> _fileInfos;

        private readonly string _folderPath;

        public UploadFilesRequest(IEnumerable<FileInfo> fileInfos, string folderPath)
        {
            this._fileInfos = fileInfos;

            this._folderPath = folderPath;
        }

        public async Task<RequestStatus> DoRequestAsync(HttpClient httpClient, HandlersChainFactory chainFactory)
        {
            Console.WriteLine("Начинаю загружать файлы на Яндекс.Диск...");

            List<Task<RequestStatus>> filesTasks = new List<Task<RequestStatus>>();

            foreach (FileInfo fileInfo in this._fileInfos)
            {
                filesTasks.Add(Task.Run(async () =>
                {
                    HttpResponseMessage hrm = await httpClient.GetAsync("v1/disk/resources/upload?path=" + HttpUtility.UrlEncode(this._folderPath + fileInfo.Name)).ConfigureAwait(false);

                    IHandler handlersChain = chainFactory.GetHandlersChain();

                    ErrorPOCO error = await handlersChain.HandleAsync(hrm);

                    if (error != null)
                    {
                        return RequestStatus.Failed;
                    }

                    Task<byte[]> byteArrTask = File.ReadAllBytesAsync(fileInfo.FullName);

                    return RequestStatus.OK;
                }));
            }

            bool containsFailed = (await Task.WhenAll(filesTasks).ConfigureAwait(false)).Contains(RequestStatus.Failed);

            if (containsFailed)
            {
                return RequestStatus.Failed;
            }

            return RequestStatus.OK;
        }
    }
}
