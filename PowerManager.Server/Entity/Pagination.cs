namespace PowerManager.Server.Entity
{
    public sealed class Pagination
    {
        public int CurrentPage { get; set; }

        public int ItemSize { get; set; }

        public int TotalCount { get; set; }
    }
}
