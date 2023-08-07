using Core.Contract.Services.Requests.ExchangeService;
using Core.Contract.Services.Responses.ExchangeService;

namespace Core.Services.Abstraction;

public interface IExchangeService
{
    Task<ConvertCurrencyResponse> ConvertCurrencyAsync(ConvertCurrencyRequest request);
}