namespace NLP_Prototype_MVC_8._0.Models.ViewModels
{
    public class TopicDetailViewModel
    {
        public string Title { get; set; } = string.Empty;
        public ICollection<Question> Questions { get; set; } = [];
        public ICollection<Document> Documents { get; set; } = [];

    }
}
