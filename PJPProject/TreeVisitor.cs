using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJPProject
{
    public class TreeVisitor: PJPProjectBaseVisitor<(PrimitiveType type, object value)>
    {
        
        private StringBuilder _code = new();
        public string GetCode() => _code.ToString();

        private void AddInstruction(string instruction)
        {
            _code.AppendLine(instruction);
        }


        public override (PrimitiveType type, object value) VisitProgram([NotNull] PJPProjectParser.ProgramContext context)
        {
            foreach(var statement in context.statement())
            {
                Visit(statement);
            }
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitDeclaration([NotNull] PJPProjectParser.DeclarationContext context)
        {
            var type = Visit(context.primitiveType());
            foreach(var identifier in context.IDENTIFIER())
            {
                AddInstruction(VirtualMachine.Instruction.Push(type));
                AddInstruction(VirtualMachine.Instruction.Save(identifier.Symbol.Text));
            }
            return (PrimitiveType.Error, -1);
        }


        public override (PrimitiveType type, object value) VisitPrimitiveType([NotNull] PJPProjectParser.PrimitiveTypeContext context)
        {
            if (context.type.Text.Equals("int"))
            {
                return (PrimitiveType.Int, 0);
            }
            else if(context.type.Text.Equals("float")) 
            {
                return (PrimitiveType.Float, 0);
            }
            else if(context.type.Text.Equals("string"))
            {
                return (PrimitiveType.String, String.Empty);
            }
            else if(context.type.Text.Equals("bool"))
            {
                return (PrimitiveType.Bool, false);
            }
            return(PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitFloat([NotNull] PJPProjectParser.FloatContext context)
        {
            float val = float.Parse(context.FLOAT().GetText());
            AddInstruction(VirtualMachine.Instruction.Push((PrimitiveType.Float, val)));
            return (PrimitiveType.Float, val);
        }

        public override (PrimitiveType type, object value) VisitInt([NotNull] PJPProjectParser.IntContext context)
        {
            int val = int.Parse(context.INT().GetText());
            AddInstruction(VirtualMachine.Instruction.Push((PrimitiveType.Int, val)));
            return (PrimitiveType.Int, val);
        }

        public override (PrimitiveType type, object value) VisitString([NotNull] PJPProjectParser.StringContext context)
        {
            string val = context.STRING().GetText();
            AddInstruction(VirtualMachine.Instruction.Push((PrimitiveType.String, val)));
            return (PrimitiveType.String, val);
        }

        public override (PrimitiveType type, object value) VisitBool([NotNull] PJPProjectParser.BoolContext context)
        {
            bool val = bool.Parse(context.BOOL().GetText());
            AddInstruction(VirtualMachine.Instruction.Push((PrimitiveType.Bool, val)));
            return (PrimitiveType.Bool, val);
        }

        public override (PrimitiveType type, object value) VisitId([NotNull] PJPProjectParser.IdContext context)
        {
            AddInstruction(VirtualMachine.Instruction.Load(context.IDENTIFIER().Symbol.Text));
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitParens([NotNull] PJPProjectParser.ParensContext context)
        {
            return Visit(context.expr());
        }

        public override (PrimitiveType type, object value) VisitAddSub([NotNull] PJPProjectParser.AddSubContext context)
        {
            Visit(context.expr()[0]);
            Visit(context.expr()[1]);
            var op = context.op.Text.Trim();

            if(op== "+")
            {
                AddInstruction(VirtualMachine.Instruction.Add);
            }
            else if(op== "-")
            {
                AddInstruction(VirtualMachine.Instruction.Sub);
            }
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitMulDiv([NotNull] PJPProjectParser.MulDivContext context)
        {
            Visit(context.expr()[0]);
            Visit(context.expr()[1]);
            var op = context.op.Text.Trim();
            if(op == "*")
            {
                AddInstruction(VirtualMachine.Instruction.Mul);
            }
            else if(op == "/")
            {
                AddInstruction(VirtualMachine.Instruction.Div);
            }
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitAssignment([NotNull] PJPProjectParser.AssignmentContext context)
        {
            var left = context.IDENTIFIER().Symbol.Text.Trim();
            var right = Visit(context.expr());
            AddInstruction(VirtualMachine.Instruction.Save(left));
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitWriteExpr([NotNull] PJPProjectParser.WriteExprContext context)
        {
            int num = 0;
            foreach (var expr in context.expr())
            {
                Visit(expr);
                num++;
            }
            AddInstruction(VirtualMachine.Instruction.Print(num));
            return (PrimitiveType.Error, -1);
        }
    }
}
