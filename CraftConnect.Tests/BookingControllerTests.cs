using BookingManagement.Application.CQRS.Commands.BookingCommands;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using BookingManagement.Application.DTOs.BookingDTOs;
using BookingManagement.Application.DTOs.BookingLineItemDTOs;
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
			var bookingResponse = new BookingResponseDTO();
			_mediatorMock.Setup(m => m.Send(It.Is<GetBookingByIdQuery>(q => q.Id == bookingId), default))
				.ReturnsAsync(bookingResponse);

			// Act
			var result = await _bookingsController.GetBookingByIdAsync(bookingId);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(bookingResponse, okResult.Value);
		}

		[Fact]
		public async Task GetBookingByIdAsync_ReturnsNotFound_WhenBookingDoesNotExist()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(It.Is<GetBookingByIdQuery>(q => q.Id == bookingId), default))
				.ReturnsAsync((BookingResponseDTO)null);

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
			var bookings = new List<BookingResponseDTO> { new(), new() };
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllBookingQuery>(), default))
				.ReturnsAsync(bookings);

			// Act
			var result = await _bookingsController.GetAllBookingAsync();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedBookings = Assert.IsAssignableFrom<IEnumerable<BookingResponseDTO>>(okResult.Value);
			Assert.Equal(2, returnedBookings.Count());
		}

		[Fact]
		public async Task GetAllBookingAsync_ReturnsNotFound_WhenNoBookingsExist()
		{
			// Arrange
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllBookingQuery>(), default))
				.ReturnsAsync(new List<BookingResponseDTO>());

			// Act
			var result = await _bookingsController.GetAllBookingAsync();

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task CreateBookingAsync_ReturnsOk_WhenBookingCreated()
		{
			// Arrange
			_bookingsController.ModelState.Clear();
			var createCommand = new CreateBookingCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<CreateBookingCommand>(), default))
				.ReturnsAsync(new BookingResponseDTO());

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
			_mediatorMock.Setup(m => m.Send(It.IsAny<CreateBookingCommand>(), default))
				.ReturnsAsync((BookingResponseDTO)null);

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
			_mediatorMock.Setup(m => m.Send(It.IsAny<DeleteBookingCommand>(), default))
				.ReturnsAsync(Unit.Value);

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
		//[Fact]
		//public async Task UpdateBookingAsync_ReturnsOk_WhenBookingUpdated()
		//{
		//	// Arrange
		//	_bookingsController.ModelState.Clear(); 
		//	var updateCommand = new UpdateBookingCommand();
		//	var response = new BookingResponseDTO
		//	{
		//		IsSuccess = true,
		//		Message = "Booking updated successfully"
		//	};
		//	_mediatorMock.Setup(m => m.Send(It.IsAny<UpdateBookingCommand>(), default))
		//		.ReturnsAsync(response);

		//	// Act
		//	var result = await _bookingsController.UpdateBookingAsync(updateCommand);

		//	// Assert
		//	var okResult = Assert.IsType<OkObjectResult>(result);
		//	Assert.Equal(response, okResult.Value);
		//}

		//[Fact]
		//public async Task UpdateBookingAsync_ReturnsNotFound_WhenResultIsNull()
		//{
		//	// Arrange
		//	var updateCommand = new UpdateBookingCommand();
		//	_mediatorMock.Setup(m => m.Send(It.IsAny<UpdateBookingCommand>(), default))
		//		.ReturnsAsync((BookingResponseDTO)null);

		//	// Act
		//	var result = await _bookingsController.UpdateBookingAsync(updateCommand);

		//	// Assert
		//	Assert.IsType<BadRequestObjectResult>(result);
		//}

		//[Fact]
		//public async Task UpdateBookingAsync_ReturnsBadRequest_WhenCommandIsNullOrIdIsEmpty()
		//{
		//	// Act
		//	var result = await _bookingsController.UpdateBookingAsync(null);

		//	// Assert
		//	Assert.IsType<BadRequestObjectResult>(result);
		//}

		[Fact]
		public async Task ConfirmBookingAsync_ReturnsOk_WhenBookingConfirmed()
		{
			// Arrange
			_bookingsController.ModelState.Clear();
			var confirmCommand = new ConfirmBookingCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<ConfirmBookingCommand>(), default))
				.ReturnsAsync(Unit.Value);

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
			_mediatorMock.Setup(m => m.Send(It.IsAny<CompleteBookingCommand>(), default))
				.ReturnsAsync(Unit.Value);

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
			var addLineItemCommand = new BookingLineItemCreateCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<BookingLineItemCreateCommand>(), default))
				.ReturnsAsync(new BookingLineItemResponseDTO());

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
			var addLineItemCommand = new BookingLineItemCreateCommand();
			_mediatorMock.Setup(m => m.Send(It.IsAny<BookingLineItemCreateCommand>(), default))
				.ReturnsAsync((BookingLineItemResponseDTO)null);

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
			var detailsQuery = "test description";
			var bookingResponse = new BookingResponseDTO();
			_mediatorMock.Setup(m => m.Send(It.Is<GetBookingByDetailsQuery>(q => q.Description == detailsQuery), default))
				.ReturnsAsync(bookingResponse); 

			// Act
			var result = await _bookingsController.GetBookingByDetailsAsync(detailsQuery);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal(bookingResponse, okResult.Value);
		}

		[Fact]
		public async Task GetBookingByDetailsAsync_ReturnsNotFound_WhenBookingDoesNotExist()
		{
			// Arrange
			var detailsQuery = "Test description";
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByDetailsQuery>(), default))
				.ReturnsAsync((BookingResponseDTO)null);

			// Act
			var result = await _bookingsController.GetBookingByDetailsAsync(detailsQuery);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}

		[Fact]
		public async Task GetBookingByDetailsAsync_ReturnsBadRequest_WhenDetailsIsEmpty()
		{
			// Arrange
			var detailsQuery = "Test description";
			_bookingsController.ModelState.AddModelError("PropertyName", "Error message");

			// Act
			var result = await _bookingsController.GetBookingByDetailsAsync(detailsQuery);

			// Assert
			Assert.IsType<NotFoundObjectResult>(result);
		}
	}
}