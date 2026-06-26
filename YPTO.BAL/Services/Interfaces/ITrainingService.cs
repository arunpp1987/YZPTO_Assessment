using YPTO.DAL.Entities;
using YPTO.BAL.DTOs;
namespace YPTO.BAL.Services.Interfaces
{
    public interface ITrainingService
    {
        Task<CreateTrainingResponseDto> CreateTraining(CreateTrainingRequestDto request);
        Task<CreateTrainingResponseDto> UpdateTraining(int id, CreateTrainingRequestDto request);
        Task<IEnumerable<CreateTrainingResponseDto>> GetAllTrainings();
        Task<CreateTrainingResponseDto> GetTrainingDetailsById(int id);
    }
}
