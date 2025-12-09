using BookingManagement.Application.CQRS.Commands.CustomerProjectCommands;
using BookingManagement.Application.CQRS.Queries.CustomerProjectQueries;
using Core.SharedKernel.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingManagement.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class CustomerProjectsController(IMediator mediator) : ControllerBase
	{
		private readonly IMediator _mediator = mediator;

		[HttpGet("my-projects")]
		public async Task<IActionResult> GetMyProjects()
		{
			var customerId = GetUserId();
			var query = new GetProjectsByCustomerQuery { CustomerId = customerId };
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetProjectById(Guid id)
		{
			var query = new GetCustomerProjectByIdQuery { ProjectId = id };
			var result = await _mediator.Send(query);
			if (result == null) return NotFound();
			return Ok(result);
		}

		[HttpGet("search")]
		[AllowAnonymous]
		public async Task<IActionResult> SearchProjects([FromQuery] SearchOpenProjectsQuery query)
		{
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpGet("{projectId:guid}/proposals")]
		public async Task<IActionResult> GetProposalsForProject(Guid projectId)
		{
			var project = await _mediator.Send(new GetCustomerProjectByIdQuery { ProjectId = projectId });
			if (project.CustomerId != GetUserId()) return Forbid();

			var query = new GetProposalsByProjectQuery { ProjectId = projectId };
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpPost("{projectId:guid}/proposals/{proposalId:guid}/accept")]
		public async Task<IActionResult> AcceptProposal(Guid projectId, Guid proposalId)
		{
			var customerId = GetUserId();
			var command = new AcceptProposalCommand
			{
				ProjectId = projectId,
				ProposalId = proposalId,
				CustomerId = customerId
			};

			var success = await _mediator.Send(command);
			if (!success) return BadRequest("Could not accept proposal.");

			return NoContent();
		}

		[HttpPost("{projectId:guid}/proposals/{proposalId:guid}/reject")]
		public async Task<IActionResult> RejectProposal(Guid projectId, Guid proposalId)
		{
			var customerId = GetUserId();
			var command = new RejectProposalCommand
			{
				ProjectId = projectId,
				ProposalId = proposalId,
				CustomerId = customerId
			};

			var success = await _mediator.Send(command);
			if (!success) return BadRequest("Could not accept proposal.");

			return NoContent();
		}

		[HttpPost("post-project")]
		public async Task<IActionResult> PostProject([FromBody] CreateCustomerProjectDTO dto)
		{
			var customerId = GetUserId();
			var command = new CreateCustomerProjectCommand
			{
				CustomerId = customerId,
				Data = dto
			};

			var projectId = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetProjectById), new { id = projectId }, projectId);
		}

		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateCustomerProjectDTO dto)
		{
			if (id != dto.ProjectId) return BadRequest("ID Mismatch");

			var customerId = GetUserId();
			var command = new UpdateCustomerProjectCommand
			{
				ProjectId = id,
				CustomerId = customerId,
				Data = dto
			};

			var success = await _mediator.Send(command);
			if (!success) return BadRequest("Update failed (Project might be closed or not yours).");

			return NoContent();
		}

		[HttpPost("{id:guid}/cancel")]
		public async Task<IActionResult> CancelProject(Guid id, [FromBody] string reason)
		{
			var customerId = GetUserId();
			var command = new CancelProjectCommand
			{
				ProjectId = id,
				CustomerId = customerId,
				Reason = reason
			};

			var success = await _mediator.Send(command);
			if (!success) return BadRequest("Could not cancel project.");

			return NoContent();
		}

		private Guid GetUserId()
		{
			var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
					   ?? User.FindFirst("sub")
					   ?? User.FindFirst("id");

			if (idClaim != null && Guid.TryParse(idClaim.Value, out var id))
			{
				return id;
			}
			throw new UnauthorizedAccessException("User ID not found.");
		}
	}
}