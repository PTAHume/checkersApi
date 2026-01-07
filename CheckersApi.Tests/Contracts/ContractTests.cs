using CheckersApi.Contracts;
using FluentAssertions;
using Xunit;

namespace CheckersApi.Tests.Contracts;

public class ContractTests
{
    [Fact]
    public void SuggestRequest_DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var request = new SuggestRequest();

        // Assert
        request.GameId.Should().Be("checkers-8x8");
        request.Level.Should().Be("weak");
    }

    [Fact]
    public void SuggestRequest_CanSetAllProperties()
    {
        // Arrange & Act
        var request = new SuggestRequest
        {
            GameId = "test-game",
            State = new StateDto
            {
                Notation = "FEN",
                Position = "B:W1,2:B31,32"
            },
            Level = "strong",
            Limits = new LimitsDto
            {
                MaxDepth = 16,
                SoftTimeMs = 500,
                HardTimeMs = 1000
            }
        };

        // Assert
        request.GameId.Should().Be("test-game");
        request.State.Position.Should().Be("B:W1,2:B31,32");
        request.Level.Should().Be("strong");
        request.Limits.MaxDepth.Should().Be(16);
    }

    [Fact]
    public void SuggestResponse_DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var response = new SuggestResponse();

        // Assert
        response.Engine.Should().Be("kingsrow");
        response.Pv.Should().BeEmpty();
        response.Info.Should().NotBeNull();
    }

    [Fact]
    public void SuggestInfo_CanSetAllProperties()
    {
        // Arrange & Act
        var info = new SuggestInfo
        {
            TablebaseHit = true,
            TimeMs = 250,
            Evaluation = 100
        };

        // Assert
        info.TablebaseHit.Should().BeTrue();
        info.TimeMs.Should().Be(250);
        info.Evaluation.Should().Be(100);
    }

    [Fact]
    public void StateDto_DefaultNotation_IsPDN()
    {
        // Arrange & Act
        var state = new StateDto();

        // Assert
        state.Notation.Should().Be("PDN");
    }

    [Fact]
    public void LimitsDto_AllPropertiesNullable()
    {
        // Arrange & Act
        var limits = new LimitsDto();

        // Assert
        limits.MaxDepth.Should().BeNull();
        limits.SoftTimeMs.Should().BeNull();
        limits.HardTimeMs.Should().BeNull();
    }

    [Fact]
    public void ValidateMoveRequest_CanSetAllProperties()
    {
        // Arrange & Act
        var request = new ValidateMoveRequest
        {
            Position = "B:W18,19:B1,5",
            Move = "18-14"
        };

        // Assert
        request.Position.Should().Be("B:W18,19:B1,5");
        request.Move.Should().Be("18-14");
    }
}

