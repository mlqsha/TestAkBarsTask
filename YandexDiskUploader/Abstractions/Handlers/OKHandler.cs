using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YandexDiskUploader.Abstractions.Requests;

namespace YandexDiskUploader.Abstractions.Handlers
{
    public class OKHandler : AbstractHandler
    {
        public override async Task<RequestStatus> HandleAsync(HttpResponseMessage httpResponse)
        {
            RequestStatus requestStatus = RequestStatus.OK;

            if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                await base.HandleAsync(httpResponse).ConfigureAwait(false);
            }

            return requestStatus;
        }
    }
}
