using AutoMapper;
using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.Enums;
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
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.InvoiceCommandHandlers
{
	public class CreateInvoiceCommandHandler(
		IMapper mapper,
		ILoggingService<CreateInvoiceCommandHandler> logger,
		IInvoiceRepository invoiceRepository,
		IBackgroundJobService backgroundJob,
		IMessageBroker messageBroker,
		IUnitOfWork unitOfWork) : IRequestHandler<CreateInvoiceCommand, InvoiceResponseDTO>
	{
		public async Task<InvoiceResponseDTO> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
		{
			var response = new InvoiceResponseDTO();
			var validator = new InvoiceCreateDTOValidator();
			var validationResult = await validator.ValidateAsync(request.InvoiceDTO, cancellationToken);
			if (!validationResult.IsValid)
			{
				logger.LogWarning("Invoice creation validation failed: {Errors}", validationResult.Errors.Select(e => e.ErrorMessage));
				response.IsSuccess = false;
				response.Message = "Validation Failed";
				response.Errors = [..validationResult.Errors.Select(e => e.ErrorMessage)];
				return response;
			}
			try
			{
				var invoiceEntity = request.InvoiceDTO.InvoiceType == "Booking" 
					? 
					Invoice.CreateForBooking(
						request.InvoiceDTO.BookingId,
						request.InvoiceDTO.IssuedTo,
						request.InvoiceDTO.IssuedBy,
						new InvoiceRecipient(request.InvoiceDTO.RecipientName,
							new Email(request.InvoiceDTO.RecipientEmail), 
							Enum.Parse<InvoiceRecipientType>(request.InvoiceDTO.RecipientType),
							request.InvoiceDTO.RecipientCompanyName, 
							new PhoneNumber(request.InvoiceDTO.RecipientCountryCode!, request.InvoiceDTO.RecipientPhone!),
							request.InvoiceDTO.RecipientTaxId, request.InvoiceDTO.RecipientRegistrationNumber),
						new Address(request.InvoiceDTO.BillingStreet, request.InvoiceDTO.BillingCity, request.InvoiceDTO.BillingPostalCode),
						request.InvoiceDTO.Currency,
						request.InvoiceDTO.TaxRate,
						request.InvoiceDTO.DueDate,
						request.InvoiceDTO.Notes,
						request.InvoiceDTO.Terms)
					: 
					Invoice.CreateForOrder(
						request.InvoiceDTO.OrderId!.Value,
						request.InvoiceDTO.IssuedTo,
						request.InvoiceDTO.IssuedBy,
						new InvoiceRecipient(request.InvoiceDTO.RecipientName,
							new Email(request.InvoiceDTO.RecipientEmail),
							Enum.Parse<InvoiceRecipientType>(request.InvoiceDTO.RecipientType),
							request.InvoiceDTO.RecipientCompanyName,
							new PhoneNumber(request.InvoiceDTO.RecipientCountryCode!, request.InvoiceDTO.RecipientPhone!),
							request.InvoiceDTO.RecipientTaxId, request.InvoiceDTO.RecipientRegistrationNumber),
						new Address(request.InvoiceDTO.BillingStreet, request.InvoiceDTO.BillingCity, request.InvoiceDTO.BillingPostalCode),
						request.InvoiceDTO.Currency,
						request.InvoiceDTO.TaxRate,
						request.InvoiceDTO.DueDate,
						request.InvoiceDTO.Notes,
						request.InvoiceDTO.Terms);

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					var createdInvoice = await invoiceRepository.AddAsync(invoiceEntity, cancellationToken);
					await messageBroker.PublishAsync(new InvoiceGeneratedIntegrationEvent(
						createdInvoice!.Id, createdInvoice.InvoiceNumber, request.InvoiceDTO.IssuedTo,
						request.InvoiceDTO.IssuedBy, new Money(request.InvoiceDTO.TotalAmount, request.InvoiceDTO.Currency),
						request.InvoiceDTO.DueDate,
						request.InvoiceDTO.BookingId, request.InvoiceDTO.OrderId), cancellationToken);

					response = mapper.Map<InvoiceResponseDTO>(createdInvoice);
				}, cancellationToken);
				
				response.IsSuccess = true;
				response.Message = "Invoice created successfully";

				backgroundJob.Enqueue<IEmailService>(
					"SendInvoiceEmail", 
					invoice => invoice.SendEmailAsync(
							invoiceEntity.Recipient.Email.Address.ToString(),
							"",
							$"Your Invoice {response.InvoiceNumber} is Created",
							false,
							CancellationToken.None)
					);				
				logger.LogInformation("Invoice created successfully with ID: {InvoiceId}", response.Id);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error occurred while creating invoice");
				response.IsSuccess = false;
				response.Message = "Error occurred while creating invoice";
				response.Errors = [ex.Message];
			}
			return response;
		}
	}
}
