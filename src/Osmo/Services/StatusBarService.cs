using Osmo.Common.Ui;

namespace Osmo.Services;

public class StatusBarService : IStatusBar
{
    private readonly HashSet<Type> _rightComponents = new();
    
    public IEnumerable<Type> RightComponents => _rightComponents;

    public void AddRightStatusBarComponent(Type componentType) => _rightComponents.Add(componentType);
}