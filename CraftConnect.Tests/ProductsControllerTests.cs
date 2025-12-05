using Core.SharedKernel.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductInventoryManagement.Application.CQRS.Commands.ProductCommands;
using ProductInventoryManagement.Application.CQRS.Queries.ProductQueries;
using ProductInventoryManagement.Presentation.Controllers;

namespace CraftConnect.Tests
{
	public class ProductsControllerTests
	{
		private readonly Mock<IMediator> _mediatorMock;
		private readonly ProductsController _controller;
		public ProductsControllerTests()
		{
			_mediatorMock = new Mock<IMediator>();
			_controller = new ProductsController(_mediatorMock.Object);
		}

		[Fact]
		public async Task GetAllProducts_ReturnsOkResult_WithProducts()
		{
			// Arrange
			var products = new List<ProductResponseDTO> { new(), new() };
			_mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
						 .ReturnsAsync(products);

			// Act
			var result = await _controller.GetAllProducts(CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnValue = Assert.IsAssignableFrom<IEnumerable<ProductResponseDTO>>(okResult.Value);
			Assert.Equal(2, returnValue.Count());
		}

		[Fact]
		public async Task GetProductsByCategoryAsync_ReturnsOkResult_WithProducts()
		{
			// Arrange
			var category = "Electronics";
			var products = new List<ProductResponseDTO> { new(), new(), new() };
			_mediatorMock.Setup(m => m.Send(It.Is<GetProductByCategoryQuery>(q => q.Category == category), It.IsAny<CancellationToken>()))
						 .ReturnsAsync(products);

			// Act
			var result = await _controller.GetProductsByCategoryAsync(category, CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnValue = Assert.IsAssignableFrom<IEnumerable<ProductResponseDTO>>(okResult.Value);
			Assert.Equal(3, returnValue.Count());
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public async Task GetProductsByCategoryAsync_ReturnsBadRequest_WhenCategoryIsInvalid(string category)
		{
			// Act
			var result = await _controller.GetProductsByCategoryAsync(category, CancellationToken.None);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Category parameter is required.", badRequestResult.Value);
		}

		[Fact]
		public async Task GetProductById_ReturnsOkResult_WhenProductExists()
		{
			// Arrange
			var productId = Guid.NewGuid();
			var productResponse = new ProductResponseDTO { ProductId = productId, IsSuccess = true };
			_mediatorMock.Setup(m => m.Send(It.Is<GetProductByIdQuery>(q => q.ProductId == productId), It.IsAny<CancellationToken>()))
						 .ReturnsAsync(productResponse);

			// Act
			var result = await _controller.GetProductById(productId, CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnValue = Assert.IsType<ProductResponseDTO>(okResult.Value);
			Assert.Equal(productId, returnValue.ProductId);
		}

		[Fact]
		public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
		{
			// Arrange
			var productId = Guid.NewGuid();
			var productResponse = new ProductResponseDTO { IsSuccess = false, Message = "Product not found" };
			_mediatorMock.Setup(m => m.Send(It.Is<GetProductByIdQuery>(q => q.ProductId == productId), It.IsAny<CancellationToken>()))
						 .ReturnsAsync(productResponse);

			// Act
			var result = await _controller.GetProductById(productId, CancellationToken.None);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			var returnValue = Assert.IsType<ProductResponseDTO>(notFoundResult.Value);
			Assert.False(returnValue.IsSuccess);
		}

		[Fact]
		public async Task GetProductById_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			var productId = Guid.Empty; // This will likely make the model state invalid if Guid validation is enabled
			_controller.ModelState.AddModelError("ProductId", "Invalid GUID value."); // Simulate model state error

			// Act
			var result = await _controller.GetProductById(productId, CancellationToken.None);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.IsType<SerializableError>(badRequestResult.Value);
		}

		[Fact]
		public async Task CreateProductAsync_ReturnsCreatedAtActionResult_WhenProductIsCreated()
		{
			// Arrange
			var command = new CreateProductCommand();
			var productResponse = new ProductResponseDTO { ProductId = Guid.NewGuid(), IsSuccess = true, Message = "Product created successfully" };
			_mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
						 .ReturnsAsync(productResponse);

			// Act
			var result = await _controller.CreateProductAsync(command, CancellationToken.None);

			// Assert
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
			Assert.Equal(nameof(ProductsController.GetProductById), createdAtActionResult.ActionName);
			Assert.Equal(productResponse.ProductId, createdAtActionResult.RouteValues["id"]);
			var returnValue = Assert.IsType<ProductResponseDTO>(createdAtActionResult.Value);
			Assert.Equal(productResponse.ProductId, returnValue.ProductId);
		}

		// Add tests for validation errors or failures within the handler if needed,
		// depending on how the CreateProductCommandHandler behaves.

		[Fact]
		public async Task UpdateProduct_ReturnsOkResult_WhenProductIsUpdated()
		{
			// Arrange
			var productId = Guid.NewGuid();
			var command = new UpdateProductCommand { ProductDTO = new ProductUpdateDTO { ProductId = productId } };
			var productResponse = new ProductResponseDTO { ProductId = productId, IsSuccess = true, Message = "Product updated successfully" };
			_mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
						 .ReturnsAsync(productResponse);

			// Act
			var result = await _controller.UpdateProduct(productId, command, CancellationToken.None);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnValue = Assert.IsType<ProductResponseDTO>(okResult.Value);
			Assert.Equal(productId, returnValue.ProductId);
		}

		[Fact]
		public async Task UpdateProduct_ReturnsBadRequest_WhenIdsDoNotMatch()
		{
			// Arrange
			var urlId = Guid.NewGuid();
			var bodyId = Guid.NewGuid();
			var command = new UpdateProductCommand { ProductDTO = new ProductUpdateDTO { ProductId = bodyId } };

			// Act
			var result = await _controller.UpdateProduct(urlId, command, CancellationToken.None);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("ID in URL does not match ID in body.", badRequestResult.Value);
		}

		[Fact]
		public async Task UpdateProduct_ReturnsBadRequest_WhenModelStateIsInvalid()
		{
			// Arrange
			var productId = Guid.NewGuid();
			var command = new UpdateProductCommand { ProductDTO = new ProductUpdateDTO { ProductId = productId } };
			_controller.ModelState.AddModelError("ProductDTO.Name", "Name is required"); // Simulate model state error

			// Act
			var result = await _controller.UpdateProduct(productId, command, CancellationToken.None);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.IsType<SerializableError>(badRequestResult.Value);
		}

		[Fact]
		public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
		{
			// Arrange
			var productId = Guid.NewGuid();
			var command = new UpdateProductCommand { ProductDTO = new ProductUpdateDTO { ProductId = productId } };
			var productResponse = new ProductResponseDTO { ProductId = productId, IsSuccess = false, Message = "Product not found" };
			_mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
						 .ReturnsAsync(productResponse);

			// Act
			var result = await _controller.UpdateProduct(productId, command, CancellationToken.None);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			var returnValue = Assert.IsType<ProductResponseDTO>(notFoundResult.Value);
			Assert.False(returnValue.IsSuccess);
		}

		[Fact]
		public async Task DeleteProductAsync_ReturnsNoContent_WhenProductIsDeleted()
		{
			// Arrange
			var productId = Guid.NewGuid();
			_mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProductCommand>, default))
				.ReturnsAsync(Unit.Value); // Mediator Send for commands typically returns Unit

			// Act
			var result = await _controller.DeleteProductAsync(productId, CancellationToken.None);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task DeleteProductAsync_ReturnsBadRequest_WhenIdIsEmpty()
		{
			// Arrange
			var productId = Guid.Empty;

			// Act
			var result = await _controller.DeleteProductAsync(productId, CancellationToken.None);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid product ID.", badRequestResult.Value);
		}

		// Consider tests for scenarios where the delete command itself might fail
		// (e.g., if the handler throws an exception for "not found" instead of
		// returning successfully). This depends on your handler's implementation.

	}
}
