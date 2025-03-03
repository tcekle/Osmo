using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Osmo.Common.Extensions;

using Plugins;

/// <summary>
/// Extension methods for adding Osmo plugins to the service collection.
/// </summary>
public static class OsmoPluginExtensions
{
    /// <summary>
    /// Adds all Osmo plugins to the service collection.
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void AddOsmoPlugins(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddBuiltInPlugins();
    }

    /// <summary>
    /// Adds all built-in Osmo plugins to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the plugins to.</param>
    private static void AddBuiltInPlugins(this IServiceCollection serviceCollection)
    {
        List<Type> pluginTypes = new();
        
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.GetName().Name?.StartsWith("Osmo") ?? false))
        {
            var embeddedTypes = assembly.GetTypes().Where(
                type =>
                    type.IsClass &&
                    type.IsPublic &&
                    !type.IsAbstract &&
                    typeof(IOsmoPlugin).IsAssignableFrom(type));
            
            pluginTypes.AddRange(embeddedTypes);
        }

        foreach (var pluginType in pluginTypes)
        {
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IOsmoPlugin), pluginType));
        }
    }
}