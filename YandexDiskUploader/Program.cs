using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using YandexDiskUploader.Abstractions.Handlers.Factories;
using YandexDiskUploader.Abstractions.POCO;
using YandexDiskUploader.Abstractions.Requests;
using YandexDiskUploader.Abstractions.Requests.Factory;

namespace YandexDiskUploader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                ConsoleExtensions.WriteLine("Неверное количество аргументов");
                ConsoleExtensions.WriteLine(@"Пример использования: C:\путь\до\файлов\ директория/яндекс/диска");

                return;
            }

            DirectoryInfo di = new DirectoryInfo(args[0]);

            if (!di.Exists)
            {
                ConsoleExtensions.WriteLine(String.Format("Выбранная директория {0} не существует", args[0]));

                return;
            }

            FileInfo[] files = di.GetFiles();

            if (files.Length == 0)
            {
                ConsoleExtensions.WriteLine(String.Format("Выбранная директория {0} не содержит в себе файлов для загрузки", args[0]));

                return;
            }

            ConsoleExtensions.WriteLine("Введите токен Яндекс.Диска", true);

            string token = ConsoleExtensions.ReadLine();

            ConsoleExtensions.WriteLine("Перезаписывать данные на Яндекс.Диске при загрузке файлов (y - да/n - нет)?", true);

            bool filesNeedToBeOverwritten = false;

            do
            {
                string yesOrNo = ConsoleExtensions.ReadLine();

                if (yesOrNo.Length == 1)
                {
                    yesOrNo = yesOrNo.ToLower();

                    //английская или русская раскладка
                    if (yesOrNo[0] == 'y' || yesOrNo[0] == 'у')
                    {
                        filesNeedToBeOverwritten = true;
                    }

                    break;
                }

                ConsoleExtensions.WriteLine("Введены неверные данные, дайте ответ в виде y - да/n - нет", true);

            } while (true);

            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://cloud-api.yandex.net/");

            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", token);

            IRequest request = RequestFactory.GetRequest<GetFolderRequest>();

            string[] yandexDiskFolderPathElems = args[1].Split("/").Where(x => !String.IsNullOrEmpty(x)).ToArray();

            ((GetFolderRequest)request).FolderPath = yandexDiskFolderPathElems;

            HandlersChainFactory hcf = new ErrorOkHandlerChain(httpClient);

            Abstractions.Handlers.IHandler handler = hcf.GetHandlersChain();

            RequestStatus requestStatus = RequestStatus.OK;

            try
            {
                requestStatus = await request.DoRequestAsync(httpClient, handler);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            if (requestStatus != RequestStatus.OK)
            {
                return;
            }

            string pathForUpload = String.Join(null, yandexDiskFolderPathElems.Select(x => x + "/"));

            IEnumerable<FileInfoPOCO> listOfFiles = files.Select((x, i) => new FileInfoPOCO(x, ConsoleExtensions.i + i));

            foreach (FileInfoPOCO fileInfoPOCO in listOfFiles)
            {
                ConsoleExtensions.WriteLine(String.Format("{0} Статус: В очереди", fileInfoPOCO.FileInfo.Name));
            }

            request = RequestFactory.GetRequest<UploadResourcePathRequest>();

            UploadResourcePathRequest publishResourceRequest = request as UploadResourcePathRequest;

            publishResourceRequest.FileInfos = listOfFiles;

            publishResourceRequest.FolderPath = pathForUpload;

            publishResourceRequest.FileOverwriting = filesNeedToBeOverwritten;

            try
            {
                requestStatus = await request.DoRequestAsync(httpClient, handler);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            if (requestStatus != RequestStatus.OK)
            {
                return;
            }

            Console.Write("\n");
            Console.WriteLine("Работа загрузчика файлов завершена");
        }
    }
}
