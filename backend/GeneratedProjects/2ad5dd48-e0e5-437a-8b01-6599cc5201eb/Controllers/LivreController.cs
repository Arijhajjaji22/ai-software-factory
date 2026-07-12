using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeneratedApp.Data;
using GeneratedApp.Models;

namespace GeneratedApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LivreController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LivreController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Livre
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Livre>>> GetLivres()
        {
            return await _context.Livres.ToListAsync();
        }

        // GET: api/Livre/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Livre>> GetLivre(Guid id)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre == null)
            {
                return NotFound();
            }
            return livre;
        }

        // POST: api/Livre
        [HttpPost]
        public async Task<ActionResult<Livre>> PostLivre(Livre livre)
        {
            _context.Livres.Add(livre);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLivre), new { id = livre.Id }, livre);
        }

        // PUT: api/Livre/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLivre(Guid id, Livre livre)
        {
            if (id != livre.Id)
            {
                return BadRequest();
            }

            _context.Entry(livre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LivreExists(id))
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

        // DELETE: api/Livre/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivre(Guid id)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre == null)
            {
                return NotFound();
            }

            _context.Livres.Remove(livre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LivreExists(Guid id)
        {
            return _context.Livres.Any(e => e.Id == id);
        }
    }
}