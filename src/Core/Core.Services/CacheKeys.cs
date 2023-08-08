namespace Core.Services;

public static class CacheKeys
{
    public static string GetExchangeRate(string exchangeRateProviderName, string currencyFrom, string currencyTo) =>
        $"GetExchangeRate_{exchangeRateProviderName}_{currencyFrom}_{currencyTo}";

    public static string GetTradesCount(int clientId) =>
        $"GetTradesCount_{clientId}";
}