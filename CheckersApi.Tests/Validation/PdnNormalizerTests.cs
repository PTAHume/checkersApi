using CheckersApi.Validation;
using FluentAssertions;
using Xunit;

namespace CheckersApi.Tests.Validation;

public class PdnNormalizerTests
{
    [Theory]
    [InlineData("b:w1,2:b31,32", "B:W1,2:B31,32")]
    [InlineData("B:W1,2:B31,32", "B:W1,2:B31,32")]
    [InlineData("B : W1, 2 : B31, 32", "B:W1,2:B31,32")]
    [InlineData("  B:W1,2:B31,32  ", "B:W1,2:B31,32")]
    [InlineData("b : w 1 , 2 : b 31 , 32", "B:W1,2:B31,32")]
    public void Normalize_VariousInputs_ReturnsNormalizedForm(string input, string expected)
    {
        // Act
        var result = PdnNormalizer.Normalize(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Normalize_WithWhitespace_RemovesAllWhitespace()
    {
        // Arrange
        var input = "B : W 1 8, 1 9 : B 1, 5";

        // Act
        var result = PdnNormalizer.Normalize(input);

        // Assert
        result.Should().NotContain(" ");
        result.Should().Be("B:W18,19:B1,5");
    }

    [Fact]
    public void Normalize_Lowercase_ConvertsToUppercase()
    {
        // Arrange
        var input = "b:wk1,2:bk31,32";

        // Act
        var result = PdnNormalizer.Normalize(input);

        // Assert
        result.Should().Be("B:WK1,2:BK31,32");
    }

    [Theory]
    [InlineData("B:W1,2:B31,32", "pdn:B:W1,2:B31,32")]
    [InlineData("b:w1,2:b31,32", "pdn:B:W1,2:B31,32")]
    [InlineData("B : W1, 2 : B31, 32", "pdn:B:W1,2:B31,32")]
    public void ToPositionKey_VariousInputs_ReturnsConsistentKey(string input, string expected)
    {
        // Act
        var result = PdnNormalizer.ToPositionKey(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToPositionKey_SamePositionDifferentFormat_ReturnsSameKey()
    {
        // Arrange
        var pdn1 = "B:W18,19:B1,5";
        var pdn2 = "b : w 1 8, 1 9 : b 1, 5";
        var pdn3 = "  B:W18,19:B1,5  ";

        // Act
        var key1 = PdnNormalizer.ToPositionKey(pdn1);
        var key2 = PdnNormalizer.ToPositionKey(pdn2);
        var key3 = PdnNormalizer.ToPositionKey(pdn3);

        // Assert
        key1.Should().Be(key2);
        key2.Should().Be(key3);
    }
}

