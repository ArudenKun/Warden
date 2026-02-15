using Autofac;
using Autofac.Core;
using Autofac.Core.Resolving.Pipeline;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Warden.Dependency.Middlewares;

namespace Warden.Dependency;

public static class ServiceCollectionExtensions
{
    public static AutofacServiceProvider BuildAutofacServiceProvider(
        this IServiceCollection services
    )
    {
        var builder = new ContainerBuilder();
        builder.ComponentRegistryBuilder.Registered += ComponentRegistryBuilderOnRegistered;
        builder.Populate(services);
        return new AutofacServiceProvider(builder.Build());
    }

    private static void ComponentRegistryBuilderOnRegistered(
        object? sender,
        ComponentRegisteredEventArgs e
    )
    {
        e.ComponentRegistration.PipelineBuilding += ComponentRegistrationOnPipelineBuilding;
    }

    private static void ComponentRegistrationOnPipelineBuilding(
        object? sender,
        IResolvePipelineBuilder e
    )
    {
        e.Use(new InitializerMiddleware(), MiddlewareInsertionMode.StartOfPhase);
        e.Use(new MessengerRecipientMiddleware());
        e.Use(new MessengerRequestMiddleware());
    }
}
