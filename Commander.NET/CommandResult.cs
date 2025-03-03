namespace Commander;

public class CommandResult(ResultType type, string message = "")
{
    public ResultType ResultType { get; } = type;
    public string Message { get; } = message;

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