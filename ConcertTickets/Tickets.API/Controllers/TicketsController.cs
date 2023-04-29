using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickets.API.Data;
using Tickets.API.Helpers;
using Tickets.Shared.DTOs;
using Tickets.Shared.Entites;


namespace Tickets.API.Controllers
{
    [ApiController]
    [Route("/api/tickets")]

    public class TicketsController : ControllerBase
    {
        private readonly DataContext _context;

        public TicketsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Tickets
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                int filterValue;
                if (int.TryParse(pagination.Filter, out filterValue))
                {
                    queryable = queryable.Where(x => x.Id == filterValue);
                }
                else
                {
                    bool boolFilter;
                    if (bool.TryParse(pagination.Filter, out boolFilter))
                    {
                        queryable = queryable.Where(x => x.Used == bool.Parse(pagination.Filter.ToLower()));
                    }
                    else
                    {
                        queryable = queryable.Where(x => x.Entrance!.ToLower().Contains(pagination.Filter.ToLower()));
                    }

                }
            }

            return Ok(await queryable
                .OrderBy(x => x.Id)
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Tickets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Id == int.Parse(pagination.Filter));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }



        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket is null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        [HttpGet("combo")]
        public async Task<ActionResult> GetCombo()
        {
            return Ok(await _context.Tickets.ToListAsync());
        }



        [HttpPost]
        public async Task<ActionResult> Post(Ticket ticket)
        {
            _context.Add(ticket);
            try
            {

                await _context.SaveChangesAsync();
                return Ok(ticket);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un registro con el mismo Id.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

        }

        [HttpPut]
        public async Task<ActionResult> PutAsync(Ticket ticket)
        {
            _context.Update(ticket);
            try
            {

                await _context.SaveChangesAsync();
                return Ok(ticket);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un registro con el mismo Id.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var afectedRows = await _context.Tickets
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();

            if (afectedRows == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
