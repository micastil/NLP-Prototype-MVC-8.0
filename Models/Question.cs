using System.ComponentModel.DataAnnotations;

namespace NLP_Prototype_MVC_8._0.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public int TopicId { get; set; }
        public Topic Topic { get; set; } = null!;

        public ICollection<StudentAnswer> Answers { get; set; } = [];
    }
}
