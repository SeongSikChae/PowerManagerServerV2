namespace PowerManager.Server.Entity
{
    public class SearchQuery
    {
        public string? Query { get; set; }

        public Pagination Pagination { get; set; } = null!;
    }
}
