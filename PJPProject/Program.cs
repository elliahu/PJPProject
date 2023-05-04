using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PJPProject;
using System.Globalization;

Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

var inputFile = "input.txt";
Logger.Log(LogLevel.INFO, "Parsing: " + inputFile);

// Parsing
var inputStreamReader = new StreamReader(inputFile);
AntlrInputStream inputStream = new AntlrInputStream(inputStreamReader);
PJPProjectLexer lexer = new PJPProjectLexer(inputStream);
CommonTokenStream tokenStream= new CommonTokenStream(lexer);
PJPProjectParser parser= new PJPProjectParser(tokenStream);

// add error listener
parser.AddErrorListener(new ErrorListener());

IParseTree tree = parser.program();

if(parser.NumberOfSyntaxErrors == 0)
{
    var visitor = new TreeVisitor();
    visitor.Visit(tree);

    VirtualMachine vm = new(visitor.GetCode());
    Console.WriteLine("\nGENERATED CODE:");
    vm.DumpCode();
    Console.WriteLine("\nOUTPUT:");
    vm.Run();

    Console.WriteLine("\nERRORS:");
    ErrorList.Dump();
}
