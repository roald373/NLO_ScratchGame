namespace NLOScratchGame_Command_API.Requests
{
    public record struct ScratchRequest
    {
        public required int UserId { get; init; }
        public required int Row { get; init; }
        public required int Column { get; init; }
    };
}
