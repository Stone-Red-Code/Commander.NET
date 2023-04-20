namespace Commander_Net;

public class CommandResult
{
    public ResultType ResultType { get; }
    public string Message { get; }

    public CommandResult(ResultType type, string message = "")
    {
        ResultType = type;
        Message = message;
    }

    public static CommandResult Success(string message = "")
    {
        return new CommandResult(ResultType.Success, message);
    }

    public static CommandResult InvalidInput(string message = "")
    {
        return new CommandResult(ResultType.InvalidInput, message);
    }

    public static CommandResult Error(string message = "")
    {
        return new CommandResult(ResultType.Error, message);
    }
}