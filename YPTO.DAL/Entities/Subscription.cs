namespace YPTO.DAL.Entities
{
    using System.ComponentModel.DataAnnotations;
    //using YPTO_POC.Model;

    public class Subscription
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string SubscriptionCode { get; set; }
        [Required]
        public int  TrainingId { get; set; }
        [Required]       
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Training Training { get; set; }
    }
}
