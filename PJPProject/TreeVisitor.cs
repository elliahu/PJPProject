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
        SymbolTable symbolTable = new();

        private static float ToFloat(object value)
        {
            return (value is int x)? (float)x: (float)value;
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
                symbolTable.Add(identifier.Symbol, type.type);
            }
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitPrintExpr([NotNull] PJPProjectParser.PrintExprContext context)
        {
            var value = Visit(context.expr());
            if(value.type != PrimitiveType.Error)
            {
                Console.WriteLine(value.value.ToString());
            }
            else
            {
                ErrorList.Dump();
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
            return(PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitFloat([NotNull] PJPProjectParser.FloatContext context)
        {
            return (PrimitiveType.Float, float.Parse(context.FLOAT().GetType().ToString()));
        }

        public override (PrimitiveType type, object value) VisitInt([NotNull] PJPProjectParser.IntContext context)
        {
            return (PrimitiveType.Int, int.Parse(context.INT().GetType().ToString()));
        }

        public override (PrimitiveType type, object value) VisitId([NotNull] PJPProjectParser.IdContext context)
        {
            return symbolTable[context.IDENTIFIER().Symbol];
        }

        public override (PrimitiveType type, object value) VisitParens([NotNull] PJPProjectParser.ParensContext context)
        {
            return Visit(context.expr());
        }

        public override (PrimitiveType type, object value) VisitAddSub([NotNull] PJPProjectParser.AddSubContext context)
        {
            return base.VisitAddSub(context);
        }

        public override (PrimitiveType type, object value) VisitMulDiv([NotNull] PJPProjectParser.MulDivContext context)
        {
            return base.VisitMulDiv(context);
        }

        public override (PrimitiveType type, object value) VisitAssignment([NotNull] PJPProjectParser.AssignmentContext context)
        {
            return base.VisitAssignment(context);
        }


    }
}
