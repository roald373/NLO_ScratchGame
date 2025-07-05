namespace NLO_ScratchGame_Contracts
{
    public record struct ScratchResult
    {
        public required int UserId { get; init; }
        public required int Row { get; init; }
        public required int Column { get; init; }
        public required string Prize { get; init; }
        public required bool SuccessFullyScratched { get; init; }
    }
}
