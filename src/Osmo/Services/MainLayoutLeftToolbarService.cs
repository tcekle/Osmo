namespace Osmo.Services;

public class MainLayoutLeftToolbarService
{
    public event Func<Type, Task>? OnSwitchToolbar;
    
    public async Task SwitchToolbar(Type type)
    {
        if (OnSwitchToolbar != null)
        {
            await OnSwitchToolbar.Invoke(type);
        }
    }
}