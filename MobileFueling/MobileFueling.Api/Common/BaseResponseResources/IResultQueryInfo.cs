using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace MobileFueling.Api.Common.BaseResponseResources
{
    public interface IResultQueryInfo
    {
        ICollection<Message> Messages { get; }

        void AddMessage(MessageType type, LocalizedString text);

        void AddException(Exception ex);

        bool HasError { get; }
    }
}