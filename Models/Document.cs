using System.ComponentModel.DataAnnotations;

namespace NLP_Prototype_MVC_8._0.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int TopicId { get; set; }
        public Topic Topic { get; set; } = null!;


        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
