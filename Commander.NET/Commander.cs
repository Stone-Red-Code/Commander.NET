using System.Text;

namespace Commander_Net;

public class Commander
{
    private readonly List<Command> commands = new();

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
        CommandResult commandMethod(string input)
        {
            method(input);
            return new CommandResult(ResultType.Success);
        }

        return Register(commandMethod, description, identifiers);
    }

    public Command Register(Func<string, CommandResult> method, HelpText description, params string[] identifiers)
    {
        Command command = new Command(method, description, identifiers);
        _ = Register(command);
        return command;
    }

    public Command Register(Command command)
    {
        commands.Add(command);
        return command;
    }

    public bool ExecuteMultible(string input, out List<CommandResult> commandResults)
    {
        List<CommandResult> internalCommandResults = new List<CommandResult>();
        string[] inputParts = input.Split(' ');
        bool found = false;
        foreach (Command? command in commands.Where(command => command.Identifiers.Contains(inputParts[0])))
        {
            List<CommandResult> tempCommandResults = command.ExecuteMultible(string.Join(' ', inputParts.Skip(1)));
            internalCommandResults.AddRange(tempCommandResults);
            found = true;
        }

        commandResults = internalCommandResults;
        return found;
    }

    public bool Execute(string input, out CommandResult? commandResult)
    {
        string[] inputParts = input.Split(' ');

        Command? command = commands.FirstOrDefault(command => command.Identifiers.Contains(inputParts[0]));

        if (command is null)
        {
            commandResult = null;
            return false;
        }

        commandResult = command.Execute(string.Join(' ', inputParts.Skip(1)));
        return true;
    }

    public bool GetHelp(string input, out string helpText)
    {
        string[] inputParts = input.Split(' ');

        StringBuilder internalHelpText = new StringBuilder();

        foreach (Command command in commands.Where(command => command.Identifiers.Contains(inputParts[0]) || string.IsNullOrWhiteSpace(input)))
        {
            _ = internalHelpText.AppendLine(command.GetHelp(string.Join(' ', inputParts.Skip(1))));
        }

        helpText = internalHelpText.ToString();
        return !string.IsNullOrWhiteSpace(helpText);
    }

    public bool PrintHelp(string input = "")
    {
        bool result = GetHelp(input, out string helpText);
        Console.WriteLine(helpText.TrimEnd('\n', '\r'));
        return result;
    }
}