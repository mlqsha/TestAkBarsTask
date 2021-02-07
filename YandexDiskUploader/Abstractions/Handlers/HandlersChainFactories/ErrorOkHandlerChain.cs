using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace YandexDiskUploader.Abstractions.Handlers.Factories
{
    public class ErrorOkHandlerChain : HandlersChainFactory
    {
        private HttpClient _httpClient;

        private static IHandler HandlersChain;

        public ErrorOkHandlerChain(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public override IHandler GetHandlersChain()
        {
            if (HandlersChain == null)
            {
                AbstractHandler abstractChain = new ErrorHandler(this._httpClient);

                abstractChain.SetNext(new OKHandler(this._httpClient));

                HandlersChain = abstractChain;
            }

            return HandlersChain;
        }
    }
}