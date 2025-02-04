using CurrencyConverterAPI.Models;

namespace CurrencyConverterAPI.Services
{
    public interface IExchangeService
    {
        Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency);
        Task<ConversionResponse> ConvertCurrencyAsync(string from, string to, decimal amount);
        Task<HistoricalResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime start, DateTime end, int page, int pageSize);
    }
}
