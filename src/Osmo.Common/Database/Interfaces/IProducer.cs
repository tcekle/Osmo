namespace Osmo.Common.Database.Interfaces;

public interface IProducer
{
    public string Name { get; set; }
    public string UniqueIdentifier { get; set; }
    string Type { get; set; }
}