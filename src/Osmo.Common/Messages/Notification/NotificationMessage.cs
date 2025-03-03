namespace Osmo.Common.Messages.Notification;

/// <summary>
/// A representation of a notification message.
/// </summary>
public record NotificationMessage
{
    /// <summary>
    /// Gets the title of the notification.
    /// </summary>
    public string Title { get; init; }
    
    /// <summary>
    /// Gets the body of the notification.
    /// </summary>
    public string Body { get; init; }
    
    /// <summary>
    /// Gets the level of the notification.
    /// </summary>
    public NotificationLevel Level { get; init; }
};