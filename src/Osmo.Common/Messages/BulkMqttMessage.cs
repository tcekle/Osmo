namespace Osmo.Common.Messages;

/// <summary>
/// Represents a collection of messages received from an MQTT broker.
/// </summary>
public record BulkMqttMessage
{
    /// <summary>
    /// Gets a collection of messages.
    /// </summary>
    public IEnumerable<MqttMessage> Messages { get; init; }
}