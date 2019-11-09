using Newtonsoft.Json;

namespace MobileFueling.Api.ApiModels.Order.Payment
{
    /// <summary>
    /// Запрос на создание платежа на стороне Яндекс.Кассы
    /// </summary>
    public class GetYandexPaymentLinkRequest
    {
        [JsonProperty(PropertyName = "amount")]
        public PaymentAmount Amount { get; set; }

        [JsonProperty(PropertyName = "capture")]
        public bool Capture { get; set; } = true;

        [JsonProperty(PropertyName = "confirmation")]
        public PaymentReturn Confirmation { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}