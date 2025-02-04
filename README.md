# CurrencyConverterAPI

## Overview

CurrencyConverterAPI is a simple .NET Web API built to fetch the latest exchange rates, perform currency conversions, and retrieve historical exchange data from the Frankfurter API. 

## Features

- **Latest Rates Endpoint:**  
  `GET /api/exchange/latest?base=EUR`  
  Retrieves the latest exchange rates for the specified base currency.

- **Currency Conversion Endpoint:**  
  `GET /api/exchange/convert?from=USD&to=GBP&amount=100`  
  Converts an amount from one currency to another.  
  *Note:* Conversions involving TRY, PLN, THB, and MXN are restricted.

- **Historical Rates Endpoint:**  
  `GET /api/exchange/historical?base=EUR&start=2020-01-01&end=2020-01-31&page=1&pageSize=10`  
  Retrieves historical exchange rates for a given period with in-memory pagination.

- **Resilience & Concurrency:**  
  - Uses [Polly](https://github.com/App-vNext/Polly) to retry external API calls up to 3 times with a 2-second delay between attempts.
  - Uses `SemaphoreSlim` to limit the number of concurrent requests to the external API, protecting it from being overloaded.

## Prerequisites

- [.NET 8 or later](https://dotnet.microsoft.com/download)
- Visual Studio Code or any other preferred IDE

## Setup Instructions

**Clone the Repository:**
