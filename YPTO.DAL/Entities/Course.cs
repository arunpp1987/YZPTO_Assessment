namespace YPTO.DAL.Entities
{
    using System.ComponentModel.DataAnnotations;
   // using YPTO_POC.Model;

    public class Course
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(200)]
        [Required]
        public string Title { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(10)]
        [Required]
        public string Status { get; set; }
        public ICollection<Training> Trainings { get; set; }
    }
}
