using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using YandexDiskUploader.Abstractions.Handlers;

namespace YandexDiskUploader.Abstractions.Requests
{
    public class GetFolderRequest : IRequest
    {
        private IEnumerable<string> _folderPath;

        public GetFolderRequest(IEnumerable<string> folderPath)
        {
            _folderPath = folderPath;
        }

        public async Task<RequestStatus> DoRequestAsync(HttpClient httpClient)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            RequestStatus requestStatus = RequestStatus.OK;

            foreach (string pathElem in _folderPath)
            {
                if (sb.Length != 0)
                {
                    sb.Append("/");
                }

                sb.Append(pathElem);

                HttpResponseMessage hrm = await httpClient.GetAsync("v1/disk/resources?path=" + HttpUtility.UrlEncode(sb.ToString())).ConfigureAwait(false);

                AbstractHandler okHandler = new OKHandler();

                okHandler.SetNext(new ErrorHandler());

                requestStatus = await okHandler.HandleAsync(hrm);

                if (requestStatus != RequestStatus.OK)
                {
                    break;
                }
            }

            return requestStatus;
        }
    }
}
