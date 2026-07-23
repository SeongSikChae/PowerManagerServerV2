namespace PowerManager.Server.Entity
{
    public record RestResult(bool Error, string? Code, string? ErrorMessage)
    {
        public static RestResult Success()
        {
            return new RestResult(false, null, null);
        }

        public static RestResult Fail(string code, string errorMessage)
        {
            return new RestResult(true, code, errorMessage);
        }
    }
}
