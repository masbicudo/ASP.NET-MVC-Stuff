using System;
using System.Runtime.InteropServices;

namespace ConsoleApplication
{
    class Program
    {
        [StructLayout(LayoutKind.Explicit, Size = 0, Pack = 0, CharSet = CharSet.Ansi)]
        struct VOID { }

        static unsafe void Main(string[] args)
        {
            var a = !(bool?)null;

            var b = a;

            Console.WriteLine(string.Format("{0} = {1}", "sizeof(VOID)", sizeof(VOID)));
            foreach (var eachAttr in typeof(VOID).GetCustomAttributes(true))
                Console.WriteLine(string.Format("{0} = {1}", "eachAttr.GetType().Name", eachAttr.GetType().Name));
            Console.ReadKey();
        }
    }
}
