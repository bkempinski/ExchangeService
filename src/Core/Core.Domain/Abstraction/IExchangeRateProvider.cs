using Core.Contract.Providers.Requests.ExchangeRateProvider;
using Core.Contract.Providers.Responses.ExchangeRateProvider;

namespace Core.Domain.Abstraction;

public interface IExchangeRateProvider
{
    string Name { get; }

    Task<GetExchangeRateResponse> GetExchangeRateAsync(GetExchangeRateRequest request);
}