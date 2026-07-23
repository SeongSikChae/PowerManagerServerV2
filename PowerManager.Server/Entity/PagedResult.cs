namespace PowerManager.Server.Entity
{
    public sealed class PagedResult<T>
    {
        public List<T> Items { get; set; } = null!;

        public Pagination Pagination { get; set; } = null!;
    }
}
