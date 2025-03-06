using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace Osmo.ConneX;

using Common.Messages.Notification;

/// <summary>
/// Service for processing MQTT messages from ConneX.
/// </summary>
internal class ConneXMqttService : IHostedService
{
    private readonly IBus _massTransitBus;
    private readonly IOptions<ConneXOptions> _options;
    private IManagedMqttClient _mqttClient;

    private List<MqttApplicationMessage> _applicationMessages = new(100);

    /// <summary>
    /// Gets a collection of the latest messages.
    /// </summary>
    public IEnumerable<MqttApplicationMessage> LatestMessages => _applicationMessages;

    /// <summary>
    /// Creates a new instance of the <see cref="ConneXMqttService"/> class.
    /// </summary>
    /// <param name="massTransitBus">A MassTransit bus for publishing messages.</param>
    /// <param name="options">An instance of the <see cref="IOptions{TOptions}"/> class for <see cref="ConneXOptions"/>.</param>
    public ConneXMqttService(IBus massTransitBus, IOptions<ConneXOptions> options)
    {
        _massTransitBus = massTransitBus;
        _options = options;
    }
    
    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_options.Value.ConneXHost.HostName)
            .Build();
        
        var mqttClient = new MqttClientFactory().CreateMqttClient();
        mqttClient.ApplicationMessageReceivedAsync += MqttClientOnApplicationMessageReceivedAsync;
        await mqttClient.ConnectAsync(mqttClientOptions, cancellationToken);
        
        await mqttClient.SubscribeAsync("#", cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient is null)
        {
            return;
        }

        await _mqttClient.StopAsync();
    }
    
    /// <summary>
    /// Message received handler.
    /// </summary>
    /// <param name="arg">The MQTT application message received event arguments.</param>
    private async Task MqttClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        if (_applicationMessages.Count >= 100)
        {
            _applicationMessages.RemoveAt(0); // Remove first;
        }
        
        _applicationMessages.Add(arg.ApplicationMessage);
        
        await _massTransitBus.Publish(new NotificationMessage
        {
            Title = arg.ApplicationMessage.Topic,
            Body = arg.ApplicationMessage.ConvertPayloadToString(),
            Level = NotificationLevel.Information
        });
    }
}