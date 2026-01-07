using CheckersApi.Validation;
using FluentAssertions;
using Xunit;

namespace CheckersApi.Tests.Validation;

public class MoveValidatorTests
{
    [Theory]
    [InlineData("22-18")]
    [InlineData("01-05")]
    [InlineData("32-28")]
    [InlineData("22-18x11-7")]
    [InlineData("12x19-26")]
    [InlineData("5-9")]
    [InlineData("10-14-18-22")]
    public void IsLegalFormat_ValidMoveFormat_ReturnsTrue(string move)
    {
        // Act
        var result = MoveValidator.IsLegalFormat(move);

        // Assert
        result.Should().BeTrue($"'{move}' should be valid format");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("22")]
    [InlineData("22-")]
    [InlineData("-18")]
    [InlineData("a2-b3")]
    [InlineData("22 18")]
    [InlineData("22->18")]
    [InlineData("1-2")]
    public void IsLegalFormat_InvalidMoveFormat_ReturnsFalse(string move)
    {
        // Act
        var result = MoveValidator.IsLegalFormat(move);

        // Assert
        result.Should().BeFalse($"'{move}' should be invalid format");
    }

    [Theory]
    [InlineData("22-18")]
    [InlineData("01-05")]
    [InlineData("32-28")]
    [InlineData("22-18x11-7")]
    [InlineData("12x19-26")]
    public void IsReasonable_ValidMove_ReturnsTrue(string move)
    {
        // Act
        var result = MoveValidator.IsReasonable(move);

        // Assert
        result.Should().BeTrue($"'{move}' should be reasonable");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("33-28")] // square 33 doesn't exist
    [InlineData("00-05")] // square 0 doesn't exist
    [InlineData("22-40")] // square 40 doesn't exist
    [InlineData("invalid")]
    [InlineData("22")]
    [InlineData("a2-b3")]
    public void IsReasonable_UnreasonableMove_ReturnsFalse(string move)
    {
        // Act
        var result = MoveValidator.IsReasonable(move);

        // Assert
        result.Should().BeFalse($"'{move}' should be unreasonable");
    }

    [Fact]
    public void IsReasonable_MoveWithAllValidSquares_ReturnsTrue()
    {
        // Arrange
        var move = "12-16-19-23";

        // Act
        var result = MoveValidator.IsReasonable(move);

        // Assert
        result.Should().BeTrue("All squares are within valid range");
    }

    [Fact]
    public void IsReasonable_MoveWithOneInvalidSquare_ReturnsFalse()
    {
        // Arrange
        var move = "12-16-19-33";

        // Act
        var result = MoveValidator.IsReasonable(move);

        // Assert
        result.Should().BeFalse("Square 33 is out of range");
    }

    [Theory]
    [InlineData("22-18x11")]
    [InlineData("22x18-11")]
    [InlineData("22-18-11")]
    public void IsLegalFormat_MixedSeparators_ReturnsTrue(string move)
    {
        // Act
        var result = MoveValidator.IsLegalFormat(move);

        // Assert
        result.Should().BeTrue("Mixed separators should be accepted");
    }

    [Fact]
    public void IsReasonable_BoundarySquares_ReturnsTrue()
    {
        // Arrange
        var moveMin = "01-05";
        var moveMax = "28-32";

        // Act
        var resultMin = MoveValidator.IsReasonable(moveMin);
        var resultMax = MoveValidator.IsReasonable(moveMax);

        // Assert
        resultMin.Should().BeTrue("Square 1 is valid");
        resultMax.Should().BeTrue("Square 32 is valid");
    }
}

