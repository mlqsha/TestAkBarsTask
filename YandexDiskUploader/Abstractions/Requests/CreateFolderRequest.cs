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
    public class CreateFolderRequest : IRequest
    {
        private string _requestedPath;

        public CreateFolderRequest(string pathToBeRequested)
        {
            this._requestedPath = pathToBeRequested;
        }

        public async Task<RequestStatus> DoRequestAsync(HttpClient httpClient, HandlersChainFactory chainFactory)
        {
            HttpResponseMessage hrm = await httpClient.PutAsync("v1/disk/resources" + this._requestedPath, null).ConfigureAwait(false);

            IHandler chainOfHandlers = chainFactory.GetHandlersChain();

            ErrorPOCO error = await chainOfHandlers.HandleAsync(hrm);

            if (error != null)
            {
                return RequestStatus.Failed;
            }

            return RequestStatus.OK;
        }
    }
}
