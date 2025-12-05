using Core.SharedKernel.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands;
using ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries;
using ProductInventoryManagement.Presentation.Controllers;

namespace CraftConnect.Tests
{
	public class CategoriesControllerTests
	{
		private readonly Mock<IMediator> _mediatorMock;
		private readonly CategoriesController _controller;
		public CategoriesControllerTests()
		{
			_mediatorMock = new Mock<IMediator>();
			_controller = new CategoriesController(_mediatorMock.Object);
		}

		[Fact]
		public async Task GetAllCategoriesAsync_ReturnsOkResult_WithCategories()
		{
			// Arrange
			var categories = new List<CategoryResponseDTO> { new CategoryResponseDTO(), new CategoryResponseDTO() };
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllCategoriesQuery>(), It.IsAny<CancellationToken>()))
						 .ReturnsAsync(categories);

			// Act
			var result = await _controller.GetAllCategoriesAsync(CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnValue = Assert.IsAssignableFrom<IEnumerable<CategoryResponseDTO>>(okResult.Value);
			Assert.Equal(2, returnValue.Count());
		}

		[Fact]
		public async Task GetCategoryByIdAsync_ReturnsOkResult_WhenCategoryExists()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			var categoryResponse = new CategoryResponseDTO { CategoryId = categoryId };
			_mediatorMock.Setup(m => m.Send(It.Is<GetCategoryByIdQuery>(q => q.Id == categoryId), It.IsAny<CancellationToken>()))
						 .ReturnsAsync(categoryResponse);

			// Act
			var result = await _controller.GetCategoryByIdAsync(categoryId, CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnValue = Assert.IsType<CategoryResponseDTO>(okResult.Value);
			Assert.Equal(categoryId, returnValue.CategoryId);
		}

		[Fact]
		public async Task GetCategoryByIdAsync_ReturnsNotFound_WhenCategoryDoesNotExist()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(It.Is<GetCategoryByIdQuery>(q => q.Id == categoryId), It.IsAny<CancellationToken>()))
						 .ReturnsAsync((CategoryResponseDTO)null);

			// Act
			var result = await _controller.GetCategoryByIdAsync(categoryId, CancellationToken.None);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task CreateCategoryAsync_ReturnsOk_WhenCategoryIsCreated()
		{
			// Arrange
			var command = new CategoryCreateCommand();
			var categoryResponse = new CategoryResponseDTO { CategoryId = Guid.NewGuid() };
			_mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
						 .ReturnsAsync(categoryResponse);

			// Act
			var result = await _controller.CreateCategoryAsync(command, CancellationToken.None);

			// Assert
			var createdAtActionResult = Assert.IsType<OkObjectResult>(result);
			Assert.IsAssignableFrom<CategoryResponseDTO>(createdAtActionResult.Value);
		}

		[Fact]
		public async Task CreateCategoryAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			var command = new CategoryCreateCommand();
			_controller.ModelState.AddModelError("Name", "Name is required"); // Simulate model state error

			// Act
			var result = await _controller.CreateCategoryAsync(command, CancellationToken.None);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.IsType<SerializableError>(badRequestResult.Value);
		}

		// Consider tests for scenarios where the create command handler might return null
		// or throw exceptions, if that's possible in your implementation.

		[Fact]
		public async Task UpdateCategoryAsync_ReturnsNoContent_WhenCategoryIsUpdated()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			var command = new UpdateCategoryCommand { CategoryUpdateDTO = new CategoryUpdateDTO { CategoryId = categoryId } };
			var categoryResponse = new CategoryResponseDTO { CategoryId = categoryId, IsSuccess = true };
			_mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
						 .ReturnsAsync(categoryResponse);

			// Act
			var result = await _controller.UpdateCategoryAsync(categoryId, command, CancellationToken.None);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task UpdateCategoryAsync_ReturnsBadRequest_WhenIdsDoNotMatch()
		{
			// Arrange
			var urlId = Guid.NewGuid();
			var bodyId = Guid.NewGuid();
			var command = new UpdateCategoryCommand { CategoryUpdateDTO = new CategoryUpdateDTO { CategoryId = bodyId } };

			// Act
			var result = await _controller.UpdateCategoryAsync(urlId, command, CancellationToken.None);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("ID in URL does not match ID in body.", badRequestResult.Value);
		}

		[Fact]
		public async Task UpdateCategoryAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			var command = new UpdateCategoryCommand { CategoryUpdateDTO = new CategoryUpdateDTO { CategoryId = categoryId } };
			_controller.ModelState.AddModelError("CategoryUpdateDTO.Name", "Name is required"); // Simulate model state error

			// Act
			var result = await _controller.UpdateCategoryAsync(categoryId, command, CancellationToken.None);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.IsType<SerializableError>(badRequestResult.Value);
		}

		[Fact]
		public async Task UpdateCategoryAsync_ReturnsNotFound_WhenCategoryDoesNotExist()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			var command = new UpdateCategoryCommand { CategoryUpdateDTO = new CategoryUpdateDTO { CategoryId = categoryId } };
			var categoryResponse = new CategoryResponseDTO { CategoryId = categoryId, IsSuccess = false }; // Indicate failure/not found
			_mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
						 .ReturnsAsync(categoryResponse);

			// Act
			var result = await _controller.UpdateCategoryAsync(categoryId, command, CancellationToken.None);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task DeleteCategoryAsync_ReturnsOk_WhenCategoryIsDeleted()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCategoryCommand>, It.IsAny<CancellationToken>()))
						 .ReturnsAsync(Unit.Value); // Mediator Send for commands typically returns Unit

			// Act
			var result = await _controller.DeleteCategoryAsync(categoryId, CancellationToken.None);

			// Assert
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task DeleteCategoryAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			var categoryId = Guid.Empty; // Example that might cause model state error
			_controller.ModelState.AddModelError("id", "Invalid ID."); // Simulate model state error

			// Act
			var result = await _controller.DeleteCategoryAsync(categoryId, CancellationToken.None);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.IsType<SerializableError>(badRequestResult.Value);
		}

		// Consider tests for scenarios where the delete command handler might indicate
		// failure (e.g., by returning a specific DTO or throwing) if that's how it handles "not found".
	}
}
