using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Commands.ProductCommands;
using ProductInventoryManagement.Application.Validators.ProductValidators;
using ProductInventoryManagement.Domain.Entities;

namespace ProductInventoryManagement.Application.CQRS.Handlers.CommandHandlers.ProductCommandHandlers
{
	public class UpdateProductCommandHandler(
		IProductRepository productRepository,
		IMapper mapper,
		ILoggingService<UpdateProductCommandHandler> logger,
		IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, ProductResponseDTO>
	{
		public async Task<ProductResponseDTO> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
		{
			var response = new ProductResponseDTO();
			var validator = new ProductUpdateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.ProductDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				response.IsSuccess = false;
				response.Message = "Product update failed due to validation errors.";
				response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
				logger.LogWarning("Product update failed due to validation errors: {Errors}", response.Errors);
				return response;
			}
			try
			{
				var existingProduct = await productRepository.GetByIdAsync(request.ProductDTO.ProductId, cancellationToken);
				if (existingProduct == null)
				{
					response.IsSuccess = false;
					response.Message = "Product not found.";
					logger.LogWarning("Product with ID: {ProductId} not found for update.", request.ProductDTO.ProductId);
					return response;
				}

				mapper.Map(request.ProductDTO, existingProduct);
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await productRepository.UpdateAsync(existingProduct, cancellationToken);
				}, cancellationToken);
				var productResponseDTO = mapper.Map<ProductResponseDTO>(existingProduct);
				response = productResponseDTO;
				response.IsSuccess = true;
				response.Message = "Product updated successfully.";
				logger.LogInformation("Product updated successfully with ID: {ProductId}", existingProduct.Id);
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = "An error occurred while updating the product.";
				response.Errors = [ex.Message];
				logger.LogError(ex, "An error occurred while updating the product: {ErrorMessage}", ex.Message);
			}
			return response;
		}
	}
}
