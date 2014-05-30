
namespace MvcStuff
{
    /// <summary>
    /// Representa a private collection key, in collections that accept objects as keys, such as 'HttpContext.Items'.
    /// </summary>
    public class CustomKey
    {
        private readonly string display;

        public CustomKey()
        {
        }

        public CustomKey(string display)
        {
            this.display = display;
        }

        public override string ToString()
        {
            return this.display ?? this.GetType().Name;
        }
    }
}