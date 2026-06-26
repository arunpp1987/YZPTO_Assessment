namespace YPTO.BAL.DTOs
{
    public class SubscriptionFilterRequestDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? TrainingId { get; set; }
        public int? CourseId { get; set; }
        public string? Month { get; set; }
        public int? UserId { get; set; }       

    }
}
