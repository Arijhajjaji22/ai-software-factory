using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeneratedApp.Data;
using GeneratedApp.Models;

namespace GeneratedApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationLogController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/NotificationLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationLog>>> GetAll()
        {
            return await _context.NotificationLogs.ToListAsync();
        }

        // GET: api/NotificationLog/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationLog>> GetById(Guid id)
        {
            var notificationLog = await _context.NotificationLogs.FindAsync(id);

            if (notificationLog == null)
            {
                return NotFound();
            }

            return notificationLog;
        }

        // POST: api/NotificationLog
        [HttpPost]
        public async Task<ActionResult<NotificationLog>> Create(NotificationLog notificationLog)
        {
            _context.NotificationLogs.Add(notificationLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = notificationLog.Id }, notificationLog);
        }

        // PUT: api/NotificationLog/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, NotificationLog notificationLog)
        {
            if (id != notificationLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(notificationLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await NotificationLogExists(id))
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

        // DELETE: api/NotificationLog/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var notificationLog = await _context.NotificationLogs.FindAsync(id);
            if (notificationLog == null)
            {
                return NotFound();
            }

            _context.NotificationLogs.Remove(notificationLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> NotificationLogExists(Guid id)
        {
            return await _context.NotificationLogs.AnyAsync(e => e.Id == id);
        }
    }
}