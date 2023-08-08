using Microsoft.Extensions.Configuration;

namespace Test.Core.Services.Fakes;

public class FakeConfiguration
{
    private IConfiguration _configuration;

    public IConfiguration Object => _configuration;

    public void Setup(IEnumerable<KeyValuePair<string, string>> initialData)
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(initialData)
            .Build();
    }
}