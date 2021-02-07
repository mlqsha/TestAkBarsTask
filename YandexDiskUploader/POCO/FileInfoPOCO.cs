using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YandexDiskUploader.Abstractions.POCO
{
    public class FileInfoPOCO
    {
        public FileInfoPOCO(FileInfo fileInfo, int consoleOutputIndex)
        {
            this.FileInfo = fileInfo;

            this.indexInConsole = consoleOutputIndex;
        }

        public FileInfo FileInfo { get; set; }

        public int indexInConsole { get; set; }
    }
}
