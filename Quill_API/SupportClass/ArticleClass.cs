namespace Quill_API.SupportClass
{
    public class ArticleClass
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime? PublishedAt { get; set; }

        public string? Status { get; set; }

        public int AuthorId { get; set; }

        public int IdTopics { get; set; }
        public string? Image { get; set; }
    }
}
