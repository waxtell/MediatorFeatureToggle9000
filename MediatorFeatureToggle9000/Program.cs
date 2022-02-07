using MediatorFeatureToggle9000;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

const string Feature = "FEATURE";
const string Control = "CONTROL";

var serviceCollection = new ServiceCollection();
serviceCollection.AddMediatR(typeof(Program).Assembly);
serviceCollection.AddNamedTransient<string, IRequestHandler<GetStuffQuery, int>, GetStuffQueryHandler>(Feature);
serviceCollection.AddNamedTransient<string, IRequestHandler<GetStuffQuery, int>, GetStuffQueryHandler2>(Control);

var provider = serviceCollection.BuildServiceProvider();

MediatorExtensions
    .AddFeatureToggleHandler<string,GetStuffQuery,int>
    (
        key => provider.GetService<string,IRequestHandler<GetStuffQuery,int>>(key),
        async (request, _) => await Task.FromResult(request.CustomerId % 2 == 0 ? Feature : Control)
    );

var mediator = provider.GetService<IMediator>();

for (var i = 0; i < 10; i++)
{
    var result = await
                    mediator
                        .Send
                        (
                            new GetStuffQuery(i),
                            CancellationToken.None
                        );

    Console.WriteLine(result);
}

namespace MediatorFeatureToggle9000
{
    public class GetStuffQuery : IRequest<int>
    {
        public int CustomerId { get; }

        public GetStuffQuery(int customerId)
        {
            CustomerId = customerId;
        }
    }

    public class GetStuffQueryHandler : IRequestHandler<GetStuffQuery,int>
    {
        public async Task<int> Handle(GetStuffQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(1);
        }
    }

    public class GetStuffQueryHandler2 : IRequestHandler<GetStuffQuery, int>
    {
        public async Task<int> Handle(GetStuffQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(2);
        }
    }
}