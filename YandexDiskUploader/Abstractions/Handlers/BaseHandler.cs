using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YandexDiskUploader.Abstractions.Requests;

namespace YandexDiskUploader.Abstractions.Handlers
{
    public interface IHandler
    {
        IHandler SetNext(IHandler handler);

        Task<RequestStatus> HandleAsync(HttpResponseMessage httpResponse);
    }

    public abstract class AbstractHandler : IHandler
    {
        private IHandler _nextHandler;

        public IHandler SetNext(IHandler handler)
        {
            this._nextHandler = handler;

            return handler;
        }

        public virtual async Task<RequestStatus> HandleAsync(HttpResponseMessage httpResponse)
        {
            RequestStatus requestStatus = RequestStatus.OK;

            if (this._nextHandler != null)
            {
                requestStatus = await this._nextHandler.HandleAsync(httpResponse).ConfigureAwait(false);
            }

            return requestStatus;
        }
    }
}
