using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YPTO.BAL.Services.Interfaces;
using YPTO.DAL.Entities;
using YPTO.DAL.Data;
using YPTO.BAL.DTOs;
using YPTO.BAL.Utitlities;

namespace YPTO.BAL.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly YptoDBContext _dbContext;
        private readonly ICourseService _courseService;
        public TrainingService(YptoDBContext dbContext,ICourseService courseService)
        {
                _dbContext = dbContext;
            _courseService = courseService;
        }
        public async Task<CreateTrainingResponseDto> CreateTraining(CreateTrainingRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Training request cannot be null.");
            }

            try
            {
                var course = await _courseService.GetCourseByCode(request.CourseCode);

                var training = new Training
                {
                    Name = request.Name,
                    Code = request.Code,
                    CourseId = course.Id,
                    Month = request.Month,
                    Status = request.Status
                };

                await _dbContext.Trainings.AddAsync(training);
                await _dbContext.SaveChangesAsync();

                var traingResponse = new CreateTrainingResponseDto
                {
                    Name = training.Name,
                    Code = training.Code,
                    Course = course.Name,
                    Month = training.Month,
                    Status = training.Status
                };
                return traingResponse;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("A database error occurred while creating the training.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while creating the training.", ex);
            }
        }

        public async Task<IEnumerable<CreateTrainingResponseDto>> GetAllTrainings()
        {
            try
            {
                var trainings = await _dbContext.Trainings.Include(t => t.Course).ToListAsync();

                return trainings.Select(t => new CreateTrainingResponseDto
                {                  
                    Name = t.Name,
                    Code = t.Code,
                    Course = t.Course.Name,
                    Month = t.Month,
                    Status = t.Status
                });
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving trainings.", ex);
            }
        }

        public async Task<CreateTrainingResponseDto> UpdateTraining(int id, CreateTrainingRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Training request cannot be null.");
            }

            var training = await _dbContext.Trainings.FindAsync(id);

            if (training == null)
            {
                throw new KeyNotFoundException($"Training with Id {id} was not found.");
            }
            var course = await _courseService.GetCourseByCode(request.CourseCode);

            training.Name = request.Name;
            training.Code = request.Code;
            training.CourseId = course.Id;
            training.Month = request.Month;
            training.Status = request.Status;

            try
            {
                await _dbContext.SaveChangesAsync();
                var traingResponse = new CreateTrainingResponseDto
                {
                    Name = training.Name,
                    Code = training.Code,
                    Course = training.Course.Name,
                    Month = training.Month,
                    Status = training.Status
                };

                return traingResponse;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("A database error occurred while updating the training.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating the training.", ex);
            }
        }

        public async Task<CreateTrainingResponseDto> GetTrainingDetailsById(int id)
        {
            if(id <= 0)
            {
                throw new KeyNotFoundException($"Training with Id {id} is not valid.");
            }

            var training = await _dbContext.Trainings
                .Include(t => t.Course)
                .Where(t => t.Id == id).Select(t => new CreateTrainingResponseDto
                           {                              
                               Code = t.Code,
                               Name = t.Name,
                               Month = t.Month,
                               Status = t.Status,                              
                               Course = t.Course.Name
                           }).FirstOrDefaultAsync();


            if (training == null)
            {
                throw new KeyNotFoundException(
                    $"Training with Id {id} is not found.");
            }
            if (!Enum.TryParse<TrainingStatus>(
                  training.Status,
                  true,
                  out var status))
            {
                throw new InvalidOperationException(
                    "Invalid Training status.");
            }
            if (status != TrainingStatus.Open)
            {
                throw new InvalidOperationException(
                    $"The Training Id {id} is not available to use.");
            }
            
            return training;
        }


    }
}
