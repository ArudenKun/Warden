using Volo.Abp.DependencyInjection;
using ZLinq;

namespace Warden.Core;

public sealed class ViewModelConventionalRegistrar : DefaultConventionalRegistrar
{
    protected override bool IsConventionalRegistrationDisabled(Type type) =>
        !type.IsAssignableTo<ViewModelBase>() || base.IsConventionalRegistrationDisabled(type);

    protected override List<Type> GetExposedServiceTypes(Type type)
    {
        var exposedServiceTypes = base.GetExposedServiceTypes(type).AsValueEnumerable();
        var viewModelBaseClasses = type.GetBaseClasses(typeof(ViewModelBase))
            .AsValueEnumerable()
            .Where(x => x.IsAssignableTo<ViewModelBase>());
        return exposedServiceTypes.Union(viewModelBaseClasses).Distinct().ToList();
    }
}
