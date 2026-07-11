using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeneratedApp.Data;
using GeneratedApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneratedApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MembresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Membre>>> GetMembres()
        {
            return await _context.Membres.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Membre>> GetMembre(Guid id)
        {
            var membre = await _context.Membres.FindAsync(id);

            if (membre == null)
            {
                return NotFound();
            }

            return membre;
        }

        [HttpPost]
        public async Task<ActionResult<Membre>> PostMembre(Membre membre)
        {
            _context.Membres.Add(membre);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMembre), new { id = membre.Id }, membre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMembre(Guid id, Membre membre)
        {
            if (id != membre.Id)
            {
                return BadRequest();
            }

            _context.Entry(membre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MembreExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMembre(Guid id)
        {
            var membre = await _context.Membres.FindAsync(id);
            if (membre == null)
            {
                return NotFound();
            }

            _context.Membres.Remove(membre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MembreExists(Guid id)
        {
            return _context.Membres.Any(e => e.Id == id);
        }
    }
}