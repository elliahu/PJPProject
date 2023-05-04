using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace PJPProject
{
    public class ErrorListener: BaseErrorListener
    {
        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {

            IList<string> stack = ((Parser)recognizer).GetRuleInvocationStack();
            _ = stack.Reverse();
            Logger.Log(LogLevel.ERROR,
                "rule stack: " + String.Join(", ", stack) +
                "line " + line + ":" + charPositionInLine + " at " + offendingSymbol + ": " + msg
                , ConsoleColor.Red);
        }
    }
}
