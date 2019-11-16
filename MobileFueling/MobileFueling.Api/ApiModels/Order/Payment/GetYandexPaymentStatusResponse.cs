using Newtonsoft.Json;

namespace MobileFueling.Api.ApiModels.Order.Payment
{
    /// <summary>
    /// Статус заказа в Яндекс.Кассе
    /// </summary>
    public class GetYandexPaymentStatusResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "paid")]
        public bool Paid { get; set; }
    }
}