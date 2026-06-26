using YPTO.BAL.Services.Interfaces;
using YPTO.DAL.Entities;
using YPTO.DAL.Data;
using YPTO.BAL.DTOs;
using YPTO.BAL.Utitlities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace YPTO.BAL.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly YptoDBContext _dbContext;
        private readonly ICourseService _courseService;
        private readonly ITrainingService _trainingService;
        private readonly IUserService _userService;
        public SubscriptionService(YptoDBContext dbContext, ICourseService courseService, ITrainingService trainingService, IUserService userService) {
            _dbContext = dbContext;
            _trainingService = trainingService;
            _courseService = courseService;
            _userService = userService;
        }
        public async Task<CreateSubscriptionResponseDto> CreateSubscription(CreateSubscriptionRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Subscription request cannot be null.");
            }

            try
            {
                var user = await _userService.GetUserById(request.UserId);

                if (user == null)
                    throw new Exception("The User is not found");

                var training = await _trainingService.GetTrainingDetailsById(request.TrainingId);

                if (training == null)
                    throw new Exception("Training is not found");

                // Validate the User, Training & subscription against a user
                await ValidateSubscription(request.UserId, request.TrainingId, user, training);              

                var subscription = new Subscription
                {                   
                   
                   SubscriptionCode = request.SubscriptionCode,
                   TrainingId = request.TrainingId,
                   UserId = request.UserId,                    
                   CreatedDate = DateTime.UtcNow                   
                };

                await _dbContext.Subscriptions.AddAsync(subscription);
                await _dbContext.SaveChangesAsync();

                var subcriptionResponse = new CreateSubscriptionResponseDto
                {
                    SubscriptionCode = request.SubscriptionCode,
                    TrainingCode = training.Code,
                    TrainingName = training.Name,
                    Course = training.Course,
                    TrainingMonth = training.Month,
                    TrainingStatus = training.Status,
                    UserName = user.Name
                };

                return subcriptionResponse;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("A database error occurred while creating the subscription.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while creating the subscription.", ex);
            }
        }

        public async Task<PagedResponseDto<SubscriptionDetailsDto>> GetAllSubscriptions (SubscriptionFilterRequestDto request)
        {
            request.Page = (request.Page <= 0) ? 1 : request.Page;
            request.PageSize = (request.PageSize < 1) ? 5 : request.PageSize; // we can make it a configurable value

            // Dynamically generating the filter properties
            var query = _dbContext.Subscriptions.AsNoTracking()
                .Include(x => x.Training)
                .ThenInclude(t => t.Course)
                .AsQueryable();

            if (request.TrainingId > 0)
            {
                query = query.Where(t =>
                    t.TrainingId == request.TrainingId.Value);
            }
            if (!string.IsNullOrEmpty(request.Month))
            {
                query = query.Where(x =>
                    x.Training.Month == request.Month);
            }

            if (request.CourseId > 0)
            {
                query = query.Where(t =>
                    t.Training.Course.Id == request.CourseId.Value);
            }

            if (request.UserId > 0)
            {
                query = query.Where(x =>
                    x.UserId == request.UserId.Value);
            }

            var sql = query.ToQueryString();

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new SubscriptionDetailsDto
                {                   
                    TrainingCode = x.Training.Code,
                    TrainingName = x.Training.Name,                   
                    CourseCode = x.Training.Course.Code,
                    CourseName = x.Training.Course.Name
                    //UserEmail = x.UserId,
                    //UserName = x.UserName,                    
                })
                .ToListAsync();

            return new PagedResponseDto<SubscriptionDetailsDto>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = totalRecords,
                Data = data
            };
        }

        //public async Task<SubscriptionDetailDto> GetSubscriptionByCode(string code)

        //{         
        //    return null
        //}
        //public async Task<CreateSubscriptionResponseDto> GetAllSubscriptions()
        //{

        //    try
        //    {
        //        var subscriptions = await _dbContext.Subscription
        //            .Include(s => s.Course).
        //            Include(s => s.Training).
        //            Include(s => s.User).ToListAsync();

        //        return subscriptions.Select(s => new CreateSubscriptionResponseDto
        //        {                    
        //            SubscriptionCode = s.Code,
        //            TrainingName = s.Training.Name,
        //            Course = s.Course.Name,
        //            Month = s.Training.Month

        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while retrieving subscriptions.", ex);
        //    }
        //}
        #region private methods
        /// <summary>
        /// Check if candidate is already subscribed anz other training in the same month
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="trainingId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>    
        private async Task ValidateSubscription(int userId, int trainingId, UserDto user, CreateTrainingResponseDto training)
        {

            if (!Enum.TryParse<UserStatus>(
                   user.Status,
                   true,
                   out var userStatus))
            {
                throw new InvalidOperationException(
                    "Invalid user status.");
            }

            if (userStatus != UserStatus.Active)
            {
                throw new InvalidOperationException(
                    $"The user {user.Name} is inactive.");
            }


            if (!Enum.TryParse<TrainingStatus>(
                  training.Status,
                  true,
                  out var trainingStatus))
            {
                throw new InvalidOperationException(
                    "Invalid user status.");
            }

            if (trainingStatus != TrainingStatus.Open)
            {
                throw new InvalidOperationException(
                    $"The Training {training.Name} is on Hold / Expired.");
            }

            var isSubscribed = await _dbContext.Subscriptions
                .AnyAsync(s =>
                    s.UserId == userId &&
                    s.Training.Month == training.Month);

            if (isSubscribed)
            {
                throw new Exception(
                    "Candidate has already subscribed an another training in the same month.");
            }
        }
        #endregion
    }
}