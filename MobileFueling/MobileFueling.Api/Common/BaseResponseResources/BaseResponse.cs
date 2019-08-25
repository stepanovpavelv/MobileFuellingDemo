using Microsoft.Extensions.Localization;
using System;

namespace MobileFueling.Api.Common.BaseResponseResources
{
    /// <summary>
    /// Базовый ответ Fuelling API
    /// </summary>
    public class BaseResponse
    {
        public ResultQueryInfo ResultQueryInfo { get; set; }

        public BaseResponse()
        {
            ResultQueryInfo = new ResultQueryInfo();
        }

        public void AddMessage(MessageType type, LocalizedString text)
        {
            ResultQueryInfo.AddMessage(type, text);
        }

        public void AddException(Exception ex)
        {
            ResultQueryInfo.AddException(ex);
        }

        public bool HasError()
        {
            return ResultQueryInfo.HasError;
        }
    }
}