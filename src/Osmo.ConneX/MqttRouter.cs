using System.Text.RegularExpressions;

namespace Osmo.ConneX;

using Common.Messages;

/// <summary>
/// Class used to route MQTT messages to the appropriate handler.
/// </summary>
public class MqttRouter
{
    private readonly Dictionary<Regex, Func<MqttMessage, Task>> _routes = new();

    /// <summary>
    /// Register a route for a given topic pattern.
    /// </summary>
    /// <param name="topicPattern">The topic pattern to match against.</param>
    /// <param name="handler">The handler to invoke when a message is received on a matching topic.</param>
    public void RegisterRoute(string topicPattern, Func<MqttMessage, Task> handler)
    {
        var regexPattern = ConvertMqttPatternToRegex(topicPattern);
        var compiledRegex = new Regex(regexPattern, RegexOptions.Compiled);
        _routes[compiledRegex] = handler;
    }

    /// <summary>
    /// Route a message to the appropriate handler.
    /// </summary>
    /// <param name="topic">The topic the message was received on.</param>
    /// <param name="message">The message to route.</param>
    public async Task RouteMessage(string topic, MqttMessage message)
    {
        foreach (var route in _routes)
        {
            if (!route.Key.IsMatch(topic))
            {
                continue;
            }
            
            await route.Value.Invoke(message);
        }
    }

    /// <summary>
    /// Convert an MQTT topic pattern to a regex pattern.
    /// </summary>
    /// <param name="topicPattern">The MQTT topic pattern to convert.</param>
    /// <returns>A regex pattern that matches the same topics as the MQTT pattern.</returns>
    private static string ConvertMqttPatternToRegex(string topicPattern)
    {
        string regex = "^" + Regex.Escape(topicPattern)
                               .Replace(@"\+", @"[^/]+")  // '+' matches a single level
                               .Replace(@"#", @".*")      // '#' matches multiple levels
                               .Replace("/", @"\/"); // Escape the '/' character
        return regex;
    }
}