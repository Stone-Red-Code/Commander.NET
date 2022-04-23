using Commander_Net;

Console.WriteLine("Hello, World!");

Commander commander = new Commander();
Command sayCommand = commander.Register(HahaYes, "say", "yell");
sayCommand.Register(HahaYesSub, "kek");

commander.Register(HahaNo, "scream", "yell");
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
    if (commander.ExecuteMultible(Console.ReadLine()!, out List<CommandResult> commandResult))
    {
        commandResult.ForEach((e) => Console.WriteLine(e.ResultType + " " + e.Message));
    }
    else
    {
        Console.WriteLine("xxx Command not found!");
    }
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
        return new CommandResult(ResultType.InvalidInput, "fuck off");
    }

    Console.WriteLine($">>> {no.ToUpper()}");

    return new CommandResult(ResultType.Success);
}