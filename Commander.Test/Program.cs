using Commander_Net;

Console.WriteLine("Hello, World!");

Commander commander = new Commander();
Command sayCommand = commander.Register(HahaYes, "say", "yell");
sayCommand.Register(HahaYesSub, "kek");

commander.Register(HahaNo, "scream", "yell");
commander.Register(Add, "add");
commander.Register((args) => Console.Clear(), "clear");
commander.Register((input) => commander.PrintHelp(input) ? CommandResult.Success : CommandResult.InvalidInput, "help");

Command fancyCommand = commander.Register(_ => new CommandResult(ResultType.InvalidInput, "How about no?"), "fancy");
fancyCommand.Register(new Command()
{
    Identifiers = new string[] { "yes", "YES" },
    HelpText = "Says that the input is fancy",

    Method = (i) => { Console.WriteLine($"Yes, {i} is very fancy!"); return new CommandResult(ResultType.Success); }
});

fancyCommand.Register(new Command()
{
    Identifiers = new string[] { "no", "NO" },
    HelpText = "Says that the input is not fancy",
    Method = (i) => { Console.WriteLine($"Yes, {i} is not very fancy!"); return new CommandResult(ResultType.Success); }
});

while (true)
{
    if (commander.ExecuteMultible(Console.ReadLine()!, out List<CommandResult> commandResults))
    {
        foreach (CommandResult commandResult in commandResults)
        {
            Console.ForegroundColor = commandResult.ResultType switch
            {
                ResultType.Success => ConsoleColor.Green,
                ResultType.InvalidInput => ConsoleColor.DarkYellow,
                ResultType.InternalError => ConsoleColor.Red,
                ResultType.UnexpectedError => ConsoleColor.DarkRed,
                _ => ConsoleColor.Magenta
            };

            Console.WriteLine($"[{commandResult.ResultType}] {commandResult.Message}");
            Console.ResetColor();
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Command not found!");
        Console.ResetColor();

        _ = commander.PrintHelp();

    }
    Console.WriteLine();
}

void HahaYes(string yes)
{
    Console.WriteLine($">>> {yes}");
}

void HahaYesSub(string yes)
{
    Console.WriteLine($">>> {yes} xD");
}

CommandResult HahaNo(string no)
{
    if (string.IsNullOrWhiteSpace(no))
    {
        return new CommandResult(ResultType.InvalidInput, "No the command doesn't work like that!");
    }

    Console.WriteLine($">>> {no.ToUpper()}");

    return new CommandResult(ResultType.Success);
}

CommandResult Add(string input)
{
    string[] parts = input.Split(' ');
    if (parts.Length != 2)
    {
        return new CommandResult(ResultType.InvalidInput, "Invalid input!");
    }
    Console.WriteLine(int.Parse(parts[0]) + int.Parse(parts[1]));
    return CommandResult.Success;
}