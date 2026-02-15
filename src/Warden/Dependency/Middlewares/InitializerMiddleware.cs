using Autofac.Core.Resolving.Pipeline;

namespace Warden.Dependency.Middlewares;

public sealed class InitializerMiddleware : IResolveMiddleware
{
    public void Execute(ResolveRequestContext context, Action<ResolveRequestContext> next)
    {
        next(context);
        if (!context.NewInstanceActivated)
            return;

        var instance = context.Instance!;
        if (instance is IInitializer initializer)
        {
            initializer.Initialize();
        }
    }

    public PipelinePhase Phase => PipelinePhase.Activation;
}
