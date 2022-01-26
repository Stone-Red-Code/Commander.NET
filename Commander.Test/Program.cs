using Commander_Net;

Console.WriteLine("Hello, World!");

Commander commander = new Commander();
Command sayCommand = commander.Register(HahaYes, "say", "tell");
sayCommand.Register(HahaYesSub, "kek");

commander.Register(HahaNo, "scream", "yell");
commander.Register((args) => Console.Clear(), "clear");

while (true)
{
    if (!commander.Execute(Console.ReadLine()!))
    {
        Console.WriteLine("Command not found!");
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

void HahaNo(string no)
{
    Console.WriteLine($">>> {no.ToUpper()}");
}