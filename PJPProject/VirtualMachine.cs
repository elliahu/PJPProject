using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJPProject
{
    public class VirtualMachine
    {
        public class Instruction
        {
            public static string Add => "ADD";
            public static string Sub => "SUB";
            public static string Mul => "MUL";
            public static string Div => "DIV";
            public static string Mod => "MOD";
            public static string Uminus => "UMINUS";
            public static string Concat => "CONCAT";
            public static string And => "AND";
            public static string Or => "OR";
            public static string Gt => "GT";
            public static string Lt => "LT";
            public static string Eq => "EQ";
            public static string Not => "NOT";
            public static string Itof => "ITOF";
            public static string Push((PrimitiveType type, object value) symbol)
            {
                return $"PUSH {Enum.GetName(symbol.type)![0]} {symbol.value}";
            }
            public static string Pop => "POP";
            public static string Load(string id) => $"LOAD {id}";
            public static string Save(string id) => $"SAVE {id}";
            public static string Label(int n) => $"LABEL {n}";
            public static string Jmp(int n) => $"JMP {n}";
            public static string Fjmp(int n) => $"FJMP {n}";
            public static string Print(int n) => $"PRINT {n}";
            public static string Read(PrimitiveType t) => $"READ {Enum.GetName(t)![0]}";
        }

        public class InterpreterException: Exception 
        {
            public InterpreterException(string msg) : base(msg) { }
        }

        private Stack<(PrimitiveType type, object value)> _stack = new();
        private List<string> _code = new();
        public static SymbolTable SymbolTable = new();

        public VirtualMachine(string code)
        {
            this._code = code.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public void DumpCode() { Console.WriteLine(String.Join("\n", this._code)); }

        public void Run()
        {
            try
            {
                foreach (var instruction in _code)
                {
                    // TODO if there is something like PUSH S "a a", it will not work
                    var parts = instruction.Split(" ");

                    if (instruction.StartsWith("ADD")) Add();
                    else if (instruction.StartsWith("SUB")) Sub();
                    else if (instruction.StartsWith("MUL")) Mul();
                    else if (instruction.StartsWith("DIV")) Div();
                    else if (instruction.StartsWith("MOD")) Mod();
                    else if (instruction.StartsWith("UMINUS")) Uminus();
                    else if (instruction.StartsWith("CONCAT")) Concat();
                    else if (instruction.StartsWith("AND")) And();
                    else if (instruction.StartsWith("OR")) Or();
                    else if (instruction.StartsWith("GT")) Gt();
                    else if (instruction.StartsWith("LT")) Lt();
                    else if (instruction.StartsWith("EQ")) Eq();
                    else if (instruction.StartsWith("NOT")) Not();
                    else if (instruction.StartsWith("ITOF")) Itof();
                    else if (instruction.StartsWith("PUSH"))
                    {
                        Push(TypeChecker.CharToType(parts[1]),
                            TypeChecker.StringToObj(
                                TypeChecker.CharToType(parts[1]),
                                parts[2])
                            );
                    }
                    else if (instruction.StartsWith("POP")) Pop();
                    else if (instruction.StartsWith("LOAD")) Load(parts[1]);
                    else if (instruction.StartsWith("SAVE")) Save(parts[1]);
                    else if (instruction.StartsWith("LABEL")) Label();
                    else if (instruction.StartsWith("JMP")) Jmp();
                    else if (instruction.StartsWith("FJMP")) Fjmp();
                    else if (instruction.StartsWith("PRINT")) Print(int.Parse(parts[1]));
                    else if (instruction.StartsWith("READ")) Read();
                }
            }
            catch(InterpreterException iex)
            {
                var c = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Interpreter encountered the following Error:");
                Console.WriteLine(iex.Message);
                Console.ForegroundColor = c;
            }
            /*catch(Exception ex)
            {
                var c = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("VirtualMachine encountered the following Exception:");
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = c;
            }*/
        }

        private void Add()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Calc(val1,val2,TypeChecker.Operation.Add);
            Push(type, value);
        }
        private void Sub()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Calc(val1, val2, TypeChecker.Operation.Subtract);
            Push(type, value);
        }
        private void Mul()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Calc(val1, val2, TypeChecker.Operation.Multiply);
            Push(type, value);
        }
        private void Div()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Calc(val1, val2, TypeChecker.Operation.Divide);
            Push(type, value);
        }
        private void Mod()
        {

        }
        private void Uminus()
        {

        }
        private void Concat()
        {

        }
        private void And()
        {

        } 
        private void Or()
        {

        }
        private void Gt()
        {

        }
        private void Lt()
        {

        }
        private void Eq()
        {

        }
        private void Not()
        {

        }
        private void Itof()
        {

        }
        private void Push(PrimitiveType type, object value)
        {
            _stack.Push((type,value));
        }
        private (PrimitiveType type, object value) Pop()
        {
            return _stack.Pop();
        }
        private void Load(string id)
        {
            var v = SymbolTable.GetVariable(id);
            if(v.type != PrimitiveType.Error)
            {
                Push(v.type, v.value);
            }
            else
            {
                throw new InterpreterException($"Could not load variable {id}. No such variable exists.");
            }
        }
        private void Save(string id)
        {
            if (SymbolTable.Contains(id))
            {
                var existing = SymbolTable.GetVariable(id);
                var asigned = Pop();
                if (TypeChecker.CanCast(asigned.type, existing.type))
                {
                    SymbolTable.SetVariable(id, (TypeChecker.TypeResult(asigned.type, existing.type), asigned.value));
                }
                else
                {
                    throw new InterpreterException($"Invalid type conversion from {Enum.GetName(asigned.type)} to {Enum.GetName(existing.type)}");
                }
            }
            else
            {
                SymbolTable.SetVariable(id, Pop());
            }
            
        }
        private void Label()
        {

        }
        private void Jmp()
        {

        }
        private void Fjmp()
        {

        }
        private void Print(int n)
        {
            PopN(n).ForEach((p) => { 
                Console.WriteLine(p);
            });
        }
        private void Read()
        {

        }

        private ((PrimitiveType type, object value), (PrimitiveType type, object value)) DoublePop()
        {
            return (Pop(),Pop());
        }

        private List<(PrimitiveType type, object value)> PopN(int n)
        {
            List<(PrimitiveType type, object value)> popped = new();
            for(int i = 0; i < n; i++)
            {
                popped.Add(Pop());
            }
            return popped;
        }
    }
}
