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
        public string RequestedPath { private get; set; }

        public CreateFolderRequest() { }

        public CreateFolderRequest(string pathToBeRequested)
        {
            this.RequestedPath = pathToBeRequested;
        }

        public async Task<RequestStatus> DoRequestAsync(HttpClient httpClient, IHandler handler)
        {
            HttpResponseMessage hrm = await httpClient.PutAsync("v1/disk/resources" + this.RequestedPath, null).ConfigureAwait(false);

            return await handler.HandleAsync(hrm, OperationType.CreateFolder);
        }
    }
}
