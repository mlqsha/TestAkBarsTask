using System;
using System.Collections.Generic;
using System.Text;

namespace YandexDiskUploader.Abstractions.Requests.Factory
{
    public class RequestFactory
    {
        public static T GetRequest<T>() where T : IRequest, new()
        {
            return new T();
        }
    }
}
