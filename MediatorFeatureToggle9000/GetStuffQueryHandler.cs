using MediatR;

namespace MediatorFeatureToggle9000;

public class GetStuffQueryHandler : IRequestHandler<GetStuffQuery,int>
{
    public async Task<int> Handle(GetStuffQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(1);
    }
}