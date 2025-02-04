using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterAPI.Controllers
{
    [ApiController]
    [Route("api/exchange")]
    public class ExchangeController : ControllerBase
    {
        private readonly IExchangeService _exchangeService;
        // Restricted currencies for the conversion endpoint
        private readonly HashSet<string> _restrictedCurrencies = new() { "TRY", "PLN", "THB", "MXN" };

        public ExchangeController(IExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        // GET: /api/exchange/latest?base=EUR
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency = "EUR")
        {
            var result = await _exchangeService.GetLatestRatesAsync(baseCurrency);
            if (result == null)
                return StatusCode(503, "External service unavailable.");
            return Ok(result);
        }

        // GET: /api/exchange/convert?from=USD&to=GBP&amount=100
        [HttpGet("convert")]
        public async Task<IActionResult> ConvertAmount([FromQuery] string fromCurrency, [FromQuery] string toCurrency, [FromQuery] decimal amount)
        {
            // Check if either currency is restricted for conversion
            if (_restrictedCurrencies.Contains(fromCurrency.ToUpper()) || _restrictedCurrencies.Contains(toCurrency.ToUpper()))
                return BadRequest("Conversion for the provided currencies is not allowed.");

            var result = await _exchangeService.ConvertCurrencyAsync(fromCurrency, toCurrency, amount);
            if (result == null)
                return StatusCode(503, "External service unavailable.");
            return Ok(result);
        }

        // GET: /api/exchange/historical?base=EUR&start=2020-01-01&end=2020-01-31&page=1&pageSize=10
        [HttpGet("historical")]
        public async Task<IActionResult> GetHistoricalRates(
            [FromQuery] string baseCurrency,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _exchangeService.GetHistoricalRatesAsync(baseCurrency, start, end, page, pageSize);
            if (result == null)
                return StatusCode(503, "External service unavailable.");
            return Ok(result);
        }
    }
}
