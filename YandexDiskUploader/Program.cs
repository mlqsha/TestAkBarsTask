using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using YandexDiskUploader.Abstractions.Requests;

namespace YandexDiskUploader
{
    class Program
    {
        private static Dictionary<string, IRequest> _requests = new Dictionary<string, IRequest>();

        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Неверное количество аргументов");
                Console.WriteLine(@"Пример использования: C:\путь\до\файлов\ директория/яндекс/диска");

                return;
            }

            DirectoryInfo di = new DirectoryInfo(args[0]);

            if (!di.Exists)
            {
                Console.WriteLine(String.Format("Выбранная директория {0} не существует", args[0]));

                return;
            }

            string[] yandexDiskFolderPathElems = args[1].Split("/");

            foreach (string pathElem in yandexDiskFolderPathElems)
            {
                if (String.IsNullOrEmpty(pathElem))
                {
                    Console.WriteLine(String.Format("Путь к папке Яндекс.Диска не может быть пустым", args[1]));

                    return;
                }
            }

            _requests.Add(nameof(GetFolderRequest), new GetFolderRequest(yandexDiskFolderPathElems));

            FileInfo[] files = di.GetFiles();

            if (files.Length == 0)
            {
                Console.WriteLine(String.Format("Выбранная директория {0} не содержит в себе файлов для загрузки", args[0]));
                return;
            }

            _requests.Add(nameof(UploadFilesRequest), new UploadFilesRequest());

            Console.WriteLine("Введите токен Яндекс.Диска");

            string token = Console.ReadLine();

            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://cloud-api.yandex.net/");

            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", token);

            IRequest request = _requests[nameof(GetFolderRequest)];

            //стратегия получения папок
            RequestStatus requestStatus = await request.DoRequestAsync(httpClient);

            if (requestStatus != RequestStatus.OK)
            {
                return;
            }

            request = _requests[nameof(UploadFilesRequest)];

            //статегия загрузки ресурсов
            requestStatus = await request.DoRequestAsync(httpClient);

            if (requestStatus != RequestStatus.OK)
            {
                return;
            }

            Console.WriteLine("Работа загрузчика файлов завершена");
        }
    }
}
