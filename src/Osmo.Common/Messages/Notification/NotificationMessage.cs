namespace Osmo.Common.Messages.Notification;

public record NotificationMessage
{
    public string Title { get; init; }
    public string Body { get; init; }
    public NotificationLevel Level { get; init; }
};