using YPTO.BAL.Services.Interfaces;
using YPTO.DAL.Entities;
using YPTO.DAL.Data;
using YPTO.BAL.DTOs;
using YPTO.BAL.Utitlities;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace YPTO.BAL.Services
{
    public class CourseService : ICourseService
    {
        private readonly YptoDBContext _dbContext;

        public CourseService(YptoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Course> GetCourseByCode(string courseCode)
        {
            var course = await _dbContext.Courses
                            .FirstOrDefaultAsync(c => c.Code == courseCode);


            if (course == null)
            {
                throw new KeyNotFoundException(
                    $"Course with code {courseCode} is not found.");
            }

            if (!Enum.TryParse<CourseStatus>(
                    course.Status,
                    true,
                    out var status))
            {
                throw new InvalidOperationException(
                    "Invalid course status.");
            }

            if (status != CourseStatus.Active)
            {
                throw new InvalidOperationException(
                    $"Course {courseCode} is inactive.");
            }

            return course;
        }
    }
}
