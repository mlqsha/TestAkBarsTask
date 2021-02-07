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

        Task<RequestStatus> HandleAsync(HttpResponseMessage httpResponse, OperationType operationType, params object[] parameters);
    }

    public abstract class AbstractHandler : IHandler
    {
        protected HttpClient _httpClient;

        private IHandler _nextHandler;

        public AbstractHandler(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public IHandler SetNext(IHandler handler)
        {
            this._nextHandler = handler;

            return handler;
        }

        public virtual async Task<RequestStatus> HandleAsync(HttpResponseMessage httpResponse, OperationType operationType, params object[] parameters)
        {
            RequestStatus requestStatus = RequestStatus.OK;

            if (this._nextHandler != null)
            {
                requestStatus = await this._nextHandler.HandleAsync(httpResponse, operationType, parameters).ConfigureAwait(false);
            }

            return requestStatus;
        }
    }

    public enum OperationType
    {
        GetFolder,
        UploadPath,
        CreateFolder,
        UploadFile
    }
}
