using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MvcStuff.SystemExtensions
{
    internal static class StringExtensions
    {
        public static IEnumerable<string> EnumerableSplit(
            [NotNull] this string str,
            char separator,
            StringSplitOptions options = StringSplitOptions.None)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return EnumerableSplitInternal(str, separator, options);
        }

        private static IEnumerable<string> EnumerableSplitInternal(
            [NotNull] string str,
            char separator,
            StringSplitOptions options)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            var prevPos = 0;
            while (true)
            {
                var nextPos = str.IndexOf(separator, prevPos);

                if (nextPos < 0)
                {
                    if (options != StringSplitOptions.RemoveEmptyEntries || prevPos < str.Length)
                        yield return str.Substring(prevPos);

                    yield break;
                }

                if (options != StringSplitOptions.RemoveEmptyEntries || nextPos != prevPos)
                    yield return str.Substring(prevPos, nextPos - prevPos);

                prevPos = nextPos + 1;
            }
        }
    }
}