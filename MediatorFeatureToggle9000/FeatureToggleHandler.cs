using MediatR;
using MediatR.Wrappers;

namespace MediatorFeatureToggle9000;

public class FeatureToggleHandler<TRequest, TResponse, TKey> : RequestHandlerWrapperImpl<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Func<TKey, IRequestHandler<TRequest, TResponse>> _handlerResolver;
    private readonly Func<TRequest, CancellationToken, Task<TKey>> _keyProvider;

    public FeatureToggleHandler(Func<TKey, IRequestHandler<TRequest, TResponse>> handlerResolver, Func<TRequest, CancellationToken, Task<TKey>> keyProvider)
    {
        _handlerResolver = handlerResolver;
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

        IRequestHandler<TRequest,TResponse> GetTheHandler(TKey key)
        {
            return
                _handlerResolver
                    .Invoke(key);
        }

        async Task<TResponse> Handler() => await GetTheHandler(await GetKey()).Handle((TRequest)request, cancellationToken);

        return 
            serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>)Handler, (next, pipeline) => () => pipeline.Handle((TRequest)request, cancellationToken, next))();
    }
}