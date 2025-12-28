using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using FluentValidation;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using NotificationManagement.Application.Contracts;
using NotificationManagement.Application.CQRS.Commands.TemplateCommands;
using NotificationManagement.Application.Validators;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Application.CQRS.Handlers.CommandHandlers.TemplateCommandHandlers;

public class CreateTemplateCommandHandler(
	IMapper mapper,
	ILoggingService<CreateTemplateCommandHandler> logger,
	INotificationTemplateRepository templateRepository,
	IUnitOfWork unitOfWork) : IRequestHandler<CreateTemplateCommand, NotificationTemplateResponseDTO>
{
	public async Task<NotificationTemplateResponseDTO> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
	{
		// Validate
		var validator = new NotificationTemplateCreateDTOValidator();
		var validationResult = await validator.ValidateAsync(request.Template!, cancellationToken);
		
		if (!validationResult.IsValid)
		{
			logger.LogWarning("Template validation failed: {Errors}", 
				string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
			throw new ValidationException(validationResult.Errors.First().ErrorMessage);
		}

		// Check if code already exists
		var existing = await templateRepository.GetByCodeAsync(request.Template!.Code, cancellationToken);
		if (existing != null)
		{
			throw new InvalidOperationException($"Template with code '{request.Template.Code}' already exists");
		}

		// Create template
		var template = NotificationTemplate.Create(
			request.Template.Name,
			request.Template.Code,
			request.Template.Type,
			request.Template.Channel,
			request.Template.SubjectTemplate,
			request.Template.BodyTemplate,
			request.Template.RequiredVariables,
			request.CreatedBy
		);

		await templateRepository.AddAsync(template, cancellationToken);
		await unitOfWork.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Template {TemplateCode} created successfully", template.Code);

		return mapper.Map<NotificationTemplateResponseDTO>(template);
	}
}
