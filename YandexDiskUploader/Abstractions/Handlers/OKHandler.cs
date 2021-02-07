using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YandexDiskUploader.Abstractions.Handlers.Factories;
using YandexDiskUploader.Abstractions.POCO;
using YandexDiskUploader.Abstractions.Requests;
using YandexDiskUploader.Abstractions.Requests.Factory;

namespace YandexDiskUploader.Abstractions.Handlers
{
    public class OKHandler : AbstractHandler
    {
        public OKHandler(HttpClient httpClient) : base(httpClient) { }

        public override async Task<RequestStatus> HandleAsync(HttpResponseMessage httpResponse, OperationType operationType, params object[] parameters)
        {
            //обработка только этих операций
            if (operationType == OperationType.UploadPath || operationType == OperationType.UploadFile)
            {
                FileInfoPOCO fileToBeUploaded = parameters[0] as FileInfoPOCO;

                if (operationType == OperationType.UploadPath)
                {
                    LinkPOCO linkObject = await System.Text.Json.JsonSerializer.DeserializeAsync<LinkPOCO>(await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false));

                    //делаем запрос put на адрес, который прислали
                    IRequest request = RequestFactory.GetRequest<UploadFileRequest>();

                    UploadFileRequest uploadFileRequest = request as UploadFileRequest;

                    uploadFileRequest.FileToBeSent = fileToBeUploaded;

                    uploadFileRequest.Href = linkObject.href;

                    IHandler handler = new ErrorOkHandlerChain(this._httpClient).GetHandlersChain();

                    //fire and forget, разрулится в error-handler'e
                    //если OK, то придет в блок ниже
                    return await request.DoRequestAsync(this._httpClient, handler).ConfigureAwait(false);
                }
                else if (operationType == OperationType.UploadFile)
                {
                    ConsoleExtensions.UpdateUploadStatus(fileToBeUploaded, "Загружен");
                }
            }

            return await base.HandleAsync(httpResponse, operationType, parameters).ConfigureAwait(false);
        }
    }
}
