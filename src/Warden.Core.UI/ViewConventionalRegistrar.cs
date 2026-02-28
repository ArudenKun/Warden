using Avalonia.Controls;
using Volo.Abp.DependencyInjection;

namespace Warden.Core;

public sealed class ViewConventionalRegistrar : DefaultConventionalRegistrar
{
    protected override bool IsConventionalRegistrationDisabled(Type type) =>
        !type.GetInterfaces()
            .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IView<>))
            && type.IsAssignableTo<Control>()
        || base.IsConventionalRegistrationDisabled(type);

    protected override List<Type> GetExposedServiceTypes(Type type)
    {
        var exposedServiceTypes = base.GetExposedServiceTypes(type);
        var viewInterfaceTypes = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IView<>))
            .ToList();
        return exposedServiceTypes.Union(viewInterfaceTypes).Distinct().ToList();
    }
}
