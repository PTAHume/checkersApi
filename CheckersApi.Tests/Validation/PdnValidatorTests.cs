using CheckersApi.Validation;
using FluentAssertions;
using Xunit;

namespace CheckersApi.Tests.Validation;

public class PdnValidatorTests
{
    [Theory]
    [InlineData("B:W18,19,22,25,27,28,30,32:B1,5,6,7,10,12,14,16")]
    [InlineData("W:W1,2,3,4:B29,30,31,32")]
    [InlineData("B:WK1:B32")]
    [InlineData("W:W1,K2,3:B30,31,K32")]
    [InlineData("B:W1,2,3,4,5,6,7,8,9,10,11,12:B21,22,23,24,25,26,27,28,29,30,31,32")]
    public void IsValid_ValidPdn_ReturnsTrue(string pdn)
    {
        // Act
        var result = PdnValidator.IsValid(pdn);

        // Assert
        result.Should().BeTrue($"'{pdn}' should be valid");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("B:W:B")]
    [InlineData("B::B1")]
    [InlineData("X:W1:B2")]
    [InlineData("B:W1")]
    [InlineData("B:W1:")]
    [InlineData(":W1:B2")]
    [InlineData("B:W0:B1")]
    [InlineData("B:W33:B1")]
    [InlineData("B:WW1:B2")]
    public void IsValid_InvalidPdn_ReturnsFalse(string pdn)
    {
        // Act
        var result = PdnValidator.IsValid(pdn);

        // Assert
        result.Should().BeFalse($"'{pdn}' should be invalid");
    }

    [Theory]
    [InlineData("b:w18,19:b1,5")]
    [InlineData("B:W18,19:B1,5")]
    [InlineData("B : W18, 19 : B1, 5")]
    public void IsValid_HandlesVariousFormats_ReturnsTrue(string pdn)
    {
        // Act
        var result = PdnValidator.IsValid(pdn);

        // Assert
        result.Should().BeTrue($"'{pdn}' should be valid after normalization");
    }

    [Fact]
    public void IsValid_PositionWithKings_ReturnsTrue()
    {
        // Arrange
        var pdn = "B:WK1,2:BK32,31";

        // Act
        var result = PdnValidator.IsValid(pdn);

        // Assert
        result.Should().BeTrue("Position with kings should be valid");
    }

    [Fact]
    public void IsValid_MinimalPosition_ReturnsTrue()
    {
        // Arrange
        var pdn = "B:W1:B32";

        // Act
        var result = PdnValidator.IsValid(pdn);

        // Assert
        result.Should().BeTrue("Minimal valid position should be valid");
    }

    [Fact]
    public void IsValid_FullBoard_ReturnsTrue()
    {
        // Arrange
        var pdn = "B:W1,2,3,4,5,6,7,8,9,10,11,12:B21,22,23,24,25,26,27,28,29,30,31,32";

        // Act
        var result = PdnValidator.IsValid(pdn);

        // Assert
        result.Should().BeTrue("Full board position should be valid");
    }
}

