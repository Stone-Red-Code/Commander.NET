using System.Text;

namespace Commander_Net;

public class Command
{
    public string[] Identifiers { get; init; }
    public Func<string, CommandResult> Method { get; init; }
    public HelpText HelpText { get; init; }

    internal readonly List<Command> subCommands = new();

    internal Command(Func<string, CommandResult> method, HelpText description, string[] identifiers)
    {
        Method = method;
        HelpText = description;
        Identifiers = identifiers;
    }

    public Command()
    {
        Identifiers = Array.Empty<string>();
        HelpText = string.Empty;
        Method = new Func<string, CommandResult>((_) => CommandResult.Success);
    }

    public Command Register(Action<string> method, params string[] identifiers)
    {
        return Register(method, new HelpText(), identifiers);
    }

    public Command Register(Func<string, CommandResult> method, params string[] identifiers)
    {
        return Register(method, new HelpText(), identifiers);
    }

    public Command Register(Action<string> method, HelpText description, params string[] identifiers)
    {
        Func<string, CommandResult> commandMethod = (input) =>
        {
            method(input);
            return new CommandResult(ResultType.Success);
        };

        return Register(commandMethod, description, identifiers);
    }

    public Command Register(Func<string, CommandResult> method, HelpText description, params string[] identifiers)
    {
        Command command = new Command(method, description, identifiers);
        Register(command);
        return command;
    }

    public Command Register(Command command)
    {
        subCommands.Add(command);
        return command;
    }

    internal List<CommandResult> ExecuteMultible(string input, List<CommandResult>? commandResults = null)
    {
        string[] inputParts = input.Split(' ');
        bool found = false;

        commandResults ??= new List<CommandResult>();

        foreach (Command? command in subCommands.Where(command => command.Identifiers.Contains(inputParts[0])))
        {
            command.ExecuteMultible(string.Join(' ', inputParts.Skip(1)), commandResults);
            found = true;
        }

        if (!found)
        {
            CommandResult result;
            try
            {
                result = Method(input);
            }
            catch (Exception)
            {
                result = new CommandResult(ResultType.UnexpectedError);
            }

            commandResults.Add(result);
        }

        return commandResults;
    }

    internal CommandResult Execute(string input)
    {
        string[] inputParts = input.Split(' ');

        Command? command = subCommands.FirstOrDefault(command => command.Identifiers.Contains(inputParts[0]));

        if (command is not null)
        {
            return command.Execute(string.Join(' ', inputParts.Skip(1)));
        }

        try
        {
            return Method(input);
        }
        catch (Exception)
        {
            return new CommandResult(ResultType.UnexpectedError);
        }
    }

    internal string GetHelp(string input)
    {
        string[] inputParts = input.Split(' ');

        StringBuilder helpText = new StringBuilder($"{string.Join(" | ", Identifiers)} > {HelpText?.Description}");

        foreach (Command command in subCommands.Where(command => command.Identifiers.Contains(inputParts[0]) || string.IsNullOrWhiteSpace(input)))
        {
            helpText.Append($"{Environment.NewLine}    {command.GetHelp(string.Join(' ', inputParts.Skip(1)))}");
        }

        return helpText.ToString();
    }
}