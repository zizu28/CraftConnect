using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.ProductIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Commands.ProductCommands;
using ProductInventoryManagement.Application.DTOs.ProductDTOs;
using ProductInventoryManagement.Application.Validators.ProductValidators;
using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Application.CQRS.Handlers.CommandHandlers.ProductCommandHandlers
{
	public class ProductCreateCommandHandler(
		IProductRepository productRepository,
		IMapper mapper,
		ILoggingService<ProductCreateCommandHandler> logger,
		IBackgroundJobService backgroundJob,
		IMessageBroker messageBroker,
		IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, ProductResponseDTO>
	{
		public async Task<ProductResponseDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
		{
			var response = new ProductResponseDTO();
			var validator = new ProductCreateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.ProductDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				response.IsSuccess = false;
				response.Message = "Product creation failed due to validation errors.";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				logger.LogWarning("Product creation failed due to validation errors: {Errors}", response.Errors);
				return response;
			}
			try
			{
				var productEntity = Product.Create(
					request.ProductDTO.Name,
					request.ProductDTO.Description,
					request.ProductDTO.Price,
					request.ProductDTO.StockQuantity,
					request.ProductDTO.CategoryId,
					request.ProductDTO.CraftmanId);
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await productRepository.AddAsync(productEntity, cancellationToken);
					await messageBroker.PublishAsync(new ProductCreatedIntegrationEvent
					{
						ProductId = response.ProductId,
						ProductName = response.Name,
						CategoryId = response.CategoryId,
						CraftmanId = response.CraftmanId,
						Price = response.Price,
						StockQuantity = response.StockQuantity,
						IsActive = response.IsActive
					}, cancellationToken);
				}, cancellationToken);
				
				var productResponseDTO = mapper.Map<ProductResponseDTO>(productEntity);
				response.IsSuccess = true;
				response.Message = "Product created successfully.";
				logger.LogInformation("Product created successfully with ID: {ProductId}", productEntity.Id);
				backgroundJob.Enqueue<IEmailService>(
					"Product-Added",
					product => product.SendEmailAsync(request.CraftmanEmail,
					$"Product {productEntity.Name} Added",
					$"Product with ID {productEntity.Id} of {productEntity.Inventory.Quantity} in quantity has been added.",
					true,
					cancellationToken));
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = "An error occurred while creating the product.";
				response.Errors = [ex.Message];
				logger.LogError(ex, "An error occurred while creating the product: {ErrorMessage}", ex.Message);
			}
			return response;
		}
	}
}
