using Newtonsoft.Json;

namespace MobileFueling.Api.ApiModels.Order.Payment
{
    public class PaymentAmount
    {
        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; } = "RUB";
    }
}