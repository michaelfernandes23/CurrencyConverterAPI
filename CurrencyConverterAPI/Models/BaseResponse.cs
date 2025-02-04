using System.Text.Json.Serialization;

namespace CurrencyConverterAPI.Models
{
    public class BaseResponse
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        
        [JsonPropertyName("base")]
        public string Base { get; set; }
        
        [JsonPropertyName("date")]
        public string Date { get; set; }
        
        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
