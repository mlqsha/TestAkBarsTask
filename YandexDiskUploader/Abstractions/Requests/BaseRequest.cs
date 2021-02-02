using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using YandexDiskUploader.Abstractions.Handlers.Factories;

namespace YandexDiskUploader.Abstractions.Requests
{
    public interface IRequest
    {
        System.Threading.Tasks.Task<RequestStatus> DoRequestAsync(HttpClient httpClient, HandlersChainFactory chainFactory);
    }

    public enum RequestStatus
    {
        OK,
        Failed
    }
}
