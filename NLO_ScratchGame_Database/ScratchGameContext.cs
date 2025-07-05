using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace NLO_ScratchGame_Database
{
    public class ScratchGameContext : DbContext
    {
        public ScratchGameContext(DbContextOptions<ScratchGameContext> options) : base(options) { }

        public DbSet<ScratchCell> ScratchCells { get; set; }
        public DbSet<ScratchAttempt> ScratchAttempts { get; set; }

        public async Task<bool> SeedData(CancellationToken cancellationToken)
        {
            if (!ScratchCells.Any())
            {
                var rand = new Random(420);
                var grid = new List<ScratchCell>();

                var prizePositions = new HashSet<(int, int)>
                {
                    (rand.Next(100), rand.Next(100))
                };

                while (prizePositions.Count < 101)
                    prizePositions.Add((rand.Next(100), rand.Next(100)));

                for (int row = 0; row < 100; row++)
                {
                    for (int col = 0; col < 100; col++)
                    {
                        string prize = "Nothing";
                        if (prizePositions.Contains((row, col)))
                        {
                            prize = prizePositions.First() == (row, col) ? "25k" : "1k";
                        }

                        grid.Add(new ScratchCell
                        {
                            Row = row,
                            Column = col,
                            Prize = prize
                        });
                    }
                }

                ScratchCells.AddRange(grid);
                await SaveChangesAsync(cancellationToken);

                return true;
            }

            return false;
        }

        public async Task ClearDatabase(CancellationToken cancellationToken)
        {
            ScratchCells.RemoveRange(ScratchCells);
            ScratchAttempts.RemoveRange(ScratchAttempts);
            await SaveChangesAsync(cancellationToken);
        }
    }

    public class ScratchCell
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int? ScratchedByUserId { get; set; }
        public string Prize { get; set; } = "Nothing";
    }

    public class ScratchAttempt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTimeOffset ScratchedAt { get; set; }
    }
}
