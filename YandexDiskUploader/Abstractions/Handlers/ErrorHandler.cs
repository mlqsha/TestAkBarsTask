using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YandexDiskUploader.Abstractions.Handlers.Factories;
using YandexDiskUploader.Abstractions.POCO;
using YandexDiskUploader.Abstractions.Requests;

namespace YandexDiskUploader.Abstractions.Handlers
{
    public class ErrorHandler : AbstractHandler
    {
        public ErrorHandler(OperationType operationType, HttpClient httpClient) : base(operationType, httpClient) { }

        public override async Task<ErrorPOCO> HandleAsync(HttpResponseMessage httpResponse)
        {
            if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ErrorPOCO error = await System.Text.Json.JsonSerializer.DeserializeAsync<ErrorPOCO>(await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false));

                IRequest request = null;

                if (this._operationType == OperationType.GetFolder && httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine(error.message + " " + "Пытаюсь создать папку...");

                    //создаем папку на яндекс диске из path'a, на который пытались отправить запрос по инфо о папке
                    request = new CreateFolderRequest(httpResponse.RequestMessage.RequestUri.Query);

                    RequestStatus folderCreationRequestStatus = await request.DoRequestAsync(this._httpClient, new ErrorOkHandlerChain(OperationType.CreateFolder)).ConfigureAwait(false);

                    if (folderCreationRequestStatus == RequestStatus.OK)
                    {
                        return await base.HandleAsync(httpResponse).ConfigureAwait(false);
                    }
                }
                
                //
                if (this._operationType == OperationType.UploadFile && httpResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    //создания папки на яндекс диске

                    return await base.HandleAsync(httpResponse).ConfigureAwait(false);
                }

                string enumValue = String.Empty;

                switch (this._operationType)
                {
                    case OperationType.GetFolder:
                        enumValue = "получения информации о ресурсе";
                        break;
                    case OperationType.UploadFile:
                        enumValue = "загрузки ресурса";
                        break;
                    case OperationType.CreateFolder:
                        enumValue = "создания папки ресурса";
                        break;
                }

                Console.WriteLine(String.Format("При выполнении операции {0} произошла ошибка: {1} ({2})", enumValue, error.ToString(), ((int)httpResponse.StatusCode).ToString()));

                return error;
            }

            return await base.HandleAsync(httpResponse).ConfigureAwait(false);
        }
    }
}
