using Core.Contract.Services.Requests.ExchangeService;
using Core.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace App.ExchangeApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExchangeController : ControllerBase
{
    private readonly IExchangeService _exchangeService;

    public ExchangeController(IExchangeService exchangeService) =>
        _exchangeService = exchangeService;

    [HttpPost("Convert")]
    public async Task<string> ConvertCurrencyAsync(decimal value, string currencyFrom = "EUR", string currencyTo = "PLN")
    {
        var result = await _exchangeService.CurrencyConvertAsync(new CurrencyConvertRequest
        {
            CurrencyFrom = currencyFrom,
            CurrencyTo = currencyTo,
            Value = value
        });

        return $"{value} {currencyFrom} = {result.Value} {currencyTo}";
    }

    [HttpPost("Trade")]
    public async Task<string> TradeCurrencyAsync(decimal value, string currencyFrom = "EUR", string currencyTo = "PLN")
    {
        var result = await _exchangeService.CurrencyTradeAsync(new CurrencyTradeRequest
        {
            ClientIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            CurrencyFrom = currencyFrom,
            CurrencyTo = currencyTo,
            Value = value
        });

        if (result.Success)
            return $"OK - {result.Message}";
        else
            return $"ERROR - {result.Message}";
    }
}