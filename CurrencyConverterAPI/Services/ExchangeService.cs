using CurrencyConverterAPI.Models;
using System.Net.Http.Json;
using Polly;
using Polly.Retry;

namespace CurrencyConverterAPI.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly SemaphoreSlim _semaphore;

        public ExchangeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Creates a retry policy that retries 3 times with a 2-second wait between retries.
            _retryPolicy = Policy.Handle<Exception>()
                                 .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));

            // Limit to 5 concurrent requests to the exchange rate API.
            _semaphore = new SemaphoreSlim(5, 5);
        }

        /// <summary>
        /// Retrieve the latest exchange rates for a specific base currency.
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <returns></returns>
        public async Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency)
        {
            await _semaphore.WaitAsync();
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetFromJsonAsync<ExchangeRateResponse>($"latest?from={baseCurrency}"));
                return response;
            }
            catch (Exception)
            {
                // TODO: Add logs later
                return null;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Convert the specified amount from one currency to another.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<ConversionResponse> ConvertCurrencyAsync(string from, string to, decimal amount)
        {
            await _semaphore.WaitAsync();
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetFromJsonAsync<ConversionResponse>($"latest?amount={amount}&from={from}&to={to}"));
                return response;
            }
            catch (Exception)
            {
            	// TODO: Add logs later
                return null;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Retrieve historical exchange rates for a specific base currency within a date range.
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<HistoricalResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime start, DateTime end, int page, int pageSize)
        {
            await _semaphore.WaitAsync();
            try
            {
                string url = $"{start:yyyy-MM-dd}..{end:yyyy-MM-dd}?from={baseCurrency}";
                var rawResponse = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetFromJsonAsync<HistoricalResponse>(url));

                if (rawResponse != null && rawResponse.Rates != null)
                {
                    // Simple in-memory pagination of the returned rates dictionary
                    var pagedRates = rawResponse.Rates
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    rawResponse.Rates = pagedRates;
                }
                return rawResponse;
            }
            catch (Exception)
            {
            	// TODO: Add logs later
                return null;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
