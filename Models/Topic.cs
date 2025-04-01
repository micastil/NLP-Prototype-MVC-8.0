using System.ComponentModel.DataAnnotations;

namespace NLP_Prototype_MVC_8._0.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty; 

        public ICollection<Question> Questions { get; set; } = [];
    }
}
