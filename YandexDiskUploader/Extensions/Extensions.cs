using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using YandexDiskUploader.Abstractions.POCO;

namespace YandexDiskUploader
{
    public static class ConsoleExtensions
    {
        private static object lockObject;

        private static Logger _logger;

        public static int i { get; set; } = 0;
        
        static ConsoleExtensions()
        {
            lockObject = new object();

            _logger = new Logger();
        }

        public static void UpdateUploadStatus(FileInfoPOCO fileInfo, string status)
        {
            lock (lockObject)
            {
                int currentCursorPosition = Console.CursorTop;

                Console.SetCursorPosition(0, fileInfo.indexInConsole);

                string textToBeWritten = String.Format("\r{0} Статус: {1}", fileInfo.FileInfo.Name, status);

                Console.Write(textToBeWritten);

                Console.Write(new string(' ', Console.WindowWidth - textToBeWritten.Length - 1));

                Console.SetCursorPosition(0, currentCursorPosition);
            }
        }

        public static void WriteLine(string text, bool needToCountLineNumber = false)
        {
            lock (lockObject)
            {
                Console.WriteLine(text);

                if (needToCountLineNumber)
                {
                    i++;
                }
            }
        }        

        public static void Write(string text, bool needToCountLineNumber = false)
        {
            lock (lockObject)
            {
                Console.Write(text);

                if (needToCountLineNumber)
                {
                    i++;
                }
            }
        }

        public static string ReadLine()
        {
            lock (lockObject)
            {
                i++;

                return Console.ReadLine();
            }
        }
    }

    public class Logger
    {
        public static readonly string FILENAME;

        static Logger()
        {
            FILENAME = String.Format("log_{0}.log", DateTime.Now.ToString("ddMMyyyy"));
        }

        public static void LogError(string message)
        {
            File.AppendAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/" + FILENAME, message + "\n");
        }
    }
}
