using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLO_ScratchGame_Database;
using NLOScratchGame_Query_API.Models;

namespace NLOScratchGame_Query_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarController : ControllerBase
    {
        private ScratchGameContext _context;

        public CalendarController(ScratchGameContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int userId, CancellationToken cancellationToken)
        {
            var scratchCells = await _context.ScratchCells.ToListAsync(cancellationToken);
            var userHasScratched = await _context.ScratchAttempts.AnyAsync(a => a.UserId == userId, cancellationToken);

            var supriseCalenderResponse = new SupriseCalenderResponse
            {
                CurrentUserHasScratched = userHasScratched,
                Grid = CreateGrid(scratchCells)
            };

            return Ok(supriseCalenderResponse);
        }

        private List<ScratchCellModel> CreateGrid(List<ScratchCell> scratchCells)
        {
            var grid = new List<ScratchCellModel>();
         
            foreach(var cell in scratchCells)
            {
                grid.Add(new ScratchCellModel
                {
                    Row = cell.Row,
                    Column = cell.Column,
                    IsScratched = cell.ScratchedByUserId.HasValue,
                    Prize = cell.ScratchedByUserId.HasValue ? cell.Prize : "?"
                });
            }

            return grid;
        }
    }
}
