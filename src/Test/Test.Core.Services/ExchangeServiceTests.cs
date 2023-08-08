using AutoFixture;
using Core.Contract.Providers.Requests.ExchangeRateProvider;
using Core.Contract.Providers.Responses.ExchangeRateProvider;
using Core.Contract.Services.Requests.ExchangeService;
using Core.Domain.Abstraction;
using Core.Domain.Abstraction.Repositories;
using Core.Services.Implementation;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Test.Core.Services.Fakes;
using Xunit;

namespace Test.Core.Services;

public class ExchangeServiceTests
{
    private Mock<ILogger<ExchangeService>> _loggerMock;
    private FakeConfiguration _fakeConfiguration;
    private FakeCacheService _fakeCacheService;
    private Mock<IExchangeRateProvider> _exchangeRateProviderMock;
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<ITradeRepository> _tradeRepositoryMock;
    private Fixture _fixture;

    public ExchangeServiceTests()
    {
        _loggerMock = new Mock<ILogger<ExchangeService>>();
        _fakeConfiguration = new FakeConfiguration();
        _fakeCacheService = new FakeCacheService();
        _exchangeRateProviderMock = new Mock<IExchangeRateProvider>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _tradeRepositoryMock = new Mock<ITradeRepository>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task CurrencyConvertAsync_WhenRequestIsNull_ShouldThrow()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var act = () => sut.CurrencyConvertAsync(null);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task CurrencyConvertAsync_WhenCurrencyFromIsNullOrEmpty_ShouldThrow()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var request = _fixture
            .Build<CurrencyConvertRequest>()
            .With(r => r.CurrencyFrom, (string)null)
            .Create();
        var act = () => sut.CurrencyConvertAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task CurrencyConvertAsync_WhenCurrencyToIsNullOrEmpty_ShouldThrow()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var request = _fixture
            .Build<CurrencyConvertRequest>()
            .With(r => r.CurrencyTo, (string)null)
            .Create();
        var act = () => sut.CurrencyConvertAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task CurrencyConvertAsync_WhenExchangeRateProviderNameIsNullOrEmpty_ShouldThrow()
    {
        // Arrange
        SetupMocks(defaultExchangeRateProviderName: null);

        var sut = CreateSut();

        // Act
        var request = _fixture
            .Build<CurrencyConvertRequest>()
            .With(r => r.ExchangeRateProviderName, (string)null)
            .Create();
        var act = () => sut.CurrencyConvertAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task CurrencyConvertAsync_WhenExchangeRateProviderNameDoesNotExists_ShouldThrow()
    {
        // Arrange
        SetupMocks();

        var sut = CreateSut();

        // Act
        var request = _fixture
            .Build<CurrencyConvertRequest>()
            .With(r => r.ExchangeRateProviderName, "TestNotExisting")
            .Create();
        var act = () => sut.CurrencyConvertAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Theory]
    [InlineData(1, 0, 0)]
    [InlineData(1, 1, 1)]
    [InlineData(12.34, 56.78, 700.6652)]
    [InlineData(1.2, 34.56, 41.472)]
    public async Task CurrencyConvertAsync_WhenAllIsCorrect_CurrencyIsConverted(decimal exchangeRate, decimal value, decimal exchangeValue)
    {
        // Arrange
        SetupMocks(exchangeRate: exchangeRate);

        var sut = CreateSut();

        // Act
        var request = _fixture
            .Build<CurrencyConvertRequest>()
            .With(r => r.Value, value)
            .With(r => r.ExchangeRateProviderName, (string)null)
            .Create();
        var act = await sut.CurrencyConvertAsync(request);

        // Assert
        act.CurrencyFrom.Should().Be(request.CurrencyFrom);
        act.CurrencyTo.Should().Be(request.CurrencyTo);
        act.ExchangeRate.Should().Be(exchangeRate);
        act.Value.Should().Be(exchangeValue);
    }

    /* CurrencyTradeAsync unit tests goes here... */

    private ExchangeService CreateSut() =>
        new ExchangeService
        (
            _loggerMock.Object,
            _fakeConfiguration.Object,
            _fakeCacheService.Object,
            new[] { _exchangeRateProviderMock.Object },
            _clientRepositoryMock.Object,
            _tradeRepositoryMock.Object
        );

    private void SetupMocks
        (
            string defaultExchangeRateProviderName = "test.io",
            decimal exchangeRate = 1m
        )
    {
        _fakeConfiguration.Setup(new Dictionary<string, string>
        {
            {"DefaultExchangeRateProviderName", defaultExchangeRateProviderName},
        });

        _exchangeRateProviderMock.Setup(x => x.Name).Returns(defaultExchangeRateProviderName);
        _exchangeRateProviderMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<GetExchangeRateRequest>())).ReturnsAsync((GetExchangeRateRequest request) =>
            new GetExchangeRateResponse
            {
                CurrencyFrom = request.CurrencyFrom,
                CurrencyTo = request.CurrencyTo,
                ExchangeRate = exchangeRate,
                Date = DateTime.UtcNow
            });
    }
}