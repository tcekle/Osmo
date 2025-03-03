using Osmo.Common.Plugins;

namespace Osmo.Plugins.MassTransit;

public class MassTransitPlugin : IOsmoPlugin
{
    /// <summary>
    /// Gets the name of the plugin
    /// </summary>
    public string Name { get; } = "MassTransit";

    /// <summary>
    /// Gets the plugin's description
    /// </summary>
    public string Description { get; set; } = "MassTransit Visualizer";

    /// <summary>
    /// Gets the unique slug to use in the URL
    /// </summary>
    public string PluginSlug { get; set; } = "MassTransitVisualizer";

    /// <summary>
    /// Gets the UI type to use for the plugin
    /// </summary>
    public Type Type { get; set; }
}