using System.ComponentModel.DataAnnotations;

namespace NLP_Prototype_MVC_8._0.Models
{
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AnswerId { get; set; }
        public StudentAnswer Answer { get; set; } = null!;

        [Required]
        public string Score { get; set; } = string.Empty;

        public string Feedback { get; set; } = string.Empty;

        [Required]
        public DateTime EvaluatedAt { get; set; } = DateTime.Now;
    }
}
