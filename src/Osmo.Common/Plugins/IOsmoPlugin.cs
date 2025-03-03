namespace Osmo.Common.Plugins;

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

    // /// <summary>
    // /// Gets a new instance of the plugin's tab
    // /// </summary>
    // /// <returns>A new instance of the plugin's tab</returns>
    // Task<DisplayTab> GetNewTabInstance();
}