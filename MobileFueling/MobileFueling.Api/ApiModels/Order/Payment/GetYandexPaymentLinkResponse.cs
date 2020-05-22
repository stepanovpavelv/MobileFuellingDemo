using Newtonsoft.Json;

namespace MobileFueling.Api.ApiModels.Order.Payment
{
    /// <summary>
    /// Ответ на создание платежа на стороне Яндекс.Кассы
    /// </summary>
    public class GetYandexPaymentLinkResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "paid")]
        public bool Paid { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public PaymentAmount Amount { get; set; }

        [JsonProperty(PropertyName = "confirmation")]
        public PaymentConfirmation Confirmation { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "refundable")]
        public bool Refundable { get; set; }

        [JsonProperty(PropertyName = "test")]
        public bool Test { get; set; }
    }
}