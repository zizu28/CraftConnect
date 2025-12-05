using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;
using PaymentManagement.Application.CQRS.Queries.InvoiceQueries;
using PaymentManagement.Presentation.Controllers;
using Core.SharedKernel.ValueObjects;
using Core.SharedKernel.DTOs;

namespace CraftConnect.Tests
{
    public class InvoicesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly InvoicesController _controller;

        public InvoicesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new InvoicesController(_mediatorMock.Object);
        }

        #region GetAllInvoicesAsync Tests

        [Fact]
        public async Task GetAllInvoicesAsync_ReturnsOkResult_WithInvoices()
        {
            // Arrange
            var invoices = new List<InvoiceResponseDTO>
            {
                new InvoiceResponseDTO { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", TotalAmount = 100.00m },
                new InvoiceResponseDTO { Id = Guid.NewGuid(), InvoiceNumber = "INV-002", TotalAmount = 250.50m }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllInvoicesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(invoices);

            // Act
            var result = await _controller.GetAllInvoicesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<InvoiceResponseDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetAllInvoicesAsync_ReturnsOkResult_WithEmptyList_WhenNoInvoices()
        {
            // Arrange
            var emptyInvoices = new List<InvoiceResponseDTO>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllInvoicesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(emptyInvoices);

            // Act
            var result = await _controller.GetAllInvoicesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<InvoiceResponseDTO>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetAllInvoicesAsync_CallsMediatorWithCorrectQuery()
        {
            // Arrange
            var invoices = new List<InvoiceResponseDTO>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllInvoicesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(invoices);

            // Act
            await _controller.GetAllInvoicesAsync();

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllInvoicesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetInvoiceByIdAsync Tests

        [Fact]
        public async Task GetInvoiceByIdAsync_ReturnsOkResult_WhenInvoiceExists()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var invoiceResponse = new InvoiceResponseDTO 
            { 
                Id = invoiceId, 
                InvoiceNumber = "INV-001", 
                TotalAmount = 100.00m,
                Currency = "USD"
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetInvoiceByIdQuery>(q => q.InvoiceId == invoiceId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(invoiceResponse);

            // Act
            var result = await _controller.GetInvoiceByIdAsync(invoiceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<InvoiceResponseDTO>(okResult.Value);
            Assert.Equal(invoiceId, returnValue.Id);
            Assert.Equal("INV-001", returnValue.InvoiceNumber);
        }

        [Fact]
        public async Task GetInvoiceByIdAsync_ReturnsNotFound_WhenInvoiceDoesNotExist()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<GetInvoiceByIdQuery>(q => q.InvoiceId == invoiceId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((InvoiceResponseDTO)null);

            // Act
            var result = await _controller.GetInvoiceByIdAsync(invoiceId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetInvoiceByIdAsync_CallsMediatorWithCorrectQuery()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var invoiceResponse = new InvoiceResponseDTO { Id = invoiceId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetInvoiceByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(invoiceResponse);

            // Act
            await _controller.GetInvoiceByIdAsync(invoiceId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<GetInvoiceByIdQuery>(q => q.InvoiceId == invoiceId), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetDaysUntilDueAsync Tests

        [Fact]
        public async Task GetDaysUntilDueAsync_ReturnsOkResult_WhenInvoiceIdIsValid()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var daysUntilDue = 15;
            _mediatorMock.Setup(m => m.Send(It.Is<GetDaysUntilDueQuery>(q => q.InvoiceId == invoiceId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(daysUntilDue);

            // Act
            var result = await _controller.GetDaysUntilDueAsync(invoiceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(daysUntilDue, okResult.Value);
        }

        [Fact]
        public async Task GetDaysUntilDueAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;

            // Act
            var result = await _controller.GetDaysUntilDueAsync(invoiceId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid invoice ID.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetDaysUntilDueAsync_DoesNotCallMediator_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;

            // Act
            await _controller.GetDaysUntilDueAsync(invoiceId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetDaysUntilDueQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region GetOutstandingAmountAsync Tests

        [Fact]
        public async Task GetOutstandingAmountAsync_ReturnsOkResult_WhenInvoiceIdIsValid()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var outstandingAmount = new Money(75.50m, "USD");
            _mediatorMock.Setup(m => m.Send(It.Is<GetOutstandingAmountQuery>(q => q.InvoiceId == invoiceId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(outstandingAmount);

            // Act
            var result = await _controller.GetOutstandingAmountAsync(invoiceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(outstandingAmount, okResult.Value);
        }

        [Fact]
        public async Task GetOutstandingAmountAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;

            // Act
            var result = await _controller.GetOutstandingAmountAsync(invoiceId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid invoice ID.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetOutstandingAmountAsync_DoesNotCallMediator_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;

            // Act
            await _controller.GetOutstandingAmountAsync(invoiceId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetOutstandingAmountQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region CreateInvoiceAsync Tests

        [Fact]
        public async Task CreateInvoiceAsync_ReturnsOkResult_WhenInvoiceIsCreated()
        {
            // Arrange
            var command = new CreateInvoiceCommand 
            { 
                InvoiceDTO = new InvoiceCreateDTO 
                { 
                    InvoiceType = "Booking",
                    IssuedTo = Guid.NewGuid(), 
                    IssuedBy = Guid.NewGuid(),
                    Currency = "USD",
                    RecipientName = "John Doe",
                    RecipientEmail = "john@example.com",
                    BillingStreet = "123 Main St",
                    BillingCity = "New York",
                    BillingPostalCode = "10001"
                } 
            };
            var invoiceResponse = new InvoiceResponseDTO { Id = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(invoiceResponse);

            // Act
            var result = await _controller.CreateInvoiceAsync(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<InvoiceResponseDTO>(okResult.Value);
        }

        [Fact]
        public async Task CreateInvoiceAsync_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            CreateInvoiceCommand command = null;

            // Act
            var result = await _controller.CreateInvoiceAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invoice data is null.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateInvoiceAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var command = new CreateInvoiceCommand();
            _controller.ModelState.AddModelError("IssuedTo", "IssuedTo is required");

            // Act
            var result = await _controller.CreateInvoiceAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateInvoiceAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var command = new CreateInvoiceCommand 
            { 
                InvoiceDTO = new InvoiceCreateDTO 
                { 
                    InvoiceType = "Order",
                    IssuedTo = Guid.NewGuid(),
                    IssuedBy = Guid.NewGuid(),
                    Currency = "USD",
                    RecipientName = "Jane Doe",
                    RecipientEmail = "jane@example.com",
                    BillingStreet = "456 Oak Ave",
                    BillingCity = "Los Angeles",
                    BillingPostalCode = "90210"
                } 
            };
            var invoiceResponse = new InvoiceResponseDTO();
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(invoiceResponse);

            // Act
            await _controller.CreateInvoiceAsync(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region MarkInvoiceAsPaidAsync Tests

        [Fact]
        public async Task MarkInvoiceAsPaidAsync_ReturnsNoContent_WhenInvoiceIsMarkedAsPaid()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkAsPaidCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.MarkInvoiceAsPaidAsync(invoiceId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task MarkInvoiceAsPaidAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;

            // Act
            var result = await _controller.MarkInvoiceAsPaidAsync(invoiceId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid invoice ID.", badRequestResult.Value);
        }

        [Fact]
        public async Task MarkInvoiceAsPaidAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkAsPaidCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.MarkInvoiceAsPaidAsync(invoiceId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<MarkAsPaidCommand>(c => c.InvoiceId == invoiceId), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region MarkAsOverdueAsync Tests

        [Fact]
        public async Task MarkAsOverdueAsync_ReturnsNoContent_WhenInvoiceIsMarkedAsOverdue()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var recipientEmail = "recipient@example.com";
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkAsOverDueCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.MarkAsOverdueAsync(invoiceId, recipientEmail);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task MarkAsOverdueAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;
            var recipientEmail = "recipient@example.com";

            // Act
            var result = await _controller.MarkAsOverdueAsync(invoiceId, recipientEmail);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid invoice ID or recipient email.", badRequestResult.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task MarkAsOverdueAsync_ReturnsBadRequest_WhenRecipientEmailIsInvalid(string recipientEmail)
        {
            // Arrange
            var invoiceId = Guid.NewGuid();

            // Act
            var result = await _controller.MarkAsOverdueAsync(invoiceId, recipientEmail);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid invoice ID or recipient email.", badRequestResult.Value);
        }

        [Fact]
        public async Task MarkAsOverdueAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var recipientEmail = "recipient@example.com";
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkAsOverDueCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.MarkAsOverdueAsync(invoiceId, recipientEmail);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<MarkAsOverDueCommand>(c => 
                c.InvoiceId == invoiceId && c.RecipientEmail == recipientEmail), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region ApplyDiscountAsync Tests

        [Fact]
        public async Task ApplyDiscountAsync_ReturnsNoContent_WhenDiscountIsApplied()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var discountAmount = 10.00m;
            var currency = "USD";
            _mediatorMock.Setup(m => m.Send(It.IsAny<ApplyDiscountCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ApplyDiscountAsync(invoiceId, discountAmount, currency);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ApplyDiscountAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;
            var discountAmount = 10.00m;
            var currency = "USD";

            // Act
            var result = await _controller.ApplyDiscountAsync(invoiceId, discountAmount, currency);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task ApplyDiscountAsync_ReturnsBadRequest_WhenDiscountAmountIsZeroOrNegative()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var discountAmount = 0m;
            var currency = "USD";

            // Act
            var result = await _controller.ApplyDiscountAsync(invoiceId, discountAmount, currency);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task ApplyDiscountAsync_ReturnsBadRequest_WhenCurrencyIsInvalid(string currency)
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var discountAmount = 10.00m;

            // Act
            var result = await _controller.ApplyDiscountAsync(invoiceId, discountAmount, currency);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task ApplyDiscountAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var discountAmount = 15.00m;
            var currency = "EUR";
            _mediatorMock.Setup(m => m.Send(It.IsAny<ApplyDiscountCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.ApplyDiscountAsync(invoiceId, discountAmount, currency);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<ApplyDiscountCommand>(c => 
                c.InvoiceId == invoiceId && 
                c.DiscountAmount == discountAmount && 
                c.Currency == currency), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region AddLineItemAsync Tests

        [Fact]
        public async Task AddLineItemAsync_ReturnsNoContent_WhenLineItemIsAdded()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var command = new AddLineItemCommand { InvoiceId = invoiceId, Description = "Test item" };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddLineItemAsync(invoiceId, command);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddLineItemAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;
            var command = new AddLineItemCommand { InvoiceId = Guid.NewGuid() };

            // Act
            var result = await _controller.AddLineItemAsync(invoiceId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddLineItemAsync_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            AddLineItemCommand command = null;

            // Act
            var result = await _controller.AddLineItemAsync(invoiceId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddLineItemAsync_ReturnsBadRequest_WhenInvoiceIdsDoNotMatch()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var command = new AddLineItemCommand { InvoiceId = Guid.NewGuid() };

            // Act
            var result = await _controller.AddLineItemAsync(invoiceId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddLineItemAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var command = new AddLineItemCommand { InvoiceId = invoiceId };
            _controller.ModelState.AddModelError("Description", "Description is required");

            // Act
            var result = await _controller.AddLineItemAsync(invoiceId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task AddLineItemAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var command = new AddLineItemCommand 
            { 
                InvoiceId = invoiceId, 
                Description = "Test item",
                UnitPrice = 50.00m,
                Quantity = 2
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.AddLineItemAsync(invoiceId, command);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region RemoveLineItemAsync Tests

        [Fact]
        public async Task RemoveLineItemAsync_ReturnsNoContent_WhenLineItemIsRemoved()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var lineItemId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveLineItemCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveLineItemAsync(invoiceId, lineItemId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveLineItemAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;
            var lineItemId = Guid.NewGuid();

            // Act
            var result = await _controller.RemoveLineItemAsync(invoiceId, lineItemId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task RemoveLineItemAsync_ReturnsBadRequest_WhenLineItemIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var lineItemId = Guid.Empty;

            // Act
            var result = await _controller.RemoveLineItemAsync(invoiceId, lineItemId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task RemoveLineItemAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var lineItemId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveLineItemCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.RemoveLineItemAsync(invoiceId, lineItemId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<RemoveLineItemCommand>(c => 
                c.InvoiceId == invoiceId && c.LineItemId == lineItemId), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UpdateLineItemAsync Tests

        [Fact]
        public async Task UpdateLineItemAsync_ReturnsNoContent_WhenLineItemIsUpdated()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var command = new UpdateLineItemCommand { InvoiceId = invoiceId };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateLineItemAsync(invoiceId, command);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateLineItemAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;
            var command = new UpdateLineItemCommand { InvoiceId = Guid.NewGuid() };

            // Act
            var result = await _controller.UpdateLineItemAsync(invoiceId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateLineItemAsync_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            UpdateLineItemCommand command = null;

            // Act
            var result = await _controller.UpdateLineItemAsync(invoiceId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateLineItemAsync_ReturnsBadRequest_WhenInvoiceIdsDoNotMatch()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var command = new UpdateLineItemCommand { InvoiceId = Guid.NewGuid() };

            // Act
            var result = await _controller.UpdateLineItemAsync(invoiceId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateLineItemAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var command = new UpdateLineItemCommand { InvoiceId = invoiceId };
            _controller.ModelState.AddModelError("UnitPrice", "Unit price must be positive");

            // Act
            var result = await _controller.UpdateLineItemAsync(invoiceId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        #endregion

        #region UpdateDueDateAsync Tests

        [Fact]
        public async Task UpdateDueDateAsync_ReturnsNoContent_WhenDueDateIsUpdated()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var newDueDate = DateTime.UtcNow.AddDays(30);
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDueDateCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateDueDateAsync(invoiceId, newDueDate);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateDueDateAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;
            var newDueDate = DateTime.UtcNow.AddDays(30);

            // Act
            var result = await _controller.UpdateDueDateAsync(invoiceId, newDueDate);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateDueDateAsync_ReturnsBadRequest_WhenDueDateIsDefault()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var newDueDate = default(DateTime);

            // Act
            var result = await _controller.UpdateDueDateAsync(invoiceId, newDueDate);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateDueDateAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var newDueDate = DateTime.UtcNow.AddDays(45);
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDueDateCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.UpdateDueDateAsync(invoiceId, newDueDate);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateDueDateCommand>(c => 
                c.InvoiceId == invoiceId && c.UpdatedDueDate == newDueDate), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region SendInvoiceAsync Tests

        [Fact]
        public async Task SendInvoiceAsync_ReturnsNoContent_WhenInvoiceIsSent()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var recipientEmail = "recipient@example.com";
            _mediatorMock.Setup(m => m.Send(It.IsAny<SendInvoiceCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SendInvoiceAsync(invoiceId, recipientEmail);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task SendInvoiceAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;
            var recipientEmail = "recipient@example.com";

            // Act
            var result = await _controller.SendInvoiceAsync(invoiceId, recipientEmail);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters", badRequestResult.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task SendInvoiceAsync_ReturnsBadRequest_WhenRecipientEmailIsInvalid(string recipientEmail)
        {
            // Arrange
            var invoiceId = Guid.NewGuid();

            // Act
            var result = await _controller.SendInvoiceAsync(invoiceId, recipientEmail);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters", badRequestResult.Value);
        }

        [Fact]
        public async Task SendInvoiceAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var recipientEmail = "test@example.com";
            _mediatorMock.Setup(m => m.Send(It.IsAny<SendInvoiceCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            await _controller.SendInvoiceAsync(invoiceId, recipientEmail);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<SendInvoiceCommand>(c => 
                c.InvoiceId == invoiceId && c.RecipientEmail == recipientEmail), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region CancelInvoiceAsync Tests

        [Fact]
        public async Task CancelInvoiceAsync_ReturnsNoContent_WhenInvoiceIsCancelled()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var reason = "Customer request";
            _mediatorMock.Setup(m => m.Send(It.IsAny<CancelInvoiceCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.CancelInvoiceAsync(invoiceId, reason);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CancelInvoiceAsync_ReturnsBadRequest_WhenInvoiceIdIsEmpty()
        {
            // Arrange
            var invoiceId = Guid.Empty;
            var reason = "Customer request";

            // Act
            var result = await _controller.CancelInvoiceAsync(invoiceId, reason);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid invoice ID.", badRequestResult.Value);
        }

        [Fact]
        public async Task CancelInvoiceAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var reason = "Billing error";
            _mediatorMock.Setup(m => m.Send(It.IsAny<CancelInvoiceCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            await _controller.CancelInvoiceAsync(invoiceId, reason);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<CancelInvoiceCommand>(c => 
                c.InvoiceId == invoiceId && c.Reason == reason), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task CancelInvoiceAsync_AcceptsNullOrEmptyReason(string reason)
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CancelInvoiceCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.CancelInvoiceAsync(invoiceId, reason);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<CancelInvoiceCommand>(c => 
                c.InvoiceId == invoiceId && c.Reason == reason), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Exception Handling Tests

        [Fact]
        public async Task GetInvoiceByIdAsync_PropagatesException_WhenMediatorThrows()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetInvoiceByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetInvoiceByIdAsync(invoiceId));
        }

        [Fact]
        public async Task CreateInvoiceAsync_PropagatesException_WhenMediatorThrows()
        {
            // Arrange
            var command = new CreateInvoiceCommand 
            { 
                InvoiceDTO = new InvoiceCreateDTO 
                { 
                    InvoiceType = "Booking",
                    IssuedTo = Guid.NewGuid(),
                    IssuedBy = Guid.NewGuid(),
                    Currency = "USD",
                    RecipientName = "Test User",
                    RecipientEmail = "test@example.com",
                    BillingStreet = "123 Test St",
                    BillingCity = "Test City",
                    BillingPostalCode = "12345"
                } 
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new ArgumentException("Invalid invoice data"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _controller.CreateInvoiceAsync(command));
        }

        [Fact]
        public async Task MarkInvoiceAsPaidAsync_PropagatesException_WhenMediatorThrows()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkAsPaidCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new KeyNotFoundException("Invoice not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.MarkInvoiceAsPaidAsync(invoiceId));
        }

        #endregion

        #region Edge Cases Tests

        [Theory]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public async Task ApplyDiscountAsync_ReturnsBadRequest_WhenDiscountAmountIsNegative(decimal discountAmount)
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var currency = "USD";

            // Act
            var result = await _controller.ApplyDiscountAsync(invoiceId, discountAmount, currency);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetDaysUntilDueAsync_HandlesNegativeDaysUntilDue()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var daysUntilDue = -5; // Overdue invoice
            _mediatorMock.Setup(m => m.Send(It.Is<GetDaysUntilDueQuery>(q => q.InvoiceId == invoiceId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(daysUntilDue);

            // Act
            var result = await _controller.GetDaysUntilDueAsync(invoiceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(daysUntilDue, okResult.Value);
        }

        [Fact]
        public async Task GetOutstandingAmountAsync_HandlesZeroOutstandingAmount()
        {
            // Arrange
            var invoiceId = Guid.NewGuid();
            var outstandingAmount = new Money(0m, "USD"); // Fully paid invoice
            _mediatorMock.Setup(m => m.Send(It.Is<GetOutstandingAmountQuery>(q => q.InvoiceId == invoiceId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(outstandingAmount);

            // Act
            var result = await _controller.GetOutstandingAmountAsync(invoiceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(outstandingAmount, okResult.Value);
        }

        #endregion
    }
}