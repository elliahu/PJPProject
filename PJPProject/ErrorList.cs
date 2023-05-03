using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJPProject
{
    public class ErrorList
    {
        private static readonly List<string> _errors = new List<string>();
        public static int NumberOfErrors => _errors.Count;

        public static void Report(IToken token, string message)
        {
            _errors.Add($"{token.Line}:{token.Column} {message}");
        }

        public static void Dump()
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var error in _errors)
            {
                Console.WriteLine(error);
            }
            _errors.Clear();
            Console.ForegroundColor = c;
        }
    }
}
