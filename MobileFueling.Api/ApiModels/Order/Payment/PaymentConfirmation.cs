using Newtonsoft.Json;

namespace MobileFueling.Api.ApiModels.Order.Payment
{
    public class PaymentReturn
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "redirect";

        [JsonProperty(PropertyName = "return_url")]
        public string ReturnUrl { get; set; }
    }

    public class PaymentConfirmation
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "redirect";

        [JsonProperty(PropertyName = "confirmation_url")]
        public string ConfirmationUrl { get; set; }
    }
}