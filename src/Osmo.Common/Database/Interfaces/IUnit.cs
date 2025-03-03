namespace Osmo.Common.Database.Interfaces;

public interface IUnit
{
    Guid Id { get; set; }
    string Name { get; set; }
    DateTime CreatedAt { get; set; }
}