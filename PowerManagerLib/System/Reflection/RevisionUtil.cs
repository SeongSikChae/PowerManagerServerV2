namespace System.Reflection
{
    public static class RevisionUtil
    {
        public static string GetRevisoin<T>() where T : Attribute, IRevisionAttribute
        {
            T? t = typeof(T).Assembly.GetCustomAttribute<T>();
            if (t is not null)
                return t.Revision;
            return string.Empty;
        }
    }
}