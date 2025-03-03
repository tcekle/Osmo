using Osmo.Common.Plugins;

namespace Osmo.ConneX;

public class ConneXPlugin : IOsmoPlugin
{
    /// <summary>
    /// Gets the name of the plugin
    /// </summary>
    public string Name => "ConneX";

    /// <summary>
    /// Gets the plugin's description
    /// </summary>
    public string Description => "Integration with Data I/O ConneX";

    /// <summary>
    /// Gets the unique slug to use in the URL
    /// </summary>
    public string PluginSlug => "connex";

    /// <summary>
    /// Gets the UI type to use for the plugin
    /// </summary>
    public Type Type { get; set; }
}