namespace NLOScratchGame_Query_API.Models
{
    public record ScratchCellModel
    {
        public required int Row { get; init; }
        public required int Column { get; init; }
        public required bool IsScratched { get; init; }
        public required string Prize { get; init; }
    }
}
