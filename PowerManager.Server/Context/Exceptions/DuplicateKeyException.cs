namespace PowerManager.Server.Context.Exceptions
{
    public class DuplicateKeyException(string deviceId, Exception? innerException) : Exception($"'{deviceId}'가 이미 있습니다.", innerException)
    {
    }
}
