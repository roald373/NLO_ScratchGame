namespace NLO_ScratchGame_Contracts
{
    public record struct ScratchCommand
    {
        public required int UserId { get; init; }
        public required int Row { get; init; }
        public required int Column { get; init; }
        public required DateTimeOffset ScratchedAt { get; init; }
    }
}
