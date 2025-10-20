using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using PaymentManagement.Application.CQRS.Queries.PaymentQueries;
using PaymentManagement.Application.DTOs.PaymentDTOs;
using PaymentManagement.Presentation.Controllers;
using PaymentManagement.Domain.Entities;
using Core.SharedKernel.ValueObjects;
using Core.SharedKernel.Enums;

namespace CraftConnect.Tests
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PaymentsController(_mediatorMock.Object);
        }

        #region GetAllPaymentsAsync Tests

        [Fact]
        public async Task GetAllPaymentsAsync_ReturnsOkResult_WithPayments()
        {
            // Arrange
            var payments = new List<PaymentResponseDTO>
            {
                new PaymentResponseDTO { Id = Guid.NewGuid(), Amount = 100.00m, Currency = "USD" },
                new PaymentResponseDTO { Id = Guid.NewGuid(), Amount = 250.50m, Currency = "EUR" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPaymentsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(payments);

            // Act
            var result = await _controller.GetAllPaymentsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PaymentResponseDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetAllPaymentsAsync_ReturnsOkResult_WithEmptyList_WhenNoPayments()
        {
            // Arrange
            var emptyPayments = new List<PaymentResponseDTO>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPaymentsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(emptyPayments);

            // Act
            var result = await _controller.GetAllPaymentsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PaymentResponseDTO>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        #endregion

        #region GetPaymentByIdAsync Tests

        [Fact]
        public async Task GetPaymentByIdAsync_ReturnsOkResult_WhenPaymentExists()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var paymentResponse = new PaymentResponseDTO 
            { 
                Id = paymentId, 
                Amount = 100.00m, 
                Currency = "USD",
                PayerId = Guid.NewGuid(),
                RecipientId = Guid.NewGuid()
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetPaymentByIdQuery>(q => q.PaymentId == paymentId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(paymentResponse);

            // Act
            var result = await _controller.GetPaymentByIdAsync(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PaymentResponseDTO>(okResult.Value);
            Assert.Equal(paymentId, returnValue.Id);
            Assert.Equal(100.00m, returnValue.Amount);
        }

        [Fact]
        public async Task GetPaymentByIdAsync_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<GetPaymentByIdQuery>(q => q.PaymentId == paymentId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((PaymentResponseDTO)null);

            // Act
            var result = await _controller.GetPaymentByIdAsync(paymentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPaymentByIdAsync_CallsMediatorWithCorrectQuery()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var paymentResponse = new PaymentResponseDTO { Id = paymentId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPaymentByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(paymentResponse);

            // Act
            await _controller.GetPaymentByIdAsync(paymentId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<GetPaymentByIdQuery>(q => q.PaymentId == paymentId), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetAllPaymentTransactionsAsync Tests

        [Fact]
        public async Task GetAllPaymentTransactionsAsync_ReturnsOkResult_WithAllTransactions_WhenNoPaymentIdProvided()
        {
            // Arrange
            var transactions = new List<PaymentTransaction>
            {
                PaymentTransaction.Create(Guid.NewGuid(), PaymentTransactionType.Payment, new Money(100m, "USD"), "Payment completed"),
                PaymentTransaction.Create(Guid.NewGuid(), PaymentTransactionType.Authorization, new Money(200m, "EUR"), "Payment authorized")
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetAllPaymentTransactionsQuery>(q => q.PaymentId == Guid.Empty), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetAllPaymentTransactionsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PaymentTransaction>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetAllPaymentTransactionsAsync_ReturnsOkResult_WithFilteredTransactions_WhenPaymentIdProvided()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var transactions = new List<PaymentTransaction>
            {
                PaymentTransaction.Create(paymentId, PaymentTransactionType.Payment, new Money(100m, "USD"), "Payment completed"),
                PaymentTransaction.Create(paymentId, PaymentTransactionType.Authorization, new Money(100m, "USD"), "Payment authorized")
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetAllPaymentTransactionsQuery>(q => q.PaymentId == paymentId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetAllPaymentTransactionsAsync(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PaymentTransaction>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetAllPaymentTransactionsAsync_ReturnsOkResult_WithEmptyList_WhenNoTransactions()
        {
            // Arrange
            var emptyTransactions = new List<PaymentTransaction>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPaymentTransactionsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(emptyTransactions);

            // Act
            var result = await _controller.GetAllPaymentTransactionsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PaymentTransaction>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        #endregion

        #region GetPaymentTransactionsByPaymentIdAsync Tests

        [Fact]
        public async Task GetPaymentTransactionsByPaymentIdAsync_ReturnsOkResult_WhenPaymentIdIsValid()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var transactions = new List<PaymentTransaction>
            {
                PaymentTransaction.Create(paymentId, PaymentTransactionType.Payment, new Money(100m, "USD"), "Payment completed"),
                PaymentTransaction.Create(paymentId, PaymentTransactionType.Authorization, new Money(100m, "USD"), "Payment authorized")
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetAllPaymentTransactionsQuery>(q => q.PaymentId == paymentId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetPaymentTransactionsByPaymentIdAsync(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PaymentTransaction>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetPaymentTransactionsByPaymentIdAsync_ReturnsBadRequest_WhenPaymentIdIsEmpty()
        {
            // Arrange
            var paymentId = Guid.Empty;

            // Act
            var result = await _controller.GetPaymentTransactionsByPaymentIdAsync(paymentId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid payment ID.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetPaymentTransactionsByPaymentIdAsync_DoesNotCallMediator_WhenPaymentIdIsEmpty()
        {
            // Arrange
            var paymentId = Guid.Empty;

            // Act
            await _controller.GetPaymentTransactionsByPaymentIdAsync(paymentId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllPaymentTransactionsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetPaymentTransactionsByPaymentIdAsync_CallsMediatorWithCorrectQuery()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var transactions = new List<PaymentTransaction>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPaymentTransactionsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(transactions);

            // Act
            await _controller.GetPaymentTransactionsByPaymentIdAsync(paymentId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<GetAllPaymentTransactionsQuery>(q => q.PaymentId == paymentId), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetAvailableRefundAmountAsync Tests

        [Fact]
        public async Task GetAvailableRefundAmountAsync_ReturnsOkResult_WhenPaymentIdIsValid()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var refundAmount = new Money(50.00m, "USD");
            _mediatorMock.Setup(m => m.Send(It.Is<GetAvailableRefundAmountQuery>(q => q.PaymentId == paymentId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(refundAmount);

            // Act
            var result = await _controller.GetAvailableRefundAmountAsync(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(refundAmount, okResult.Value);
        }

        [Fact]
        public async Task GetAvailableRefundAmountAsync_ReturnsBadRequest_WhenPaymentIdIsEmpty()
        {
            // Arrange
            var paymentId = Guid.Empty;

            // Act
            var result = await _controller.GetAvailableRefundAmountAsync(paymentId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid payment ID.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAvailableRefundAmountAsync_DoesNotCallMediator_WhenPaymentIdIsEmpty()
        {
            // Arrange
            var paymentId = Guid.Empty;

            // Act
            await _controller.GetAvailableRefundAmountAsync(paymentId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAvailableRefundAmountQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region VerifyPaymentAsync Tests

        [Fact]
        public async Task VerifyPaymentAsync_ReturnsOkResult_WhenVerificationSucceeds()
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "test_reference_123",
                PaymentDTO = new PaymentUpdateDTO
                {
                    PaymentId = Guid.NewGuid(),
                    Status = "success",
                    ModifiedAt = DateTime.UtcNow
                }
            };

            var expectedResult = ("Payment verified successfully.", true);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.VerifyPaymentAsync(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task VerifyPaymentAsync_ReturnsBadRequest_WhenVerificationFails()
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "invalid_reference",
                PaymentDTO = new PaymentUpdateDTO
                {
                    PaymentId = Guid.NewGuid(),
                    Status = "failed"
                }
            };

            var expectedResult = ("Payment verification failed.", false);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.VerifyPaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Payment verification failed.", badRequestResult.Value);
        }

        [Fact]
        public async Task VerifyPaymentAsync_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            VerifyPaymentCommand command = null;

            // Act
            var result = await _controller.VerifyPaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Command data is null.", badRequestResult.Value);
        }

        [Fact]
        public async Task VerifyPaymentAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "test_reference",
                PaymentDTO = new PaymentUpdateDTO { PaymentId = Guid.NewGuid() }
            };
            _controller.ModelState.AddModelError("Reference", "Reference is required");

            // Act
            var result = await _controller.VerifyPaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task VerifyPaymentAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "test_reference_456",
                PaymentDTO = new PaymentUpdateDTO
                {
                    PaymentId = Guid.NewGuid(),
                    Status = "success"
                }
            };

            var expectedResult = ("Payment verified successfully.", true);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResult);

            // Act
            await _controller.VerifyPaymentAsync(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task VerifyPaymentAsync_HandlesInvalidReference(string reference)
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = reference,
                PaymentDTO = new PaymentUpdateDTO { PaymentId = Guid.NewGuid() }
            };

            var expectedResult = ("Invalid reference provided.", false);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.VerifyPaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid reference provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task VerifyPaymentAsync_HandlesPaymentNotFound()
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "valid_reference",
                PaymentDTO = new PaymentUpdateDTO { PaymentId = Guid.NewGuid() }
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new KeyNotFoundException("Payment not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.VerifyPaymentAsync(command));
        }

        [Fact]
        public async Task VerifyPaymentAsync_HandlesExternalServiceException()
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "test_reference",
                PaymentDTO = new PaymentUpdateDTO { PaymentId = Guid.NewGuid() }
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new InvalidOperationException("External service error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.VerifyPaymentAsync(command));
        }

        [Theory]
        [InlineData("success", true)]
        [InlineData("failed", false)]
        [InlineData("abandoned", false)]
        [InlineData("pending", false)]
        public async Task VerifyPaymentAsync_HandlesVariousVerificationStatuses(string status, bool expectedSuccess)
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "test_reference",
                PaymentDTO = new PaymentUpdateDTO
                {
                    PaymentId = Guid.NewGuid(),
                    Status = status
                }
            };

            var expectedMessage = expectedSuccess ? "Payment verified successfully." : "Payment verification failed.";
            var expectedResult = (expectedMessage, expectedSuccess);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.VerifyPaymentAsync(command);

            // Assert
            if (expectedSuccess)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(expectedResult, okResult.Value);
            }
            else
            {
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal(expectedMessage, badRequestResult.Value);
            }
        }

        [Fact]
        public async Task VerifyPaymentAsync_DoesNotCallMediator_WhenCommandIsNull()
        {
            // Arrange
            VerifyPaymentCommand command = null;

            // Act
            await _controller.VerifyPaymentAsync(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<VerifyPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task VerifyPaymentAsync_DoesNotCallMediator_WhenModelStateIsInvalid()
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "test_reference",
                PaymentDTO = new PaymentUpdateDTO { PaymentId = Guid.NewGuid() }
            };
            _controller.ModelState.AddModelError("Reference", "Reference is required");

            // Act
            await _controller.VerifyPaymentAsync(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<VerifyPaymentCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task VerifyPaymentAsync_HandlesTimeoutException()
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "timeout_reference",
                PaymentDTO = new PaymentUpdateDTO { PaymentId = Guid.NewGuid() }
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new TimeoutException("Payment verification timeout"));

            // Act & Assert
            await Assert.ThrowsAsync<TimeoutException>(() => _controller.VerifyPaymentAsync(command));
        }

        [Fact]
        public async Task VerifyPaymentAsync_HandlesVerificationWithCustomMessage()
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "custom_reference",
                PaymentDTO = new PaymentUpdateDTO
                {
                    PaymentId = Guid.NewGuid(),
                    Status = "success"
                }
            };

            var customResult = ("Payment verified with additional details.", true);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(customResult);

            // Act
            var result = await _controller.VerifyPaymentAsync(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(customResult, okResult.Value);
        }

        #endregion

        #region CreatePaymentAsync Tests

        [Fact]
        public async Task CreatePaymentAsync_ReturnsOkResult_WhenPaymentIsCreated()
        {
            // Arrange
            var command = new CreatePaymentCommand 
            { 
                RecipientEmail = "test@example.com",
                PaymentDTO = new PaymentCreateDTO 
                { 
                    Amount = 100.00m, 
                    Currency = "USD",
                    PaymentMethod = "CreditCard",
                    PaymentStatus = "Pending",
                    PaymentType = "Booking",
                    PayerId = Guid.NewGuid(),
                    RecipientId = Guid.NewGuid(),
                    BillingStreet = "123 Main St",
                    BillingCity = "New York",
                    BillingPostalCode = "10001"
                } 
            };
            var paymentResponse = new PaymentResponseDTO { Id = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(paymentResponse);

            // Act
            var result = await _controller.CreatePaymentAsync(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<PaymentResponseDTO>(okResult.Value);
        }

        [Fact]
        public async Task CreatePaymentAsync_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            CreatePaymentCommand command = null;

            // Act
            var result = await _controller.CreatePaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Payment data is null.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreatePaymentAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var command = new CreatePaymentCommand();
            _controller.ModelState.AddModelError("Amount", "Amount is required");

            // Act
            var result = await _controller.CreatePaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreatePaymentAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var command = new CreatePaymentCommand 
            { 
                RecipientEmail = "test@example.com",
                PaymentDTO = new PaymentCreateDTO 
                { 
                    Amount = 100.00m, 
                    Currency = "USD",
                    PaymentMethod = "CreditCard",
                    PaymentStatus = "Pending",
                    PaymentType = "Booking",
                    PayerId = Guid.NewGuid(),
                    RecipientId = Guid.NewGuid(),
                    BillingStreet = "123 Main St",
                    BillingCity = "New York",
                    BillingPostalCode = "10001"
                } 
            };
            var paymentResponse = new PaymentResponseDTO();
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(paymentResponse);

            // Act
            await _controller.CreatePaymentAsync(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UpdatePaymentAsync Tests

        [Fact]
        public async Task UpdatePaymentAsync_ReturnsOkResult_WhenPaymentIsUpdated()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var command = new UpdatePaymentCommand 
            { 
                PaymentDTO = new PaymentUpdateDTO 
                { 
                    PaymentId = paymentId,
                    Description = "Updated payment"
                } 
            };
            var paymentResponse = new PaymentResponseDTO { Id = paymentId };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(paymentResponse);

            // Act
            var result = await _controller.UpdatePaymentAsync(paymentId, command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<PaymentResponseDTO>(okResult.Value);
        }

        [Fact]
        public async Task UpdatePaymentAsync_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            UpdatePaymentCommand command = null;

            // Act
            var result = await _controller.UpdatePaymentAsync(paymentId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid payment data.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdatePaymentAsync_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var urlId = Guid.NewGuid();
            var bodyId = Guid.NewGuid();
            var command = new UpdatePaymentCommand 
            { 
                PaymentDTO = new PaymentUpdateDTO { PaymentId = bodyId } 
            };

            // Act
            var result = await _controller.UpdatePaymentAsync(urlId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid payment data.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdatePaymentAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var command = new UpdatePaymentCommand 
            { 
                PaymentDTO = new PaymentUpdateDTO { PaymentId = paymentId } 
            };
            _controller.ModelState.AddModelError("Description", "Description is invalid");

            // Act
            var result = await _controller.UpdatePaymentAsync(paymentId, command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdatePaymentAsync_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var command = new UpdatePaymentCommand 
            { 
                PaymentDTO = new PaymentUpdateDTO { PaymentId = paymentId } 
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync((PaymentResponseDTO)null);

            // Act
            var result = await _controller.UpdatePaymentAsync(paymentId, command);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region AuthorizePaymentAsync Tests

        [Fact]
        public async Task AuthorizePaymentAsync_ReturnsNoContent_WhenPaymentIsAuthorized()
        {
            // Arrange
            var command = new AuthorizePaymentCommand 
            { 
                PaymentId = Guid.NewGuid(),
                RecipientEmail = "test@example.com",
                ExternalTransactionId = "ext_123"
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AuthorizePaymentAsync(command);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AuthorizePaymentAsync_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            AuthorizePaymentCommand command = null;

            // Act
            var result = await _controller.AuthorizePaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Command data is null.", badRequestResult.Value);
        }

        [Fact]
        public async Task AuthorizePaymentAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var command = new AuthorizePaymentCommand();
            _controller.ModelState.AddModelError("PaymentId", "Payment ID is required");

            // Act
            var result = await _controller.AuthorizePaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        #endregion

        #region CompletePaymentAsync Tests

        [Fact]
        public async Task CompletePaymentAsync_ReturnsNoContent_WhenPaymentIsCompleted()
        {
            // Arrange
            var command = new CompletePaymentCommand 
            { 
                PaymentId = Guid.NewGuid(),
                RecipientEmail = "test@example.com",
                externalTransactionId = "ext_123"
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.CompletePaymentAsync(command);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CompletePaymentAsync_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            CompletePaymentCommand command = null;

            // Act
            var result = await _controller.CompletePaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Command data is null.", badRequestResult.Value);
        }

        [Fact]
        public async Task CompletePaymentAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var command = new CompletePaymentCommand();
            _controller.ModelState.AddModelError("PaymentId", "Payment ID is required");

            // Act
            var result = await _controller.CompletePaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CompletePaymentAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var command = new CompletePaymentCommand 
            { 
                PaymentId = Guid.NewGuid(),
                RecipientEmail = "test@example.com",
                externalTransactionId = "ext_123"
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            await _controller.CompletePaymentAsync(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region FailPaymentAsync Tests

        [Fact]
        public async Task FailPaymentAsync_ReturnsNoContent_WhenPaymentIsFailed()
        {
            // Arrange
            var command = new FailedPaymentCommand 
            { 
                PaymentId = Guid.NewGuid(),
                FailureReason = "Insufficient funds"
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            // Act
            var result = await _controller.FailPaymentAsync(command);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task FailPaymentAsync_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            FailedPaymentCommand command = null;

            // Act
            var result = await _controller.FailPaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Command data is null.", badRequestResult.Value);
        }

        [Fact]
        public async Task FailPaymentAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var command = new FailedPaymentCommand();
            _controller.ModelState.AddModelError("PaymentId", "Payment ID is required");

            // Act
            var result = await _controller.FailPaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task FailPaymentAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var command = new FailedPaymentCommand 
            { 
                PaymentId = Guid.NewGuid(),
                FailureReason = "Payment gateway error"
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            // Act
            await _controller.FailPaymentAsync(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region RefundPaymentAsync Tests

        [Fact]
        public async Task RefundPaymentAsync_ReturnsNoContent_WhenRefundIsProcessed()
        {
            // Arrange
            var command = new RefundPaymentCommand 
            { 
                PaymentId = Guid.NewGuid(),
                RecipientEmail = "test@example.com",
                Currency = "USD",
                RefundAmount = 50.00m,
                Reason = "Customer request",
                InitiatedBy = Guid.NewGuid()
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.RefundPaymentAsync(command);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RefundPaymentAsync_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Arrange
            RefundPaymentCommand command = null;

            // Act
            var result = await _controller.RefundPaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Command data is null.", badRequestResult.Value);
        }

        [Fact]
        public async Task RefundPaymentAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var command = new RefundPaymentCommand();
            _controller.ModelState.AddModelError("RefundAmount", "Refund amount must be greater than zero");

            // Act
            var result = await _controller.RefundPaymentAsync(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task RefundPaymentAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var command = new RefundPaymentCommand 
            { 
                PaymentId = Guid.NewGuid(),
                RecipientEmail = "test@example.com",
                Currency = "USD",
                RefundAmount = 25.00m,
                Reason = "Partial refund requested",
                InitiatedBy = Guid.NewGuid()
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            await _controller.RefundPaymentAsync(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeletePaymentAsync Tests

        [Fact]
        public async Task DeletePaymentAsync_ReturnsNoContent_WhenPaymentIsDeleted()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var email = "test@example.com";
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeletePaymentCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.DeletePaymentAsync(paymentId, email);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePaymentAsync_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var email = "test@example.com";
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeletePaymentCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            await _controller.DeletePaymentAsync(paymentId, email);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<DeletePaymentCommand>(c => c.Id == paymentId && c.Email == email), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task DeletePaymentAsync_AcceptsNullOrEmptyEmail(string email)
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeletePaymentCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.DeletePaymentAsync(paymentId, email);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<DeletePaymentCommand>(c => c.Id == paymentId && c.Email == email), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Exception Handling Tests

        [Fact]
        public async Task GetPaymentByIdAsync_PropagatesException_WhenMediatorThrows()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPaymentByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetPaymentByIdAsync(paymentId));
        }

        [Fact]
        public async Task CreatePaymentAsync_PropagatesException_WhenMediatorThrows()
        {
            // Arrange
            var command = new CreatePaymentCommand 
            { 
                RecipientEmail = "test@example.com",
                PaymentDTO = new PaymentCreateDTO 
                { 
                    Amount = 100.00m, 
                    Currency = "USD",
                    PaymentMethod = "CreditCard",
                    PaymentStatus = "Pending",
                    PaymentType = "Booking",
                    PayerId = Guid.NewGuid(),
                    RecipientId = Guid.NewGuid(),
                    BillingStreet = "123 Main St",
                    BillingCity = "New York",
                    BillingPostalCode = "10001"
                } 
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new ArgumentException("Invalid payment data"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _controller.CreatePaymentAsync(command));
        }

        [Fact]
        public async Task VerifyPaymentAsync_PropagatesException_WhenMediatorThrows()
        {
            // Arrange
            var command = new VerifyPaymentCommand
            {
                Reference = "test_reference",
                PaymentDTO = new PaymentUpdateDTO { PaymentId = Guid.NewGuid() }
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new InvalidOperationException("External service error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.VerifyPaymentAsync(command));
        }

        [Fact]
        public async Task GetAllPaymentTransactionsAsync_PropagatesException_WhenMediatorThrows()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPaymentTransactionsQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetAllPaymentTransactionsAsync());
        }

        #endregion
    }
}