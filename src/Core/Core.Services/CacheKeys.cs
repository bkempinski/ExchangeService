namespace Core.Services;

public static class CacheKeys
{
    public static string GetExchangeRate(string exchangeRateProviderName, string baseCurrency, string currency) =>
        $"GetExchangeRate_{exchangeRateProviderName}_{baseCurrency}_{currency}";
}