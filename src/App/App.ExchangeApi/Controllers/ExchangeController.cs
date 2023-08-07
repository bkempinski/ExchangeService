using Core.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace App.ExchangeApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExchangeController : ControllerBase
{
    private readonly IExchangeService _exchangeService;

    public ExchangeController
        (
            IExchangeService exchangeService
        ) => (_exchangeService)
            = (exchangeService);

    [HttpPost]
    public async Task<decimal> ConvertCurrencyAsync(decimal value)
    {
        var result = await _exchangeService.ConvertCurrencyAsync(new Core.Contract.Services.Requests.ExchangeService.ConvertCurrencyRequest
        {
            Currency = "PLN",
            FromCurrency = "EUR",
            FromValue = value
        });

        return result.Value;
    }
}