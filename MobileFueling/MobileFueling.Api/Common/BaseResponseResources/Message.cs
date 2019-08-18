using Microsoft.Extensions.Localization;

namespace MobileFueling.Api.Common.BaseResponseResources
{
    public class Message
    {
        public LocalizedString Text { get; set; }
        public MessageType Type { get; set; }
    }
}