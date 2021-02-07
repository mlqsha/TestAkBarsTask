using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using YandexDiskUploader.Abstractions.Handlers;
using YandexDiskUploader.Abstractions.Handlers.Factories;
using YandexDiskUploader.Abstractions.POCO;

namespace YandexDiskUploader.Abstractions.Requests
{
    public class GetFolderRequest : IRequest
    {
        public IEnumerable<string> FolderPath { private get; set; }

        public GetFolderRequest() { }

        public GetFolderRequest(IEnumerable<string> folderPath)
        {
            FolderPath = folderPath;
        }

        //проверка на существование папок
        public async Task<RequestStatus> DoRequestAsync(HttpClient httpClient, IHandler handler)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            RequestStatus requestStatus = RequestStatus.OK;

            foreach (string pathElem in FolderPath)
            {
                if (sb.Length != 0)
                {
                    sb.Append("/");
                }

                sb.Append(pathElem);

                HttpResponseMessage hrm = await httpClient.GetAsync("v1/disk/resources?path=" + HttpUtility.UrlEncode(sb.ToString())).ConfigureAwait(false);

                requestStatus = await handler.HandleAsync(hrm, OperationType.GetFolder);

                if (requestStatus == RequestStatus.Failed)
                {
                    break;
                }
            }

            return requestStatus;
        }
    }
}
