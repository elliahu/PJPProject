using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private Dictionary<int, int> _labels = new();

        public void ReadFile(string filename)
        {
            var sourceFile = File.ReadAllLines(filename);
            _code = new List<string>(sourceFile);
        }

        public void Dump() { Console.WriteLine(String.Join("\n", this._code)); }

        public void Run()
        {
            if(_code.Count == 0)
            {
                Logger.Log(LogLevel.ERROR, "No code to interpret. Did you foregt to call ReadFile()?", ConsoleColor.Red);
                return;
            }

            _stack.Clear();
            try
            {
                IndexLabels();
                for(int row = 0; row < _code.Count;row++)
                {
                    var instruction = _code[row];

                    var parts = Regex.Matches(instruction, @"[\""].+?[\""]|[^ ]+")
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToList();

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
                        if (parts[1] == "S" &&
                            parts.Count == 2)
                        {
                            Push(TypeChecker.CharToType(parts[1]),
                            TypeChecker.StringToObj(
                                TypeChecker.CharToType(parts[1]),
                                "")
                            );
                        }
                        else
                        {
                            Push(TypeChecker.CharToType(parts[1]),
                            TypeChecker.StringToObj(
                                TypeChecker.CharToType(parts[1]),
                                parts[2])
                            );
                        }
                    }
                    else if (instruction.StartsWith("POP")) Pop();
                    else if (instruction.StartsWith("LOAD")) Load(parts[1]);
                    else if (instruction.StartsWith("SAVE")) Save(parts[1]);
                    else if (instruction.StartsWith("LABEL")) Label();
                    else if (instruction.StartsWith("JMP")) Jmp(int.Parse(parts[1]),ref row);
                    else if (instruction.StartsWith("FJMP")) Fjmp(int.Parse(parts[1]), ref row);
                    else if (instruction.StartsWith("PRINT")) Print(int.Parse(parts[1]));
                    else if (instruction.StartsWith("READ")) 
                    {
                        if (parts[1] == "I") Read(PrimitiveType.Int);
                        if (parts[1] == "F") Read(PrimitiveType.Float);
                        if (parts[1] == "S") Read(PrimitiveType.String);
                        if (parts[1] == "B") Read(PrimitiveType.Bool);
                    }
                }
            }
            catch(InterpreterException iex)
            {
                Logger.Log(LogLevel.ERROR, "Interpreter encountered the following Error:", ConsoleColor.Red);
                Logger.Log(LogLevel.ERROR, iex.Message, ConsoleColor.Red);
            }
            _code.Clear();
        }

        private void IndexLabels()
        {
            int row = 0;
            foreach (var instruction in _code)
            {
                if (instruction.StartsWith("LABEL"))
                {
                    var parts = instruction.Split(" ");
                    int label = int.Parse(parts[1]);
                    _labels[label] = row;
                }
                row++;
            }
        }

        private void Add()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Algebra(val1,val2,TypeChecker.AlgebraOperation.Add);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '+' for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        }
        private void Sub()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Algebra(val1, val2, TypeChecker.AlgebraOperation.Subtract);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '-' for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        }
        private void Mul()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Algebra(val1, val2, TypeChecker.AlgebraOperation.Multiply);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '*' (MUL) for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        }
        private void Div()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Algebra(val1, val2, TypeChecker.AlgebraOperation.Divide);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '/' (DIV) for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        }
        private void Mod()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Algebra(val1, val2, TypeChecker.AlgebraOperation.Modulo);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '%' (modulo) for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        }
        private void Uminus()
        {
            var uminus = Pop();
            var val = TypeChecker.Uminus(uminus);
            if(val.type != PrimitiveType.Error) Push(val.type, val.value);
            else throw new InterpreterException($"Invalid operation '-' (unary minus) for type '{Enum.GetName(uminus.type)}' ");
        }
        private void Concat()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Concat(val1, val2);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '.' (concat) for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        }
        private void And()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Logic(val1,val2, TypeChecker.LogicOperation.And);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '&&' (AND) for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        } 
        private void Or()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Logic(val1, val2, TypeChecker.LogicOperation.Or);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '||' (OR) for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        }
        private void Gt()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Compare(val1, val2, TypeChecker.CompareOperation.GreaterThan);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '>' for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        }
        private void Lt()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Compare(val1, val2, TypeChecker.CompareOperation.LessThan);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '<' for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        }
        private void Eq()
        {
            var (val2, val1) = DoublePop();
            var (type, value) = TypeChecker.Compare(val1, val2, TypeChecker.CompareOperation.Equal);
            if (type != PrimitiveType.Error) Push(type, value);
            else throw new InterpreterException($"Invalid operation '==' for types '{Enum.GetName(val1.type)}' and  '{Enum.GetName(val2.type)}'");
        }
        private void Not()
        {
            var not = Pop();
            var val = TypeChecker.Not(not);
            if (val.type != PrimitiveType.Error) Push(val.type, val.value);
            else throw new InterpreterException($"Invalid operation '!' for type '{Enum.GetName(not.type)}' ");
        }
        private void Itof()
        {
            var integer = Pop();
            if(integer.type == PrimitiveType.Int)
            {
                Push(PrimitiveType.Float, Convert.ToSingle(integer.value));
            }
            else throw new InterpreterException($"Invalid operation 'itof' for non integer type '{Enum.GetName(integer.type)}' ");
        }
        private void Push(PrimitiveType type, object value)
        {
            _stack.Push((type,value));
        }
        private (PrimitiveType type, object value) Pop()
        {
            try
            {
                var pop = _stack.Pop();
                return pop;
            }
            catch (Exception)
            {
                throw new InterpreterException($"Invalid order of operations caused stack exception.");
            }
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
                throw new InterpreterException($"Could not load variable '{id}'. No such variable exists.");
            }
        }
        private void Save(string id)
        {
            // TODO check if declared
            if (SymbolTable.Contains(id))
            {
                var existing = SymbolTable.GetVariable(id);
                var asigned = Pop();
                if (TypeChecker.CanCast(asigned.type, existing.type))
                {
                    var typeResult = TypeChecker.TypeResult(asigned.type, existing.type);
                    SymbolTable.SetVariable(id, (typeResult, asigned.value));
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
        private void Jmp(int to, ref int row)
        {
            row = _labels[to];
        }
        private void Fjmp(int to, ref int row)
        {
            var value = Pop();
            if(value.type == PrimitiveType.Bool)
            {
                if (!Convert.ToBoolean(value.value))
                {
                    row = _labels[to];
                }
            }
            else throw new InterpreterException($"Result of condition is not Boolean value");
        }
        private void Print(int n)
        {
            PopN(n).ForEach((p) => { 
                Console.WriteLine(p);
            });
        }
        private void Read(PrimitiveType t)
        {
            var read = Console.ReadLine();
            int resultInt;
            if (int.TryParse(read, out resultInt))
            {
                Push(t, resultInt);
                return;
            }
            float resultFloat;
            if (float.TryParse(read, out resultFloat))
            {
                Push(t, resultFloat);
                return;
            }
            bool resultBool;
            if(bool.TryParse(read, out resultBool))
            {
                Push(t, resultBool);
                return;
            }
            else
            {
                Push(t, read!);
                return;
            }
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
