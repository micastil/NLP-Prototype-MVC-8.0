using System.ComponentModel.DataAnnotations;

namespace NLP_Prototype_MVC_8._0.Models.ViewModels
{
    public class DocumentUploadViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; } = string.Empty;
    }
}
