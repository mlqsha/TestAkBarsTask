using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YandexDiskUploader.Abstractions.POCO;
using YandexDiskUploader.Abstractions.Requests;

namespace YandexDiskUploader.Abstractions.Handlers
{
    public class OKHandler : AbstractHandler
    {
        public OKHandler(OperationType operationType, HttpClient httpClient) : base(operationType, httpClient) { }

        public override async Task<ErrorPOCO> HandleAsync(HttpResponseMessage httpResponse)
        {
            //можно захендлить здесь в будущем
            //здесь реализовать уже асинхронную операцию которая чекает файлы
            if (this._operationType == OperationType.UploadFile)
            {

            }

            return await base.HandleAsync(httpResponse).ConfigureAwait(false);
        }
    }
}
