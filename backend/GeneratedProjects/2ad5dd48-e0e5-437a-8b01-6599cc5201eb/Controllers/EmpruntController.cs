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
    [Route("api/[controller]")]
    [ApiController]
    public class EmpruntController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpruntController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Emprunt
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Emprunt>>> GetEmprunts()
        {
            return await _context.Emprunts.ToListAsync();
        }

        // GET: api/Emprunt/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Emprunt>> GetEmprunt(Guid id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt == null)
            {
                return NotFound();
            }
            return emprunt;
        }

        // POST: api/Emprunt
        [HttpPost]
        public async Task<ActionResult<Emprunt>> PostEmprunt(Emprunt emprunt)
        {
            _context.Emprunts.Add(emprunt);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmprunt), new { id = emprunt.Id }, emprunt);
        }

        // PUT: api/Emprunt/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmprunt(Guid id, Emprunt emprunt)
        {
            if (id != emprunt.Id)
            {
                return BadRequest();
            }

            _context.Entry(emprunt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpruntExists(id))
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

        // DELETE: api/Emprunt/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmprunt(Guid id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt == null)
            {
                return NotFound();
            }

            _context.Emprunts.Remove(emprunt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmpruntExists(Guid id)
        {
            return _context.Emprunts.Any(e => e.Id == id);
        }
    }
}
