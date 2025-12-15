using BookingManagement.Application.CQRS.Commands.CraftsmanProposalCommands;
using BookingManagement.Application.CQRS.Queries.CraftsmanProposalQueries;
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
	public class CraftsmanProposalsController(IMediator mediator) : ControllerBase
	{
		private readonly IMediator _mediator = mediator;

		[HttpGet("my-proposals")]
		public async Task<IActionResult> GetMyProposals()
		{
			var craftsmanId = GetUserId();
			var query = new GetProposalsByCraftsmanQuery { CraftsmanId = craftsmanId };
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetProposalById(Guid id)
		{
			var query = new GetCraftsmanProposalByIdQuery { ProposalId = id };
			var result = await _mediator.Send(query);
			if (result == null) return NotFound();

			//var userId = GetUserId();
			//if (result.CraftsmanId != userId) return Forbid();

			return Ok(result);
		}

		[HttpPost("submit-proposal")]
		public async Task<IActionResult> SubmitProposal([FromBody] CreateCraftsmanProposalDTO dto)
		{
			var craftsmanId = GetUserId();

			var command = new SubmitProposalCommand
			{
				CraftsmanId = craftsmanId,
				Data = dto
			};

			var proposalId = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetProposalById), new { id = proposalId }, proposalId);
		}

		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateProposal(Guid id, [FromBody] UpdateCraftsmanProposalDTO dto)
		{
			if (id != dto.ProposalId) return BadRequest("ID mismatch");

			var craftsmanId = GetUserId();
			var command = new UpdateProposalCommand
			{
				ProposalId = id,
				CraftsmanId = craftsmanId,
				Data = dto
			};

			var success = await _mediator.Send(command);
			if (!success) return NotFound("Proposal not found or cannot be edited.");

			return NoContent();
		}

		[HttpPost("{id:guid}/withdraw")]
		public async Task<IActionResult> WithdrawProposal(Guid id)
		{
			var craftsmanId = GetUserId();
			var command = new WithdrawProposalCommand
			{
				ProposalId = id,
				CraftsmanId = craftsmanId
			};

			var success = await _mediator.Send(command);
			if (!success) return BadRequest("Could not withdraw proposal.");

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
			throw new UnauthorizedAccessException("User ID not found in token.");
		}
	}
}