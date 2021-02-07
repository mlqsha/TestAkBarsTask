using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YandexDiskUploader.Abstractions.Handlers.Factories
{
    public abstract class HandlersChainFactory
    {
        public abstract IHandler GetHandlersChain();
    }
}
