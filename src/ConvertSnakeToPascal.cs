using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConverterOfNspecToXunit
{
    public class ConvertSnakeToPascal
    {
        public static string Convert(string oldName)
        {
            do
            {
                if (oldName.ToLower().Equals("q")) break;

                if (oldName.Substring(0,1) == "_") break;

                var converted = oldName.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries).Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1)).Aggregate(string.Empty, (s1, s2) => s1 + s2);

                return converted;

            } while (true);

            Console.WriteLine("\nNome antigo mantido: ");

            return oldName;

        }

    }
}
