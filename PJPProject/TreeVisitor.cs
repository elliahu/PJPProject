﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PJPProject
{
    public class TreeVisitor: PJPProjectBaseVisitor<(PrimitiveType type, object value)>
    {
        public static SymbolTable SymbolTable = new();
        private StringBuilder _code = new();
        private List<int> _labels = new(){ 0 };
        public string GetCode() => _code.ToString();
        public void Dump(string filename)
        {
            Logger.Log(LogLevel.INFO, $"Created file {filename}");
            File.WriteAllText(filename, _code.ToString());
        }

        private void AddInstruction(string instruction)
        {
            _code.AppendLine(instruction);
        }

        public override (PrimitiveType type, object value) VisitCodeBlock([NotNull] PJPProjectParser.CodeBlockContext context)
        {
            foreach(var statement in context.statement())
            {
                Visit(statement);
            }
            return (PrimitiveType.Error, -1);
        }
        public override (PrimitiveType type, object value) VisitConditionBlock([NotNull] PJPProjectParser.ConditionBlockContext context)
        {
            int last = _labels.Last() + 1;
            var condition = Visit(context.expr()); // condition
            AddInstruction(VirtualMachine.Instruction.Fjmp(last));
            Visit(context.block());
            AddInstruction(VirtualMachine.Instruction.Label(last));
            _labels.Add(last);

            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitConditionStatement([NotNull] PJPProjectParser.ConditionStatementContext context)
        {
            int last = _labels.Last() + 1;
            var condition = Visit(context.expr());
            AddInstruction(VirtualMachine.Instruction.Fjmp(last));
            Visit(context.statement());
            AddInstruction(VirtualMachine.Instruction.Label(last));
            _labels.Add(last);

            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitConditionBlockElseBlock([NotNull] PJPProjectParser.ConditionBlockElseBlockContext context)
        {
            int last = _labels.Last() + 1;
            var condition = Visit(context.expr()); // condition
            
            AddInstruction(VirtualMachine.Instruction.Fjmp(last));
            Visit(context.block()[0]);
            AddInstruction(VirtualMachine.Instruction.Label(last));
            _labels.Add(last);

            if (!Convert.ToBoolean(condition.value))
            {
                Visit(context.block()[1]);
            }
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitConditionBlockElseStatement([NotNull] PJPProjectParser.ConditionBlockElseStatementContext context)
        {
            int last = _labels.Last() + 1;
            var condition = Visit(context.expr()); // condition

            AddInstruction(VirtualMachine.Instruction.Fjmp(last));
            Visit(context.block());
            AddInstruction(VirtualMachine.Instruction.Label(last));
            _labels.Add(last);

            if (!Convert.ToBoolean(condition.value))
            {
                Visit(context.statement());
            }
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitConditionStatementElseBlock([NotNull] PJPProjectParser.ConditionStatementElseBlockContext context)
        {
            int last = _labels.Last() + 1;
            var condition = Visit(context.expr()); // condition

            AddInstruction(VirtualMachine.Instruction.Fjmp(last));
            Visit(context.statement());
            AddInstruction(VirtualMachine.Instruction.Label(last));
            _labels.Add(last);

            if (!Convert.ToBoolean(condition.value))
            {
                Visit(context.block());
            }
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitConditionStatementElseStatement([NotNull] PJPProjectParser.ConditionStatementElseStatementContext context)
        {
            int last = _labels.Last() + 1;
            var condition = Visit(context.expr()); // condition

            AddInstruction(VirtualMachine.Instruction.Fjmp(last));
            Visit(context.statement()[0]);
            AddInstruction(VirtualMachine.Instruction.Label(last));
            _labels.Add(last);

            if (!Convert.ToBoolean(condition.value))
            {
                Visit(context.statement()[1]);
            }
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitWhileBlockBlock([NotNull] PJPProjectParser.WhileBlockBlockContext context)
        {
            int skip = _labels.Last() + 1;
            _labels.Add(skip);
            int loop = _labels.Last() + 1;
            _labels.Add(loop);
            Visit(context.expr()); // condition
            AddInstruction(VirtualMachine.Instruction.Fjmp(skip));
            AddInstruction(VirtualMachine.Instruction.Label(loop));
            Visit(context.block());
            Visit(context.expr());// condition
            AddInstruction(VirtualMachine.Instruction.Not);
            AddInstruction(VirtualMachine.Instruction.Fjmp(loop));
            AddInstruction(VirtualMachine.Instruction.Label(skip));

            return (PrimitiveType.Error, -1);
        }
        public override (PrimitiveType type, object value) VisitDeclaration([NotNull] PJPProjectParser.DeclarationContext context)
        {
            var type = Visit(context.primitiveType());
            foreach(var identifier in context.IDENTIFIER())
            {
                SymbolTable.Add(identifier.Symbol, type.type);
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
                return (PrimitiveType.Float, 0.0f);
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
            else if(op == ".")
            {
                AddInstruction(VirtualMachine.Instruction.Concat);
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
            else if(op == "%")
            {
                AddInstruction(VirtualMachine.Instruction.Mod);
            }
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitCompare([NotNull] PJPProjectParser.CompareContext context)
        {
            Visit(context.expr()[0]);
            Visit(context.expr()[1]);
            var op = context.op.Text.Trim();
            if(op == "==")
            {
                AddInstruction(VirtualMachine.Instruction.Eq);
            }
            if(op == ">")
            {
                AddInstruction(VirtualMachine.Instruction.Gt);
            }
            if(op == "<")
            {
                AddInstruction(VirtualMachine.Instruction.Lt);
            }
            if(op == "!=")
            {
                AddInstruction(VirtualMachine.Instruction.Eq);
                AddInstruction(VirtualMachine.Instruction.Not);
            }
            return (PrimitiveType.Error, -1);
        }

      
        public override (PrimitiveType type, object value) VisitAndOr([NotNull] PJPProjectParser.AndOrContext context)
        {
            Visit(context.expr()[0]);
            Visit(context.expr()[1]);
            var op = context.op.Text.Trim();
            if(op == "&&")
            {
                AddInstruction(VirtualMachine.Instruction.And);
            }
            else if(op == "||")
            {
                AddInstruction(VirtualMachine.Instruction.Or);
            }
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitNot([NotNull] PJPProjectParser.NotContext context)
        {
            Visit(context.expr());
            AddInstruction(VirtualMachine.Instruction.Not);
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitUminus([NotNull] PJPProjectParser.UminusContext context)
        {
            Visit(context.expr());
            AddInstruction(VirtualMachine.Instruction.Uminus);
            return (PrimitiveType.Error, -1);
        }
        public override (PrimitiveType type, object value) VisitAssignment([NotNull] PJPProjectParser.AssignmentContext context)
        {
            var id = context.IDENTIFIER().Symbol.Text.Trim(); // ID
            var right = Visit(context.expr()); // Resolve expression on the right
            var left = SymbolTable.GetVariable(id);
            if(TypeChecker.CanCast(right.type, left.type) &&
               right.type == PrimitiveType.Int &&
               left.type == PrimitiveType.Float)
            {
                AddInstruction(VirtualMachine.Instruction.Itof);
            }
            AddInstruction(VirtualMachine.Instruction.Save(id)); // save the result to the ID on the left
            AddInstruction(VirtualMachine.Instruction.Load(id)); // Push value of the ID to the stack
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitWriteExpr([NotNull] PJPProjectParser.WriteExprContext context)
        {
            int num = 0;
            foreach (var expr in context.expr().Reverse())
            {
                Visit(expr);
                num++;
            }
            AddInstruction(VirtualMachine.Instruction.Print(num));
            return (PrimitiveType.Error, -1);
        }

        public override (PrimitiveType type, object value) VisitReadExpr([NotNull] PJPProjectParser.ReadExprContext context)
        {
            foreach (var id in context.IDENTIFIER())
            {
                var found = SymbolTable.GetVariable(id.Symbol.Text);
                if(found.type != PrimitiveType.Error)
                {
                    AddInstruction(VirtualMachine.Instruction.Read(found.type));
                    AddInstruction(VirtualMachine.Instruction.Save(id.Symbol.Text));
                }
                else
                {
                    throw new VirtualMachine.InterpreterException($"Could not load variable '{id}'. No such variable exists.");
                }
            }
            return (PrimitiveType.Error, -1);
        }
    }
}
