using MediatR;

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
}