namespace Osmo.Common.Ui;

public record LayoutComponentRegistration
{
    public Type ComponentType { get; init; }
    public string ComponentIdentifier { get; init; }
}