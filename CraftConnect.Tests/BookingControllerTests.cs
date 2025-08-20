using Moq;
using MediatR;
using BookingManagement.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using BookingManagement.Application.DTOs.BookingDTOs;
using BookingManagement.Application.DTOs.BookingLineItemDTOs;
using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.CQRS.Queries.BookingQueries;

namespace CraftConnect.Tests
{
	public class BookingsControllerTests
	{
		private readonly Mock<IMediator> _mediatorMock;
		private readonly BookingsController _controller;

		public BookingsControllerTests()
		{
			_mediatorMock = new Mock<IMediator>();
			_controller = new BookingsController(_mediatorMock.Object);
		}

		[Fact]
		public async Task GetAllBookingAsync_ReturnsOk_WhenBookingsExist()
		{
			// Arrange
			var bookings = new List<BookingResponseDTO> { new(), new() };
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllBookingQuery>(), default))
				.ReturnsAsync(bookings);

			// Act
			var result = await _controller.GetAllBookingAsync();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnValue = Assert.IsType<List<BookingResponseDTO>>(okResult.Value);
			Assert.Equal(2, returnValue.Count);
		}

		[Fact]
		public async Task GetAllBookingAsync_ReturnsNotFound_WhenNoBookingsExist()
		{
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllBookingQuery>(), default))
				.ReturnsAsync(new List<BookingResponseDTO>());
			var result = await _controller.GetAllBookingAsync();
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task GetBookingByIdAsync_ReturnsBadRequest_WhenIdIsEmpty()
		{
			var result = await _controller.GetBookingByIdAsync(Guid.Empty);
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task GetBookingByIdAsync_ReturnsNotFound_WhenBookingDoesNotExist()
		{
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByIdQuery>(), default))
			.ReturnsAsync((BookingResponseDTO)null);
			var result = await _controller.GetBookingByIdAsync(Guid.NewGuid());
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetBookingByIdAsync_ReturnsOk_WhenBookingExists()
		{
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByIdQuery>(), default))
				.ReturnsAsync(new BookingResponseDTO());
			var result = await _controller.GetBookingByIdAsync(Guid.NewGuid());
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task GetBookingByDetailsAsync_ReturnsBadRequest_WhenDetailsIsEmpty()
		{
			var result = await _controller.GetBookingByDetailsAsync("");
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task GetBookingByDetailsAsync_ReturnsNotFound_WhenBookingDoesNotExist()
		{
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByDetailsQuery>(), default))
				.ReturnsAsync((BookingResponseDTO)null);
			var result = await _controller.GetBookingByDetailsAsync("details");
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetBookingByDetailsAsync_ReturnsOk_WhenBookingExists()
		{
			// Arrange
			var details = "specific details";
			_mediatorMock.Setup(m => m.Send(It.Is<GetBookingByDetailsQuery>(q => q.Description == details), default))
				.ReturnsAsync(new BookingResponseDTO());

			// Act
			var result = await _controller.GetBookingByDetailsAsync(details);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task CreateBookingAsync_ReturnsBadRequest_WhenCommandIsNull()
		{
			var result = await _controller.CreateBookingAsync(null);
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task CreateBookingAsync_ReturnsBadRequest_WhenResultIsNull()
		{
			_mediatorMock.Setup(m => m.Send(It.IsAny<CreateBookingCommand>(), default))
				.ReturnsAsync((BookingResponseDTO)null);
			var result = await _controller.CreateBookingAsync(new CreateBookingCommand());
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task CreateBookingAsync_ReturnsOk_WhenBookingCreated()
		{
			_mediatorMock.Setup(m => m.Send(It.IsAny<CreateBookingCommand>(), default))
				.ReturnsAsync(new BookingResponseDTO());
			var result = await _controller.CreateBookingAsync(new CreateBookingCommand());
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task UpdateBookingAsync_ReturnsBadRequest_WhenCommandIsNullOrIdIsEmpty()
		{
			var result = await _controller.UpdateBookingAsync(null);
			Assert.IsType<BadRequestObjectResult>(result);
			var result2 = await _controller.UpdateBookingAsync(new UpdateBookingCommand { BookingId = Guid.Empty });
			Assert.IsType<BadRequestObjectResult>(result2);
		}

		[Fact]
		public async Task UpdateBookingAsync_ReturnsNotFound_WhenResultIsNull()
		{
			// No mock setup needed. The default null return of a reference type is sufficient.
			var result = await _controller.UpdateBookingAsync(new UpdateBookingCommand { BookingId = Guid.NewGuid() });
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task UpdateBookingAsync_ReturnsOk_WhenBookingUpdated()
		{
			_mediatorMock.Setup(m => m.Send(It.IsAny<UpdateBookingCommand>(), default))
				.ReturnsAsync(new BookingResponseDTO());
			var result = await _controller.UpdateBookingAsync(new UpdateBookingCommand { BookingId = Guid.NewGuid() });
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task DeleteBookingAsync_ReturnsBadRequest_WhenIdIsEmpty()
		{
			var result = await _controller.DeleteBookingAsync(Guid.Empty);
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task DeleteBookingAsync_ReturnsNoContent_WhenBookingDeleted()
		{
			var id = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(It.IsAny<DeleteBookingCommand>(), default)).ReturnsAsync(Unit.Value);
			var result = await _controller.DeleteBookingAsync(id);
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task CompleteBookingAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			_controller.ModelState.AddModelError("error", "error");
			var result = await _controller.CompleteBookingAsync(new CompleteBookingCommand());
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task CompleteBookingAsync_ReturnsOk_WhenBookingCompleted()
		{
			var command = new CompleteBookingCommand { BookingId = Guid.NewGuid() };
			var result = await _controller.CompleteBookingAsync(command);
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task ConfirmBookingAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			_controller.ModelState.AddModelError("error", "error");
			var result = await _controller.ConfirmBookingAsync(new ConfirmBookingCommand());
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task ConfirmBookingAsync_ReturnsOk_WhenBookingConfirmed()
		{
			var command = new ConfirmBookingCommand { BookingId = Guid.NewGuid() };
			var result = await _controller.ConfirmBookingAsync(command);
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task AddLineItemToBookingAsync_ReturnsBadRequest_WhenIdOrCommandIsInvalid()
		{
			var result = await _controller.AddLineItemToBookingAsync(Guid.Empty, null);
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task AddLineItemToBookingAsync_ReturnsNotFound_WhenResultIsNull()
		{
			_mediatorMock.Setup(m => m.Send(It.IsAny<BookingLineItemCreateCommand>(), default))
				.ReturnsAsync((BookingLineItemResponseDTO)null);
			var result = await _controller.AddLineItemToBookingAsync(Guid.NewGuid(), new BookingLineItemCreateCommand());
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task AddLineItemToBookingAsync_ReturnsOk_WhenLineItemAdded()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var command = new BookingLineItemCreateCommand();
			_mediatorMock.Setup(m => m.Send(It.Is<BookingLineItemCreateCommand>(c => c.BookingId == bookingId), default))
				.ReturnsAsync(new BookingLineItemResponseDTO());

			// Act
			var result = await _controller.AddLineItemToBookingAsync(bookingId, command);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}
	}
}
