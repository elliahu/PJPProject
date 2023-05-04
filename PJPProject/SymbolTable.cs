using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PJPProject
{
    public class SymbolTable
    {
        Dictionary<string, (PrimitiveType type, object value)> memory = new();

        public void Add(IToken variable, PrimitiveType type)
        {
            var name = variable.Text.Trim();
            if (memory.ContainsKey(name))
            {
                ErrorList.Report(variable, $"Redeclaration of variable '{name}'!");
                return;
            }

            if (type == PrimitiveType.Int)
                memory.Add(name, (type, 0));
            else if (type == PrimitiveType.Float)
                memory.Add(name, (type, (float)0));
        }

        public (PrimitiveType type, object value) this[IToken variable]
        {
            get
            {
                var name = variable.Text.Trim();
                if (memory.ContainsKey(name))
                {
                    return memory[name];
                }
                else
                {
                    ErrorList.Report(variable, $"Undeclared variable '{name}'!");
                    return (PrimitiveType.Error, -1);
                }
            }
            set
            {
                memory[variable.Text.Trim()] = value;
            }
        }

        public (PrimitiveType type, object value) GetVariable(string id)
        {
            if (memory.ContainsKey(id))
            {
                return memory[id];
            }
            else
            {
                return (PrimitiveType.Error, -1);
            }
        }

        public void SetVariable(string id, (PrimitiveType type , object value) variable)
        {
            memory[id] = variable;
        }

        public bool Contains(string id)
        {
            return memory.ContainsKey(id);
        }
    }
}
