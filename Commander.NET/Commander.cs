namespace Commander_Net;

public class Command
{
    internal string[] Identifiers { get; }

    internal readonly Action<string> method;
    internal readonly List<Command> subCommands = new();

    public Command(Action<string> method, string[] identifiers)
    {
        this.method = method;
        Identifiers = identifiers;
    }

    public Command Register(Action<string> method, params string[] identifiers)
    {
        Command command = new Command(method, identifiers);
        subCommands.Add(command);
        return command;
    }

    internal void Execute(string input)
    {
        string[] inputParts = input.Split(' ');
        bool found = false;

        foreach (Command? command in subCommands)
        {
            if (command.Identifiers.Contains(inputParts[0]))
            {
                command.Execute(string.Join(' ', inputParts.Skip(1)));
                found = true;
            }
        }

        if (!found)
        {
            method.Invoke(input);
        }
    }
}

public class Commander
{
    private readonly List<Command> commands = new();

    public Command Register(Action<string> method, params string[] identifiers)
    {
        Command command = new Command(method, identifiers);
        commands.Add(command);
        return command;
    }

    public bool Execute(string input)
    {
        string[] inputParts = input.Split(' ');
        bool found = false;

        foreach (Command? command in commands)
        {
            if (command.Identifiers.Contains(inputParts[0]))
            {
                command.Execute(string.Join(' ', inputParts.Skip(1)));
                found = true;
            }
        }

        return found;
    }
}