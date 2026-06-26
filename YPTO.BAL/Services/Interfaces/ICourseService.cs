using YPTO.DAL.Entities;
using YPTO.BAL.DTOs;
namespace YPTO.BAL.Services.Interfaces
{
    public interface ICourseService
    {
        Task<Course> GetCourseByCode(string courseCode);
    }
}
