using System.Reflection;

namespace PowerManagerLib
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class RevisionAttribute : Attribute, IRevisionAttribute
    {
        public RevisionAttribute(string revision)
        {
            Revision = revision;
        }

        public string Revision { get; }
    }
}
