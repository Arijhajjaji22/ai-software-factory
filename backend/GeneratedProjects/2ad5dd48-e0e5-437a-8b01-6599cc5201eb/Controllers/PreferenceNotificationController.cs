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
    [ApiController]
    [Route("api/[controller]")]
    public class PreferenceNotificationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PreferenceNotificationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PreferenceNotification
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreferenceNotification>>> GetPreferenceNotifications()
        {
            return await _context.PreferenceNotifications.ToListAsync();
        }

        // GET: api/PreferenceNotification/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PreferenceNotification>> GetPreferenceNotification(Guid id)
        {
            var pref = await _context.PreferenceNotifications.FindAsync(id);
            if (pref == null)
            {
                return NotFound();
            }
            return pref;
        }

        // POST: api/PreferenceNotification
        [HttpPost]
        public async Task<ActionResult<PreferenceNotification>> PostPreferenceNotification(PreferenceNotification preferenceNotification)
        {
            _context.PreferenceNotifications.Add(preferenceNotification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPreferenceNotification), new { id = preferenceNotification.Id }, preferenceNotification);
        }

        // PUT: api/PreferenceNotification/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPreferenceNotification(Guid id, PreferenceNotification preferenceNotification)
        {
            if (id != preferenceNotification.Id)
            {
                return BadRequest();
            }

            _context.Entry(preferenceNotification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PreferenceNotificationExists(id))
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

        // DELETE: api/PreferenceNotification/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePreferenceNotification(Guid id)
        {
            var pref = await _context.PreferenceNotifications.FindAsync(id);
            if (pref == null)
            {
                return NotFound();
            }

            _context.PreferenceNotifications.Remove(pref);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PreferenceNotificationExists(Guid id)
        {
            return _context.PreferenceNotifications.Any(e => e.Id == id);
        }
    }
}
