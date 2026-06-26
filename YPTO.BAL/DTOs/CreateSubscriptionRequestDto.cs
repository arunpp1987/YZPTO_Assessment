namespace YPTO.BAL.DTOs
{
    public class CreateSubscriptionRequestDto
    {  
        public string SubscriptionCode { get; set; }

        public int TrainingId { get; set; }

        public int UserId { get; set; }
       
    }
}
