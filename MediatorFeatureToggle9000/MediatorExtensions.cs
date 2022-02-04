using System.Collections.Concurrent;
using System.Reflection;
using MediatR;
using MediatR.Wrappers;

namespace MediatorFeatureToggle9000;

public static class MediatorExtensions
{
    // Ideally the following method could be exposed in Mediator.cs
    //public static void AddFeatureToggleHandler(Type type, RequestHandlerBase handler)
    //{
    //    _requestHandlers[type] = handler;
    //}

    public static void AddFeatureToggleHandler(Type type, RequestHandlerBase handler)
    {
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        if 
        (
            typeof(Mediator)
                .GetField
                (
                    "_requestHandlers",
                    BindingFlags.Static | BindingFlags.NonPublic
                )
                ?.GetValue(null) is not ConcurrentDictionary<Type, RequestHandlerBase> requestHandlers
        )
        {
            throw 
                new InvalidOperationException("Can't access Mediator request handlers collection!");
        }
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

        requestHandlers[type] = handler;
    }
}