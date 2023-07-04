using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace AuroraSMS.Controllers
{
    [ApiKey]
    [ApiController]
    [Route("api/[action]")]
    public class SMSController : ControllerBase
    {
        private readonly ILogger<SMSController> _logger;
        private readonly AuroraSMSDbContext _db;

        public SMSController(ILogger<SMSController> logger, AuroraSMSDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetUnsentMessages()
        {
            var unsentMessages = await _db
                .Messages
                .Where(o => o.Status == MessageStatus.UnSent)
                .ToListAsync();
            return Ok(unsentMessages);
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var stats = _db.Messages
                .Where(o => o.Status == MessageStatus.Sent)
                .GroupBy(o => o.Created.Date)
                .ToList();
            return Ok(stats);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string msg, string to)
        {
            try
            {
                var message = new AuroraSmsMessage { Message = msg, To = to, Status = MessageStatus.UnSent, Created = DateTime.Now };
                await _db.AddAsync(message);
                await _db.SaveChangesAsync();
                return Ok(message);
            }catch
            {
                return BadRequest("Error!");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMessageStatus(int id, MessageStatus newStatus)
        {
            try
            {
                var message = await _db.Messages.FindAsync(id);
                
                if (message == null)
                {
                    return NotFound();
                }

                message.Status = newStatus;
                await _db.SaveChangesAsync();

                return Ok(message);
            }
            catch
            {
                return BadRequest("Error!");
            }
        }

    }
}