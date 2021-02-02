using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace YandexDiskUploader.Abstractions.Handlers.Factories
{
    public class ErrorOkHandlerChain : HandlersChainFactory
    {
        public ErrorOkHandlerChain(OperationType operationType, params object[] parameters) : base(operationType, parameters) { }

        public override IHandler GetHandlersChain()
        {
            HttpClient httpClient = _params[0] as HttpClient;

            AbstractHandler abstractChain = new ErrorHandler(this.operationType, httpClient);

            abstractChain.SetNext(new OKHandler(this.operationType, httpClient));

            return abstractChain;
        }
    }
}