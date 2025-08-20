using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using BookingManagement.Presentation.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CraftConnect.Tests
{
	public class BookingsControllerTests
	{
		private readonly Mock<IMediator> _mediatorMock;
		private readonly BookingsController _bookingsController;

		public BookingsControllerTests()
		{
			_mediatorMock = new Mock<IMediator>();
			_bookingsController = new BookingsController(_mediatorMock.Object);
		}

		[Fact]
		public async Task GetBookingByIdAsync_ReturnsOk_WhenBookingExists()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var bookingQuery = new GetBookingByIdQuery();
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByIdQuery>(), default))
				.ReturnsAsync(bookingQuery);

			// Act
			var result = await _bookingsController.GetBookingByIdAsync(bookingId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(bookingQuery, okResult.Value);
		}

		[Fact]
		public async Task GetBookingByIdAsync_ReturnsNotFound_WhenBookingDoesNotExist()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByIdQuery>(), default)).ReturnsAsync((GetBookingByIdQuery)null);

			// Act
			var result = await _bookingsController.GetBookingByIdAsync(bookingId);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetBookingByIdAsync_ReturnsBadRequest_WhenIdIsEmpty()
		{
			// Arrange
			var bookingId = Guid.Empty;

			// Act
			var result = await _bookingsController.GetBookingByIdAsync(bookingId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task GetAllBookingAsync_ReturnsOk_WhenBookingsExist()
		{
			// Arrange
			var bookings = new List<GetAllBookingsQuery> { new GetAllBookingsQuery(), new GetAllBookingsQuery() };
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllBookingsQuery>(), default)).ReturnsAsync(bookings);

			// Act
			var result = await _bookingsController.GetAllBookingAsync();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedBookings = Assert.IsAssignableFrom<IEnumerable<GetAllBookingsQuery>>(okResult.Value);
			Assert.Equal(2, returnedBookings.Count());
		}

		[Fact]
		public async Task GetAllBookingAsync_ReturnsNotFound_WhenNoBookingsExist()
		{
			// Arrange
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllBookingsQuery>(), default)).ReturnsAsync(new List<GetAllBookingsQuery>());

			// Act
			var result = await _bookingsController.GetAllBookingAsync();

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task CreateBookingAsync_ReturnsOk_WhenBookingCreated()
		{
			// Arrange
			_bookingsController.ModelState.Clear();
			var createCommand = new CreateBookingCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<CreateBookingCommand>(), default)).ReturnsAsync(true);

			// Act
			var result = await _bookingsController.CreateBookingAsync(createCommand);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task CreateBookingAsync_ReturnsBadRequest_WhenCommandIsNull()
		{
			// Act
			var result = await _bookingsController.CreateBookingAsync(null);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task CreateBookingAsync_ReturnsBadRequest_WhenResultIsNull()
		{
			// Arrange
			_bookingsController.ModelState.Clear();
			var createCommand = new CreateBookingCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<CreateBookingCommand>(), default)).ReturnsAsync(false);

			// Act
			var result = await _bookingsController.CreateBookingAsync(createCommand);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task DeleteBookingAsync_ReturnsNoContent_WhenBookingDeleted()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(It.IsAny<DeleteBookingCommand>(), default)).ReturnsAsync(true);

			// Act
			var result = await _bookingsController.DeleteBookingAsync(bookingId);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task DeleteBookingAsync_ReturnsBadRequest_WhenIdIsEmpty()
		{
			// Arrange
			var bookingId = Guid.Empty;

			// Act
			var result = await _bookingsController.DeleteBookingAsync(bookingId);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task UpdateBookingAsync_ReturnsOk_WhenBookingUpdated()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var updateCommand = new UpdateBookingCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<UpdateBookingCommand>(), default)).ReturnsAsync(true);

			// Act
			var result = await _bookingsController.UpdateBookingAsync(bookingId, updateCommand);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task UpdateBookingAsync_ReturnsNotFound_WhenResultIsNull()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var updateCommand = new UpdateBookingCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<UpdateBookingCommand>(), default)).ReturnsAsync(false);

			// Act
			var result = await _bookingsController.UpdateBookingAsync(bookingId, updateCommand);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task UpdateBookingAsync_ReturnsBadRequest_WhenCommandIsNullOrIdIsEmpty()
		{
			// Arrange
			var bookingId = Guid.Empty;

			// Act
			var result = await _bookingsController.UpdateBookingAsync(bookingId, null);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task ConfirmBookingAsync_ReturnsOk_WhenBookingConfirmed()
		{
			// Arrange
			_bookingsController.ModelState.Clear();
			var confirmCommand = new ConfirmBookingCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<ConfirmBookingCommand>(), default)).ReturnsAsync(true);

			// Act
			var result = await _bookingsController.ConfirmBookingAsync(confirmCommand);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task ConfirmBookingAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_bookingsController.ModelState.AddModelError("BookingId", "BookingId is required");

			// Act
			var result = await _bookingsController.ConfirmBookingAsync(new ConfirmBookingCommand());

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task CompleteBookingAsync_ReturnsOk_WhenBookingCompleted()
		{
			// Arrange
			_bookingsController.ModelState.Clear();
			var completeCommand = new CompleteBookingCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<CompleteBookingCommand>(), default)).ReturnsAsync(true);

			// Act
			var result = await _bookingsController.CompleteBookingAsync(completeCommand);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task CompleteBookingAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			_bookingsController.ModelState.AddModelError("BookingId", "BookingId is required");

			// Act
			var result = await _bookingsController.CompleteBookingAsync(new CompleteBookingCommand());

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task AddLineItemToBookingAsync_ReturnsOk_WhenLineItemAdded()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var addLineItemCommand = new AddLineItemToBookingCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<AddLineItemToBookingCommand>(), default)).ReturnsAsync(true);

			// Act
			var result = await _bookingsController.AddLineItemToBookingAsync(bookingId, addLineItemCommand);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task AddLineItemToBookingAsync_ReturnsNotFound_WhenResultIsNull()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var addLineItemCommand = new AddLineItemToBookingCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<AddLineItemToBookingCommand>(), default)).ReturnsAsync(false);

			// Act
			var result = await _bookingsController.AddLineItemToBookingAsync(bookingId, addLineItemCommand);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task AddLineItemToBookingAsync_ReturnsBadRequest_WhenIdOrCommandIsInvalid()
		{
			// Arrange
			var bookingId = Guid.Empty;

			// Act
			var result = await _bookingsController.AddLineItemToBookingAsync(bookingId, null);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task GetBookingByDetailsAsync_ReturnsOk_WhenBookingExists()
		{
			// Arrange
			var detailsQuery = new GetBookingByDetailsQuery();
			var booking = new GetBookingByDetailsQuery();
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByDetailsQuery>(), default)).ReturnsAsync(booking);

			// Act
			var result = await _bookingsController.GetBookingByDetailsAsync(detailsQuery);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(booking, okResult.Value);
		}

		[Fact]
		public async Task GetBookingByDetailsAsync_ReturnsNotFound_WhenBookingDoesNotExist()
		{
			// Arrange
			var detailsQuery = new GetBookingByDetailsQuery();
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByDetailsQuery>(), default)).ReturnsAsync((GetBookingByDetailsQuery)null);

			// Act
			var result = await _bookingsController.GetBookingByDetailsAsync(detailsQuery);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetBookingByDetailsAsync_ReturnsBadRequest_WhenDetailsIsEmpty()
		{
			// Arrange
			var detailsQuery = new GetBookingByDetailsQuery();
			_bookingsController.ModelState.AddModelError("PropertyName", "Error message");

			// Act
			var result = await _bookingsController.GetBookingByDetailsAsync(detailsQuery);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}
	}
}