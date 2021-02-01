using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace YandexDiskUploader.Abstractions.Requests
{
    public interface IRequest
    {
        System.Threading.Tasks.Task<RequestStatus> DoRequestAsync(HttpClient httpClient);
    }

    public enum RequestStatus
    {
        OK,
        Failed
    }
}
