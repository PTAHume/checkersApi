using CheckersApi.Engine;
using FluentAssertions;
using Xunit;

namespace CheckersApi.Tests.Engine;

public class SearchLimitsResolverTests
{
    [Fact]
    public void Resolve_WeakLevel_ReturnsCorrectLimits()
    {
        // Act
        var (depth, timeMs) = SearchLimitsResolver.Resolve("weak");

        // Assert
        depth.Should().Be(8);
        timeMs.Should().Be(100);
    }

    [Fact]
    public void Resolve_MediumLevel_ReturnsCorrectLimits()
    {
        // Act
        var (depth, timeMs) = SearchLimitsResolver.Resolve("medium");

        // Assert
        depth.Should().Be(12);
        timeMs.Should().Be(250);
    }

    [Fact]
    public void Resolve_StrongLevel_ReturnsCorrectLimits()
    {
        // Act
        var (depth, timeMs) = SearchLimitsResolver.Resolve("strong");

        // Assert
        depth.Should().Be(16);
        timeMs.Should().Be(600);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("WEAK")]
    [InlineData("unknown")]
    [InlineData(null)]
    public void Resolve_UnknownLevel_ReturnsWeakLevelDefaults(string level)
    {
        // Act
        var (depth, timeMs) = SearchLimitsResolver.Resolve(level);

        // Assert
        depth.Should().Be(8);
        timeMs.Should().Be(100);
    }

    [Fact]
    public void Resolve_AllLevels_ReturnIncreasingDifficulty()
    {
        // Act
        var weak = SearchLimitsResolver.Resolve("weak");
        var medium = SearchLimitsResolver.Resolve("medium");
        var strong = SearchLimitsResolver.Resolve("strong");

        // Assert
        weak.depth.Should().BeLessThan(medium.depth);
        medium.depth.Should().BeLessThan(strong.depth);
        weak.timeMs.Should().BeLessThan(medium.timeMs);
        medium.timeMs.Should().BeLessThan(strong.timeMs);
    }
}

