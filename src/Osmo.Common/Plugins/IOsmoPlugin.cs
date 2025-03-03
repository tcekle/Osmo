namespace Osmo.Common.Plugins;

/// <summary>
/// An interface representing an Osmo plugin
/// </summary>
public interface IOsmoPlugin
{
    /// <summary>
    /// Gets the name of the plugin
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the plugin's description
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the unique slug to use in the URL
    /// </summary>
    string PluginSlug { get; }

    /// <summary>
    /// Gets the UI type to use for the plugin
    /// </summary>
    Type Type { get; }
}