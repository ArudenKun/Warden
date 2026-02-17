using Volo.Abp.DependencyInjection;
using ZLinq;

namespace Warden.ViewModels;

public sealed class ViewModelConventionalRegistrar : DefaultConventionalRegistrar
{
    protected override bool IsConventionalRegistrationDisabled(Type type) =>
        !type.GetBaseClasses().AsValueEnumerable().Any(x => x.IsAssignableTo<ViewModel>())
        || base.IsConventionalRegistrationDisabled(type);

    protected override List<Type> GetExposedServiceTypes(Type type)
    {
        var exposedServiceTypes = base.GetExposedServiceTypes(type).AsValueEnumerable();
        var viewModelBaseClasses = type.GetBaseClasses()
            .AsValueEnumerable()
            .Where(x => x.BaseType is not null && x.IsAssignableTo<ViewModel>());
        return exposedServiceTypes.Union(viewModelBaseClasses).Distinct().ToList();
    }
}
