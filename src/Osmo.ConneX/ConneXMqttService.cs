using MassTransit;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Osmo.Common.Messages.Notification;

namespace Osmo.ConneX;

internal class ConneXMqttService : IHostedService
{
    private readonly IBus _massTransitBus;
    private IManagedMqttClient _mqttClient;

    private List<MqttApplicationMessage> _applicationMessages = new(100);

    public IEnumerable<MqttApplicationMessage> LatestMessages => _applicationMessages;

    // public int AverageMessageSize
    // {
    //     get
    //     {
    //         return (int)_applicationMessages.Select(x => x.PayloadSegment.Count).Average();
    //     }
    // }

    public ConneXMqttService(IBus massTransitBus)
    {
        _massTransitBus = massTransitBus;
    }
    
    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var options = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId("Osmo")
                .WithTcpServer("connexion.dataio.internal", 1883)
                .Build())
            .Build();

        _mqttClient = new MqttFactory().CreateManagedMqttClient();
        
        await _mqttClient.SubscribeAsync("#");
        _mqttClient.ApplicationMessageReceivedAsync += MqttClientOnApplicationMessageReceivedAsync;
        await _mqttClient.StartAsync(options);
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