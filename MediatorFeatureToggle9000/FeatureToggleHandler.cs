using MediatR;
using MediatR.Wrappers;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorFeatureToggle9000;

public class FeatureToggleHandler<TRequest, TResponse, TKey> : RequestHandlerWrapperImpl<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Func<TRequest, CancellationToken, Task<TKey>> _keyProvider;

    public FeatureToggleHandler(IServiceProvider provider, Func<TRequest, CancellationToken, Task<TKey>> keyProvider)
    {
        _serviceProvider = provider;
        _keyProvider = keyProvider;
    }

    public override Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken,
        ServiceFactory serviceFactory)
    {
        async Task<TKey> GetKey()
        {
            return 
                await
                    _keyProvider
                        .Invoke((TRequest)request, cancellationToken);
        }

        THandler GetTheHandler<THandler>(TKey key)
        {
            return 
                _serviceProvider
                    .GetService<TKey, THandler>(key);
        }

        async Task<TResponse> Handler() => await GetTheHandler<IRequestHandler<TRequest, TResponse>>(await GetKey()).Handle((TRequest)request, cancellationToken);

        return 
            serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>)Handler, (next, pipeline) => () => pipeline.Handle((TRequest)request, cancellationToken, next))();
    }
}