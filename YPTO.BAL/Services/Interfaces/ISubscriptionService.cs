using YPTO.DAL.Entities;
using YPTO.BAL.DTOs;
namespace YPTO.BAL.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<CreateSubscriptionResponseDto> CreateSubscription(CreateSubscriptionRequestDto request);
        Task<PagedResponseDto<SubscriptionDetailsDto>> GetAllSubscriptions(SubscriptionFilterRequestDto request);
    }
}
