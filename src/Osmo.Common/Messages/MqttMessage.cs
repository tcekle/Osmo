using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.Common.Messages;

/// <summary>
/// Represents a message received from an MQTT broker.
/// </summary>
[Table(name: "mqtt_messages")]
public record MqttMessage
{
    /// <summary>
    /// Gets the unique identifier of the message.
    /// </summary>
    [Column(name: "id")]
    public Guid Id { get; init; }
    
    /// <summary>
    /// Gets the topic of the message.
    /// </summary>
    [Column(name: "topic")]
    public string Topic { get; init; }
    
    /// <summary>
    /// Gets the content type of the message.
    /// </summary>
    [Column(name: "content_type")]
    public string ContentType { get; init; }
    
    /// <summary>
    /// Gets the payload of the message.
    /// </summary>
    [Column(name: "payload")]
    public byte[] Payload { get; init; }
    
    /// <summary>
    /// Gets the payload of the message as a string.
    /// </summary>
    [Column(name: "payload_as_string")]
    public string PayloadAsString { get; init; }
    
    /// <summary>
    /// Gets the timestamp of the message.
    /// </summary>
    [Column(name: "timestamp")]
    public DateTime Timestamp { get; init; }
}