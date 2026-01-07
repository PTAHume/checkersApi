using CheckersApi.Contracts;
using CheckersApi.Engine;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace CheckersApi.Tests.Engine;

public class CachedEngineAdapterTests
{
    private readonly Mock<IEngineAdapter> _innerAdapterMock;
    private readonly IMemoryCache _cache;
    private readonly CachedEngineAdapter _cachedAdapter;

    public CachedEngineAdapterTests()
    {
        _innerAdapterMock = new Mock<IEngineAdapter>();
        _cache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 100 });
        _cachedAdapter = new CachedEngineAdapter(_innerAdapterMock.Object, _cache);
    }

    [Fact]
    public void Suggest_FirstCall_CallsInnerAdapter()
    {
        // Arrange
        var request = CreateSuggestRequest("B:W18,19:B1,5");
        var expectedResponse = CreateSuggestResponse("18-14");

        _innerAdapterMock
            .Setup(x => x.Suggest(It.IsAny<SuggestRequest>(), It.IsAny<CancellationToken>()))
            .Returns(expectedResponse);

        // Act
        var result = _cachedAdapter.Suggest(request, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _innerAdapterMock.Verify(x => x.Suggest(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Suggest_SecondCallSamePosition_UsesCacheDoesNotCallInnerAdapter()
    {
        // Arrange
        var request = CreateSuggestRequest("B:W18,19:B1,5");
        var expectedResponse = CreateSuggestResponse("18-14");

        _innerAdapterMock
            .Setup(x => x.Suggest(It.IsAny<SuggestRequest>(), It.IsAny<CancellationToken>()))
            .Returns(expectedResponse);

        // Act
        var result1 = _cachedAdapter.Suggest(request, CancellationToken.None);
        var result2 = _cachedAdapter.Suggest(request, CancellationToken.None);

        // Assert
        result1.Should().BeEquivalentTo(expectedResponse);
        result2.Should().BeEquivalentTo(expectedResponse);
        _innerAdapterMock.Verify(x => x.Suggest(It.IsAny<SuggestRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Suggest_DifferentPositions_CallsInnerAdapterForEach()
    {
        // Arrange
        var request1 = CreateSuggestRequest("B:W18,19:B1,5");
        var request2 = CreateSuggestRequest("B:W20,21:B1,5");
        var response1 = CreateSuggestResponse("18-14");
        var response2 = CreateSuggestResponse("20-16");

        _innerAdapterMock
            .Setup(x => x.Suggest(It.Is<SuggestRequest>(r => r.State.Position.Contains("18,19")), It.IsAny<CancellationToken>()))
            .Returns(response1);
        _innerAdapterMock
            .Setup(x => x.Suggest(It.Is<SuggestRequest>(r => r.State.Position.Contains("20,21")), It.IsAny<CancellationToken>()))
            .Returns(response2);

        // Act
        var result1 = _cachedAdapter.Suggest(request1, CancellationToken.None);
        var result2 = _cachedAdapter.Suggest(request2, CancellationToken.None);

        // Assert
        result1.BestMove.Should().Be("18-14");
        result2.BestMove.Should().Be("20-16");
        _innerAdapterMock.Verify(x => x.Suggest(It.IsAny<SuggestRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public void Suggest_SamePositionDifferentCasing_UsesCache()
    {
        // Arrange
        var request1 = CreateSuggestRequest("B:W18,19:B1,5");
        var request2 = CreateSuggestRequest("b:w18,19:b1,5");
        var expectedResponse = CreateSuggestResponse("18-14");

        _innerAdapterMock
            .Setup(x => x.Suggest(It.IsAny<SuggestRequest>(), It.IsAny<CancellationToken>()))
            .Returns(expectedResponse);

        // Act
        var result1 = _cachedAdapter.Suggest(request1, CancellationToken.None);
        var result2 = _cachedAdapter.Suggest(request2, CancellationToken.None);

        // Assert
        result1.Should().BeEquivalentTo(expectedResponse);
        result2.Should().BeEquivalentTo(expectedResponse);
        _innerAdapterMock.Verify(x => x.Suggest(It.IsAny<SuggestRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static SuggestRequest CreateSuggestRequest(string position)
    {
        return new SuggestRequest
        {
            State = new StateDto { Position = position },
            Level = "medium"
        };
    }

    private static SuggestResponse CreateSuggestResponse(string bestMove)
    {
        return new SuggestResponse
        {
            Engine = "kingsrow",
            BestMove = bestMove,
            Depth = 12,
            Nodes = 1000,
            PositionKey = $"pdn:{bestMove}",
            Info = new SuggestInfo { TimeMs = 100 }
        };
    }
}

