using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YandexDiskUploader.Abstractions.POCO;
using YandexDiskUploader.Abstractions.Requests;

namespace YandexDiskUploader.Abstractions.Handlers
{
    public class ErrorHandler : AbstractHandler
    {
        public override async Task<RequestStatus> HandleAsync(HttpResponseMessage httpResponse)
        {
            RequestStatus requestStatus = RequestStatus.OK;

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ErrorPOCO errorPoco = System.Text.Json.JsonSerializer.Deserialize<ErrorPOCO>(await httpResponse.Content.ReadAsStringAsync());
            }
            else
            {
                requestStatus = await base.HandleAsync(httpResponse);
            }

            return requestStatus;
        }
    }
}
