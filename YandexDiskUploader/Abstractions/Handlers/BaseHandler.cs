using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YandexDiskUploader.Abstractions.POCO;
using YandexDiskUploader.Abstractions.Requests;

namespace YandexDiskUploader.Abstractions.Handlers
{
    public interface IHandler
    {
        IHandler SetNext(IHandler handler);

        Task<ErrorPOCO> HandleAsync(HttpResponseMessage httpResponse);
    }

    public abstract class AbstractHandler : IHandler
    {
        protected OperationType _operationType;

        protected HttpClient _httpClient;

        private IHandler _nextHandler;

        public AbstractHandler(OperationType operationType, HttpClient httpClient)
        {
            this._operationType = operationType;

            this._httpClient = httpClient;
        }

        public IHandler SetNext(IHandler handler)
        {
            this._nextHandler = handler;

            return handler;
        }

        public virtual async Task<ErrorPOCO> HandleAsync(HttpResponseMessage httpResponse)
        {
            ErrorPOCO errorPoco = null;

            if (this._nextHandler != null)
            {
                errorPoco = await this._nextHandler.HandleAsync(httpResponse).ConfigureAwait(false);
            }

            return errorPoco;
        }
    }

    public enum OperationType
    {
        GetFolder = 1,
        UploadFile = 2,
        CreateFolder = 3
    }
}
