using Microsoft.AspNetCore.Mvc;
using RepairShop.Common.Helpers;
using Tickets.Contracts.Requests;
using Tickets.Domain.Services;

namespace Tickets.Controllers
{
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketsService _ticketService;

        public TicketsController(ITicketsService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost("tickets")]
        public async Task<IActionResult> Create([FromBody] TicketRequest request)
        {
            try
            {
                var ticket = request.ToTicket();

                if (await _ticketService.CreateAsync(ticket, request.UserId))
                {
                    var ticketResponse = ticket.ToTicketResponse();

                    return CreatedAtAction("Get", new { ticketResponse.Id }, ticketResponse);

                }

                return StatusCode(500, "An error occurred while creating the Ticket.");
            }
            catch (InvalidTicketException)
            {
                return BadRequest("Invalid ticket.");
            }
            catch (InvalidUserIdException)
            {
                return BadRequest("Invalid user ID.");
            }
            catch (UserNotFoundException)
            {
                return NotFound("User not found.");
            }
        }

        [HttpGet("tickets/{id:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            try
            {
                var ticket = await _ticketService.GetAsync(id);

                if (ticket is null)
                {
                    return NotFound();
                }

                var ticketResponse = ticket.ToTicketResponse();
                return Ok(ticketResponse);
            }
            catch (InvalidTicketException)
            {
                return BadRequest("Invalid ticket.");
            }
            catch (UserNotFoundException)
            {
                return NotFound("User not found.");
            }
            catch (TicketNotFoundException)
            {
                return NotFound("User not found.");
            }
        }

        [HttpGet("tickets")]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.GetAllAsync();
            var ticketsResponse = tickets.ToTicketsResponse();
            return Ok(ticketsResponse);
        }

        [HttpPut("tickets/{id:guid}")]
        public async Task<IActionResult> Update([FromMultiSource] UpdateTicketRequest request)
        {
            try
            {
                var existingTicket = await _ticketService.GetAsync(request.Id);

                if (existingTicket is null)
                {
                    return NotFound();
                }

                var ticket = request.ToTicket(existingTicket.DateOfIssue);
                if (await _ticketService.UpdateAsync(ticket, request.Ticket.UserId))
                {
                    var ticketResponse = ticket.ToTicketResponse();
                    return Ok(ticketResponse);
                }

                return StatusCode(500, "An error occurred while Update the Ticket.");

            }
            catch (InvalidTicketException)
            {
                return BadRequest("Invalid ticket.");
            }
            catch (UserNotFoundException)
            {
                return NotFound("User not found.");
            }
        }

        [HttpDelete("tickets/{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deleted = await _ticketService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
