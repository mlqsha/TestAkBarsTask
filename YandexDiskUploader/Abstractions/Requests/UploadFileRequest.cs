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
    public class UploadFileRequest : IRequest
    {
        public FileInfoPOCO FileToBeSent { private get; set; }
        public string Href { private get; set; }

        public UploadFileRequest() { }

        public UploadFileRequest(FileInfoPOCO fileToBeSent)
        {
            this.FileToBeSent = fileToBeSent;
        }

        public async Task<RequestStatus> DoRequestAsync(HttpClient httpClient, IHandler handler)
        {
            //в HttpClient уже будет лежать соответствующий URL
            HttpResponseMessage httpResponseMessage = await httpClient.PutAsync(Href, new ByteArrayContent(await File.ReadAllBytesAsync(FileToBeSent.FileInfo.FullName)));

            return await handler.HandleAsync(httpResponseMessage, OperationType.UploadFile, FileToBeSent);
        }
    }
}
