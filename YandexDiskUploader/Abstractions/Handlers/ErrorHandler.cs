using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YandexDiskUploader.Abstractions.Handlers.Factories;
using YandexDiskUploader.Abstractions.POCO;
using YandexDiskUploader.Abstractions.Requests;
using YandexDiskUploader.Abstractions.Requests.Factory;

namespace YandexDiskUploader.Abstractions.Handlers
{
    public class ErrorHandler : AbstractHandler
    {
        public ErrorHandler(HttpClient httpClient) : base(httpClient) { }

        public override async Task<RequestStatus> HandleAsync(HttpResponseMessage httpResponse, OperationType operationType, params object[] parameters)
        {
            if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                #region CreateFolderHandler

                //если папка создана, то ошибки нет
                if ((operationType == OperationType.CreateFolder || operationType == OperationType.UploadFile) && httpResponse.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return await base.HandleAsync(httpResponse, operationType, parameters).ConfigureAwait(false);
                }

                #endregion

                string enumValue = String.Empty;

                switch (operationType)
                {
                    case OperationType.GetFolder:
                        enumValue = "получения информации о папке";
                        break;
                    case OperationType.UploadPath:
                    case OperationType.UploadFile:
                        enumValue = "загрузки файла";
                        break;
                    case OperationType.CreateFolder:
                        enumValue = "создания папки";
                        break;
                }

                ErrorPOCO error = await System.Text.Json.JsonSerializer.DeserializeAsync<ErrorPOCO>(await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false));

                #region GetFolderHandler

                if (operationType == OperationType.GetFolder && httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ConsoleExtensions.Write(error.message + " " + "Пытаюсь создать папку...", true);

                    //создаем папку на яндекс диске из path'a, на который пытались отправить запрос по инфо о папке
                    IRequest request = RequestFactory.GetRequest<CreateFolderRequest>();

                    ((CreateFolderRequest)request).RequestedPath = httpResponse.RequestMessage.RequestUri.Query;

                    IHandler requestHandler = this;

                    RequestStatus folderCreationRequestStatus = await request.DoRequestAsync(this._httpClient, requestHandler).ConfigureAwait(false);

                    //успешно создана
                    if (folderCreationRequestStatus == RequestStatus.OK)
                    {
                        ConsoleExtensions.WriteLine(" OK");

                        return await base.HandleAsync(httpResponse, operationType, parameters).ConfigureAwait(false);
                    }
                    else
                    {
                        ConsoleExtensions.Write("\n");
                    }
                }

                #endregion

                string mainError = String.Format("При выполнении операции {0} произошла ошибка: {1} ({2})", enumValue, error.ToString(), ((int)httpResponse.StatusCode).ToString());

                //нужно выдать ошибку в лог и проапдейтить консоль
                if (operationType == OperationType.UploadPath 
                    || operationType == OperationType.UploadFile)
                {
                    ConsoleExtensions.UpdateUploadStatus(parameters[0] as FileInfoPOCO, "Ошибка");

                    Logger.LogError(mainError);
                }
                //для всех остальных - в консоль
                else
                {
                    Console.WriteLine(mainError);
                }

                return RequestStatus.Failed;
            }

            //без ошибки - дальше по цепочке обработчиков
            return await base.HandleAsync(httpResponse, operationType, parameters).ConfigureAwait(false);
        }
    }
}
