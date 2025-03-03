using MassTransit;
using Osmo.Common.Messages.Notification;

namespace Osmo.Services;

public class GlobalNotificationService
{
    public event EventHandler<NotificationMessage> OnNotificationReceived;

    public void InvokeNotificationReceived(NotificationMessage notificationMessage)
    {
        OnNotificationReceived?.Invoke(this, notificationMessage);
    }
}

public class MassTransitNotificationMessageConsumer : IConsumer<NotificationMessage>
{
    private readonly GlobalNotificationService _globalNotificationService;

    public MassTransitNotificationMessageConsumer(GlobalNotificationService globalNotificationService)
    {
        _globalNotificationService = globalNotificationService;
    }
    
    public Task Consume(ConsumeContext<NotificationMessage> context)
    {
        _globalNotificationService.InvokeNotificationReceived(context.Message);

        return Task.CompletedTask;
    }
}