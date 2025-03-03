namespace Commander;

public class HelpText
{
    public string? Description { get; init; }

    public HelpText(string? description)
    {
        Description = description;
    }

    internal HelpText()
    { }

    public static implicit operator HelpText(string helpText)
    {
        return new HelpText(helpText);
    }
}