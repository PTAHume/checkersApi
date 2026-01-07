using CheckersApi.Contracts;
using CheckersApi.Controllers;
using CheckersApi.Engine;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CheckersApi.Tests.Controllers;

public class MoveControllerTests
{
    private readonly Mock<IEngineAdapter> _engineMock;
    private readonly Mock<ILogger<MoveController>> _loggerMock;
    private readonly MoveController _controller;

    public MoveControllerTests()
    {
        _engineMock = new Mock<IEngineAdapter>();
        _loggerMock = new Mock<ILogger<MoveController>>();
        _controller = new MoveController(_engineMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Suggest_ValidRequest_ReturnsOkWithResponse()
    {
        // Arrange
        var request = new SuggestRequest
        {
            State = new StateDto { Position = "B:W18,19:B1,5" },
            Level = "medium"
        };

        var expectedResponse = new SuggestResponse
        {
            Engine = "kingsrow",
            BestMove = "18-14",
            Depth = 12,
            Nodes = 1000,
            PositionKey = "pdn:B:W18,19:B1,5",
            Info = new SuggestInfo { TimeMs = 100 }
        };

        _engineMock
            .Setup(x => x.Suggest(It.IsAny<SuggestRequest>(), It.IsAny<CancellationToken>()))
            .Returns(expectedResponse);

        // Act
        var result = _controller.Suggest(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public void Suggest_NullRequest_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Suggest(null!);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Suggest_NullPosition_ReturnsBadRequest()
    {
        // Arrange
        var request = new SuggestRequest
        {
            State = new StateDto { Position = null! },
            Level = "medium"
        };

        // Act
        var result = _controller.Suggest(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Suggest_InvalidPdn_ReturnsUnprocessableEntity()
    {
        // Arrange
        var request = new SuggestRequest
        {
            State = new StateDto { Position = "invalid-pdn" },
            Level = "medium"
        };

        // Act
        var result = _controller.Suggest(request);

        // Assert
        result.Should().BeOfType<UnprocessableEntityObjectResult>();
    }

    [Fact]
    public void Suggest_EngineThrowsArgumentException_ReturnsUnprocessableEntity()
    {
        // Arrange
        var request = new SuggestRequest
        {
            State = new StateDto { Position = "B:W18,19:B1,5" },
            Level = "medium"
        };

        _engineMock
            .Setup(x => x.Suggest(It.IsAny<SuggestRequest>(), It.IsAny<CancellationToken>()))
            .Throws(new ArgumentException("Invalid position"));

        // Act
        var result = _controller.Suggest(request);

        // Assert
        result.Should().BeOfType<UnprocessableEntityObjectResult>();
    }

    [Fact]
    public void Suggest_EngineThrowsInvalidOperationException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new SuggestRequest
        {
            State = new StateDto { Position = "B:W18,19:B1,5" },
            Level = "medium"
        };

        _engineMock
            .Setup(x => x.Suggest(It.IsAny<SuggestRequest>(), It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Engine failed"));

        // Act
        var result = _controller.Suggest(request);

        // Assert
        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public void Validate_ValidMove_ReturnsOkWithTrue()
    {
        // Arrange
        var request = new ValidateMoveRequest
        {
            Position = "B:W18,19:B1,5",
            Move = "18-14"
        };

        // Act
        var result = _controller.Validate(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value;
        response.Should().NotBeNull();
        response!.GetType().GetProperty("legal")!.GetValue(response).Should().Be(true);
    }

    [Fact]
    public void Validate_InvalidMove_ReturnsOkWithFalse()
    {
        // Arrange
        var request = new ValidateMoveRequest
        {
            Position = "B:W18,19:B1,5",
            Move = "invalid"
        };

        // Act
        var result = _controller.Validate(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value;
        response.Should().NotBeNull();
        response!.GetType().GetProperty("legal")!.GetValue(response).Should().Be(false);
    }

    [Fact]
    public void Validate_NullRequest_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Validate(null!);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Validate_NullPosition_ReturnsBadRequest()
    {
        // Arrange
        var request = new ValidateMoveRequest
        {
            Position = null!,
            Move = "18-14"
        };

        // Act
        var result = _controller.Validate(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Validate_InvalidPosition_ReturnsUnprocessableEntity()
    {
        // Arrange
        var request = new ValidateMoveRequest
        {
            Position = "invalid-pdn",
            Move = "18-14"
        };

        // Act
        var result = _controller.Validate(request);

        // Assert
        result.Should().BeOfType<UnprocessableEntityObjectResult>();
    }
}

