using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace YandexDiskUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Неверное количество аргументов");
                Console.WriteLine(@"Пример использования: C:\путь\до\файлов\ путь до яндекс диска");

                return;
            }

            DirectoryInfo di = new DirectoryInfo(args[0]);

            if (!di.Exists)
            {
                Console.WriteLine(String.Format("Выбранная директория {0} не существует", args[0]));

                return;
            }

            FileInfo[] files = di.GetFiles();

            if (files.Length == 0)
            {
                Console.WriteLine(String.Format("Выбранная директория {0} не содержит в себе файлов для загрузки", args[0]));
                return;
            }

            Console.WriteLine("Начинаю загружать файлы на Яндекс.Диск...");

            List<Task> filesTasks = new List<Task>();

            foreach (FileInfo fileInfo in files)
            {
                byte[] byteArr = File.ReadAllBytes(fileInfo.FullName);
            }

            Task.WaitAll(filesTasks.ToArray());

            Console.WriteLine("Загрузка файлов на Яндекс.Диск успешно завершена!");
        }
    }
}
