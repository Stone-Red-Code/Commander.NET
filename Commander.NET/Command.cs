using System.Text;

namespace Commander;

public class Command
{
    internal readonly List<Command> subCommands = [];
    public string[] Identifiers { get; set; }
    public Func<string, CommandResult> Method { get; set; }
    public HelpText HelpText { get; set; }

    public Command()
    {
        Identifiers = [];
        HelpText = string.Empty;
        Method = new Func<string, CommandResult>((_) => CommandResult.Success());
    }

    internal Command(Func<string, CommandResult> method, HelpText description, string[] identifiers)
    {
        Method = method;
        HelpText = description;
        Identifiers = identifiers;
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
        subCommands.Add(command);
        return command;
    }

    public Command WithDescription(string description)
    {
        HelpText = description;
        return this;
    }

    public Command WithIdentifiers(params string[] identifiers)
    {
        Identifiers = identifiers;
        return this;
    }

    public Command WithMethod(Func<string, CommandResult> method)
    {
        Method = method;
        return this;
    }

    internal List<CommandResult> ExecuteMultible(string input, List<CommandResult>? commandResults = null)
    {
        string[] inputParts = input.Split(' ');
        bool found = false;

        commandResults ??= [];

        foreach (Command? command in subCommands.Where(command => command.Identifiers.Contains(inputParts[0])))
        {
            _ = command.ExecuteMultible(string.Join(' ', inputParts.Skip(1)), commandResults);
            found = true;
        }

        if (!found)
        {
            CommandResult result;
            try
            {
                result = Method(input);
            }
            catch (Exception ex)
            {
                result = new CommandResult(ResultType.UnexpectedError, ex.ToString());
            }

            commandResults.Add(result);
        }

        return commandResults;
    }

    internal CommandResult Execute(string input)
    {
        string[] inputParts = input.Split(' ');

        Command? command = subCommands.Find(command => command.Identifiers.Contains(inputParts[0]));

        if (command is not null)
        {
            return command.Execute(string.Join(' ', inputParts.Skip(1)));
        }

        try
        {
            return Method(input);
        }
        catch (Exception ex)
        {
            return new CommandResult(ResultType.UnexpectedError, ex.ToString());
        }
    }

    internal string GetHelp(string input, int indentation = 2, int paddingSize = 0, bool includeDescription = true)
    {
        string[] inputParts = input.Split(' ');

        if (paddingSize - indentation < 0)
        {
            paddingSize = indentation;
        }

        StringBuilder helpText = new StringBuilder(string.Join(" | ", Identifiers).PadRight(paddingSize - indentation));

        if (!string.IsNullOrWhiteSpace(HelpText.Description) && includeDescription)
        {
            _ = helpText.Append($" > {HelpText.Description}");
        }

        foreach (Command command in subCommands.Where(command => command.Identifiers.Contains(inputParts[0]) || string.IsNullOrWhiteSpace(input)))
        {
            _ = helpText.Append($"{Environment.NewLine}{new string(' ', indentation)}{command.GetHelp(string.Join(' ', inputParts.Skip(1)), indentation + 2, paddingSize, includeDescription)}");
        }

        // Indentation is 2 when this is called from the Commander class/not a subcommand
        if (subCommands.Count > 0 && indentation == 2)
        {
            _ = helpText.AppendLine();
        }

        return helpText.ToString();
    }
}