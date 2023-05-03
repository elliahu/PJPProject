using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJPProject
{
    public class VirtualMachine
    {
        private Stack<(PrimitiveType type, object value)> _stack = new();
        private List<string> _code = new();

        public VirtualMachine(string code)
        {
            this._code = code.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public void Run()
        {
            foreach (var instruction in _code) 
            { 
                Console.WriteLine(instruction);
            }
        }
    }
}
