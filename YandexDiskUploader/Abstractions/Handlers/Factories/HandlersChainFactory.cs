using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YandexDiskUploader.Abstractions.Handlers.Factories
{
    public abstract class HandlersChainFactory
    {
        protected OperationType operationType;

        protected List<object> _params;

        protected HandlersChainFactory(OperationType operationType, params object[] parameters)
        {
            this.operationType = operationType;

            this._params = parameters.ToList();
        }

        public abstract IHandler GetHandlersChain();
    }
}
