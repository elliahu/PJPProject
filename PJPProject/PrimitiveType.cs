using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJPProject
{
    public enum PrimitiveType
    {
        Int,
        Float,
        String,
        Bool,
        Error
    }

    public class InvalidPrimitiveTypeCastException: Exception { }

    public class TypeChecker
    {
        public enum Operation
        {
            Add,
            Subtract, 
            Multiply,
            Divide,
        }
        public static bool CanCast(PrimitiveType t1, PrimitiveType t2) 
        {
            if(t1 == PrimitiveType.Int)
            {
                return t2 == PrimitiveType.Int || t2 == PrimitiveType.Float || t2 == PrimitiveType.Bool;
            }
            else if (t1 == PrimitiveType.Float)
            {
                return t2 == PrimitiveType.Float;
            }
            else if(t1 == PrimitiveType.String)
            {
                return t2 == PrimitiveType.String;
            }
            else if(t1 == PrimitiveType.Bool)
            {
                return t2 == PrimitiveType.Bool || t2 == PrimitiveType.Int;
            }
            return false;
        }

        public static (PrimitiveType type, object value) Cast((PrimitiveType type, object value) value, PrimitiveType type)
        {
            if(CanCast(value.type, type))
            {
                if(type == PrimitiveType.Int)
                {
                    return (type, (int)value.value);
                }
                else if(type == PrimitiveType.Float)
                {
                    return (type, Convert.ToSingle(value.value));
                }
                else if(type == PrimitiveType.String)
                {
                    return (type, (string)value.value);
                }
                else if (type == PrimitiveType.Bool)
                {
                    return (type, (bool)value.value);
                }
            }
            throw new InvalidPrimitiveTypeCastException();
        }

        public static PrimitiveType CharToType(string c)
        {
            switch(c)
            {
                case "I":
                    return PrimitiveType.Int;
                    break;
                case "F":
                    return PrimitiveType.Float;
                    break;
                case "S":
                    return PrimitiveType.String;
                    break;
                case "B":
                    return PrimitiveType.Bool;
                    break;
                default:
                    return PrimitiveType.Error;
            }
        }

        public static PrimitiveType TypeResult(PrimitiveType type1, PrimitiveType type2) 
        {
            if(type1 == PrimitiveType.String || type2 == PrimitiveType.String) 
            {
                return PrimitiveType.Error;
            }

            else if(type1 == PrimitiveType.Float || type2 == PrimitiveType.Float)
            {
                return PrimitiveType.Float;
            }

            else if(type1 == type2)
            {
                return type1;
            }

            return PrimitiveType.Error;
        }

        public static object StringToObj(PrimitiveType type, string value)
        {
            if(type == PrimitiveType.String)
            {
                return value.Replace("\"","");
            }
            if(type == PrimitiveType.Float)
            {
                return float.Parse(value);
            }
            if(type == PrimitiveType.Int)
            {
                return int.Parse(value);
            }
            if(type == PrimitiveType.Bool)
            {
                return bool.Parse(value);
            }
            return int.Parse(value);
        }

        public static (PrimitiveType type, object value) Calc(
            (PrimitiveType type, object value) val1, 
            (PrimitiveType type, object value) val2, 
            Operation operation)
        {
            var resultType = TypeResult(val1.type, val2.type);
            if(resultType == PrimitiveType.Int)
            {
                switch(operation)
                {
                    case Operation.Add: return (resultType, Convert.ToInt32(val1.value) + Convert.ToInt32(val2.value));
                    case Operation.Subtract: return (resultType, Convert.ToInt32(val1.value) - Convert.ToInt32(val2.value));
                    case Operation.Divide:
                        if (Convert.ToInt32(val2.value) == 0) throw new VirtualMachine.InterpreterException("Cannot devide by 0!");
                        return (resultType, Convert.ToInt32(val1.value) / Convert.ToInt32(val2.value));
                    case Operation.Multiply: return (resultType, Convert.ToInt32(val1.value) * Convert.ToInt32(val2.value));
                }
            }
            else if(resultType == PrimitiveType.Float)
            {
                switch (operation)
                {
                    case Operation.Add: return (resultType, Convert.ToSingle(val1.value) + Convert.ToSingle(val2.value));
                    case Operation.Subtract: return (resultType, Convert.ToSingle(val1.value) - Convert.ToSingle(val2.value));
                    case Operation.Divide: 
                        if(Convert.ToSingle(val2.value) == Convert.ToSingle(0)) throw new VirtualMachine.InterpreterException("Cannot devide by 0!");
                        return (resultType, Convert.ToSingle(val1.value) / Convert.ToSingle(val2.value));
                    case Operation.Multiply: return (resultType, Convert.ToSingle(val1.value) * Convert.ToSingle(val2.value));
                }
            }
            return (PrimitiveType.Error, -1);
        }
    }
}
