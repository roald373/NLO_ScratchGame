namespace NLOScratchGame_Query_API
{
    public record SupriseCalenderResponse
    {
        public required bool CurrentUserHasScratched { get; init; }

        public required List<ScratchCellModel> Grid { get; init; }
    }

    public record ScratchCellModel
    {
        public required int Row { get; init; }
        public required int Column { get; init; }
        public required bool IsScratched { get; init; }
        public required string Prize { get; init; }
    }
}
