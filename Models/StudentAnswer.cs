using System.ComponentModel.DataAnnotations;

namespace NLP_Prototype_MVC_8._0.Models
{
    public class StudentAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        [Required]
        public string AnswerText { get; set; } = string.Empty;

        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public Evaluation Evaluation { get; set; } = null!;
    }
}
