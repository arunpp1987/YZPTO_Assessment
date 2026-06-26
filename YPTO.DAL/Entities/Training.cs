namespace YPTO.DAL.Entities
{
    using System.ComponentModel.DataAnnotations;
    //using YPTO_POC.Model;

    public class Training
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(50)]
        [Required]
        public string Code { get; set; }
        [StringLength(10)]
        public string Month { get; set; }
        [StringLength(10)]
        [Required]
        public string Status { get; set; }
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
