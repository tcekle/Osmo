namespace Osmo.Common.Services;

public class GoldenLayoutService
{
    public event Func<string, string, object, Task>? OnAddRoute;

    public async Task AddRoute(string route, string title, object parameters = null)
    {
        if (OnAddRoute != null)
        {
            await OnAddRoute.Invoke(route, title, parameters);
        }
    }
}