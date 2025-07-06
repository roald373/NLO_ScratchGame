namespace NLOScratchGame_Query_API.Models
{
    public record SupriseCalenderResponse
    {
        public required bool CurrentUserHasScratched { get; init; }

        public required List<ScratchCellModel> Grid { get; init; }
    }
}
