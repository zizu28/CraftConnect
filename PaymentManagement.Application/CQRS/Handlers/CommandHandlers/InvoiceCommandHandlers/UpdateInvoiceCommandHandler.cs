using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents;
using Core.SharedKernel.ValueObjects;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;
using PaymentManagement.Application.DTOs.InvoiceDTOS;
using PaymentManagement.Application.Validators.InvoiceValidators;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.InvoiceCommandHandlers
{
	public class UpdateInvoiceCommandHandler(
		IMapper mapper,
		IInvoiceRepository invoiceRepository,
		ILoggingService<UpdateInvoiceCommandHandler> logger,
		IBackgroundJobService backgroundJob,
		IMessageBroker messageBroker,
		IUnitOfWork unitOfWork) : IRequestHandler<UpdateInvoiceCommand, InvoiceResponseDTO>
	{
		public async Task<InvoiceResponseDTO> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
		{
			var response = new InvoiceResponseDTO();
			var validator = new InvoiceUpdateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.InvoiceDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Invoice update validation failed: {Errors}", validationResult.Errors.Select(e => e.ErrorMessage));
				response.IsSuccess = false;
				response.Message = "Validation Failed";
				response.Errors = [..validationResult.Errors.Select(e => e.ErrorMessage)];
				return response;
			}
			try
			{
				var existingInvoice = await invoiceRepository.GetByIdAsync(request.InvoiceDTO.InvoiceId, cancellationToken);
				if (existingInvoice == null)
				{
					response.IsSuccess = false;
					response.Message = "Invoice not found";
				}
				mapper.Map(request.InvoiceDTO, existingInvoice);
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await invoiceRepository.UpdateAsync(existingInvoice!, cancellationToken);
					await messageBroker.PublishAsync(new InvoiceUpdatedIntegrationEvent(
						request.InvoiceDTO.InvoiceId, existingInvoice!.InvoiceNumber,
						existingInvoice.IssuedTo, existingInvoice.IssuedBy,
						existingInvoice.TotalAmount,
						request.InvoiceDTO.DueDate), cancellationToken);
				}, cancellationToken);

				backgroundJob.Enqueue<IEmailService>(
					"UpdateInvoice",
					invoice => invoice.SendEmailAsync(
						existingInvoice!.Recipient.Email.Address.ToString(),
						$"INVOICE UPDATE",
						$"Invoice with ID {existingInvoice.Id} has been updated successfully",
						false,
						CancellationToken.None
					)
				);

				response = mapper.Map<InvoiceResponseDTO>(existingInvoice);
				response.IsSuccess = true;
				response.Message = "Invoice updated successfully";
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error occurred while updating invoice.");
				response.IsSuccess = false;
				response.Message = "An error occurred while updating the invoice.";
			}
			return response;
		}
	}
}
