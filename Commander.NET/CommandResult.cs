namespace Commander_Net;

public class CommandResult
{
    public ResultType ResultType { get; }
    public string Message { get; }

    public static CommandResult Success => new CommandResult(ResultType.Success);
    public static CommandResult InvalidInput => new CommandResult(ResultType.InvalidInput);
    public static CommandResult InternalError => new CommandResult(ResultType.InternalError);

    public CommandResult(ResultType type, string message = "")
    {
        ResultType = type;
        Message = message;
    }
}