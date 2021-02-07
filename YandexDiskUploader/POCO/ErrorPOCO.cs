using System;
using System.Collections.Generic;
using System.Text;

namespace YandexDiskUploader.Abstractions.POCO
{
    public class ErrorPOCO
    {
        public string message { get; set; }
        public string description { get; set; }
        public string error { get; set; }

        public override string ToString()
        {
            return error;
        }
    }
}
