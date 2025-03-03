namespace Osmo.Common.Services;

/// <summary>
/// Service to interact with the GoldenLayout javascript library.
/// </summary>
public class GoldenLayoutService
{
    /// <summary>
    /// Event to add a component to the GoldenLayout.
    /// </summary>
    public event Func<string, string, object, Task> OnAddComponent;

    /// <summary>
    /// Add a component to the GoldenLayout.
    /// </summary>
    /// <param name="component">The name of the component to add.</param>
    /// <param name="title"></param>
    /// <param name="parameters"></param>
    public async Task AddComponent(string component, string title, object parameters = null)
    {
        if (OnAddComponent != null)
        {
            await OnAddComponent.Invoke(component, title, parameters);
        }
    }
}