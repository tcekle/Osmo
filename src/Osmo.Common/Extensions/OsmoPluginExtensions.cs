using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osmo.Common.Plugins;

namespace Osmo.Common.Extensions;

public static class OsmoPluginExtensions
{
    public static void AddOsmoPlugins(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddBuiltInPlugins();
    }

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
            //serviceCollection.TryAddTransient(typeof(IOsmoPlugin), pluginType);
        }
    }
}