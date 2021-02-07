using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using YandexDiskUploader.Abstractions.Handlers;
using YandexDiskUploader.Abstractions.Handlers.Factories;
using YandexDiskUploader.Abstractions.POCO;

namespace YandexDiskUploader.Abstractions.Requests
{
    public class UploadResourcePathRequest : IRequest
    {
        public IEnumerable<FileInfoPOCO> FileInfos { private get; set; }

        public string FolderPath { private get; set; }

        public bool FileOverwriting { private get; set; }

        public UploadResourcePathRequest() { }

        public UploadResourcePathRequest(IEnumerable<FileInfoPOCO> fileInfos, string folderPath, bool needFilesOverwriting)
        {
            this.FileInfos = fileInfos;

            this.FolderPath = folderPath;

            this.FileOverwriting = needFilesOverwriting;
        }

        public async Task<RequestStatus> DoRequestAsync(HttpClient httpClient, IHandler handler)
        {
            Console.WriteLine("Начинаю загружать файлы на Яндекс.Диск...");

            List<Task<RequestStatus>> filesTasks = new List<Task<RequestStatus>>();

            foreach (FileInfoPOCO fileInfo in this.FileInfos)
            {
                ConsoleExtensions.UpdateUploadStatus(fileInfo, "Загружается");

                filesTasks.Add(Task.Run(async () =>
                {
                    HttpResponseMessage hrm = await httpClient.GetAsync("v1/disk/resources/upload?path=" + HttpUtility.UrlEncode(this.FolderPath + fileInfo.FileInfo.Name) + "&overwrite=" + this.FileOverwriting.ToString().ToLower()).ConfigureAwait(false);

                    return await handler.HandleAsync(hrm, OperationType.UploadPath, fileInfo);
                }));
            }

            bool containsFailed = (await Task.WhenAll(filesTasks).ConfigureAwait(false)).Contains(RequestStatus.Failed);

            if (containsFailed)
            {
                //дописать путь до лога
                Console.WriteLine(String.Format("Некоторые файлы не были загружены, см. лог-файл {0}", Logger.FILENAME));

                return RequestStatus.Failed;
            }

            return RequestStatus.OK;
        }
    }
}
