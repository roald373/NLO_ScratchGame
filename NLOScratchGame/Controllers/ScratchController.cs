using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using NLO_ScratchGame_Contracts;
using NLO_ScratchGame_Database;

namespace NLOScratchGame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScratchController : ControllerBase
    {
        private readonly ILogger<ScratchController> _logger;
        private IBus _bus;
        private readonly ScratchGameContext _context;

        public ScratchController(ILogger<ScratchController> logger, IBus bus, ScratchGameContext context)
        {
            _logger = logger;
            _bus = bus;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Scratch(ScratchRequest request, CancellationToken cancellationToken)
        {
            var command = new ScratchCommand
            {
                UserId = request.UserId,
                Row = request.Row,
                Column = request.Column,
                ScratchedAt = DateTimeOffset.UtcNow
            };

            await _bus.PubSub.PublishAsync(command, cancellationToken);

            return Ok();
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedDatabase(CancellationToken cancellationToken)
        {
            var result = await _context.SeedData(cancellationToken);

            return result ? Ok(new { Message = "Database seeded successfully." })
                : BadRequest(new { Message = "Database already seeded." });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearDatabase(CancellationToken cancellationToken)
        {
            await _context.ClearDatabase(cancellationToken);
            return Ok();
        }
    }
}
