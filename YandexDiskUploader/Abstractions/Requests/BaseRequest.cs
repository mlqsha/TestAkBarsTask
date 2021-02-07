using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using YandexDiskUploader.Abstractions.Handlers;
using YandexDiskUploader.Abstractions.Handlers.Factories;

namespace YandexDiskUploader.Abstractions.Requests
{
    public interface IRequest
    {
        System.Threading.Tasks.Task<RequestStatus> DoRequestAsync(HttpClient httpClient, IHandler handler);
    }

    public enum RequestStatus
    {
        OK,
        Failed
    }
}
