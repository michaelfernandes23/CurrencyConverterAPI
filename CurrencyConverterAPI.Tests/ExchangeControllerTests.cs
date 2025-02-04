using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverterAPI.Controllers;
using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Services;

namespace CurrencyConverterAPI.Tests
{
    public class ExchangeControllerTests
    {
        private readonly Mock<IExchangeService> _exchangeServiceMock;
        private readonly ExchangeController _controller;

        public ExchangeControllerTests()
        {
            _exchangeServiceMock = new Mock<IExchangeService>();
            _controller = new ExchangeController(_exchangeServiceMock.Object);
        }

        [Fact]
        public async Task GetLatestRates_ReturnsOkResult_WhenServiceReturnsData()
        {
            // Arrange
            var baseCurrency = "EUR";
            var expectedResponse = new ExchangeRateResponse
            {
                Amount = 1,
                Base = baseCurrency,
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Rates = new Dictionary<string, decimal>
                {
                    { "USD", 1.12m },
                    { "GBP", 0.85m }
                }
            };

            _exchangeServiceMock.Setup(service => service.GetLatestRatesAsync(baseCurrency))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetLatestRates(baseCurrency);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task GetLatestRates_Returns503_WhenServiceReturnsNull()
        {
            // Arrange
            var baseCurrency = "EUR";
            _exchangeServiceMock.Setup(service => service.GetLatestRatesAsync(baseCurrency))
                .ReturnsAsync((ExchangeRateResponse)null);

            // Act
            var result = await _controller.GetLatestRates(baseCurrency);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(503, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task ConvertAmount_ReturnsBadRequest_ForRestrictedCurrency()
        {
            // Act: "TRY" is a restricted currency for conversion
            var result = await _controller.ConvertAmount("TRY", "USD", 100);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ConvertAmount_ReturnsOkResult_WhenServiceReturnsData()
        {
            // Arrange
            var from = "USD";
            var to = "GBP";
            var amount = 100m;
            var expectedResponse = new ConversionResponse
            {
                Amount = amount,
                Base = from,
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Rates = new Dictionary<string, decimal> { { to, 85m } }
            };

            _exchangeServiceMock.Setup(service => service.ConvertCurrencyAsync(from, to, amount))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.ConvertAmount(from, to, amount);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task GetHistoricalRates_ReturnsOkResult_WhenServiceReturnsData()
        {
            // Arrange
            var baseCurrency = "EUR";
            var start = new DateTime(2020, 1, 1);
            var end = new DateTime(2020, 1, 31);
            var page = 1;
            var pageSize = 10;
            var expectedResponse = new HistoricalResponse
            {
                Amount = 1,
                Base = baseCurrency,
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Rates = new Dictionary<string, decimal> { { "2020-01-01", 1.10m } }
            };

            _exchangeServiceMock.Setup(service =>
                service.GetHistoricalRatesAsync(baseCurrency, start, end, page, pageSize))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetHistoricalRates(baseCurrency, start, end, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }
    }
}
