using AuditManagement.Application.CQRS.Commands;
using AuditManagement.Application.CQRS.Queries;
using Core.SharedKernel.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuditManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuditLogsController(IMediator mediator) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> GetAllAuditLogsAsync()
		{
			var query = new GetAllAuditLogsQuery();
			var response = await mediator.Send(query);
			if (response.Count == 0) return BadRequest("No audit logs found.");
			return Ok(response);
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetAuditLogByIdAsync([FromRoute] Guid id)
		{
			var query = new GetAuditLogByIdQuery { AuditLogId = id };
			var response = await mediator.Send(query);
			if (response == null) return BadRequest($"Audit log with Id {id} not found.");
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> CreateAuditLogAsync([FromBody] CreateAuditLogDto auditLog)
		{
			ArgumentNullException.ThrowIfNull(auditLog, nameof(auditLog));
			var command = new CreateAuditLogCommand { AuditLog = auditLog };
			await mediator.Send(command);
			return Ok();
		}

		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteAuditLogAsync([FromRoute] Guid id)
		{
			if (id == Guid.Empty) throw new ArgumentException("Invalid ID");
			var command = new DeleteAuditLogCommand { AuditLogId = id };
			await mediator.Send(command);
			return Ok();
		}
	}
}
