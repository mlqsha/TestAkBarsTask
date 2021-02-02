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
        private readonly IEnumerable<string> _folderPath;

        public GetFolderRequest(IEnumerable<string> folderPath)
        {
            _folderPath = folderPath;
        }

        //проверка на существование папок
        public async Task<RequestStatus> DoRequestAsync(HttpClient httpClient, HandlersChainFactory chainFactory)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (string pathElem in _folderPath)
            {
                if (sb.Length != 0)
                {
                    sb.Append("/");
                }

                sb.Append(pathElem);

                HttpResponseMessage hrm = await httpClient.GetAsync("v1/disk/resources?path=" + HttpUtility.UrlEncode(sb.ToString())).ConfigureAwait(false);

                IHandler chainOfHandlers = chainFactory.GetHandlersChain();

                ErrorPOCO error = await chainOfHandlers.HandleAsync(hrm);

                if (error != null)
                {
                    return RequestStatus.Failed;
                }
            }

            return RequestStatus.OK;
        }
    }
}
