using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeneratedApp.Models;
using GeneratedApp.Data;

namespace GeneratedApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsagerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsagerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usager
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usager>>> GetUsagers()
        {
            return await _context.Usagers.ToListAsync();
        }

        // GET: api/Usager/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Usager>> GetUsager(Guid id)
        {
            var usager = await _context.Usagers.FindAsync(id);

            if (usager == null)
            {
                return NotFound();
            }

            return usager;
        }

        // POST: api/Usager
        [HttpPost]
        public async Task<ActionResult<Usager>> PostUsager(Usager usager)
        {
            _context.Usagers.Add(usager);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsager), new { id = usager.Id }, usager);
        }

        // PUT: api/Usager/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsager(Guid id, Usager usager)
        {
            if (id != usager.Id)
            {
                return BadRequest();
            }

            _context.Entry(usager).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsagerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Usager/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsager(Guid id)
        {
            var usager = await _context.Usagers.FindAsync(id);
            if (usager == null)
            {
                return NotFound();
            }

            _context.Usagers.Remove(usager);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsagerExists(Guid id)
        {
            return _context.Usagers.Any(e => e.Id == id);
        }
    }
}
