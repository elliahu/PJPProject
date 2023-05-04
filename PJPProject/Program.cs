using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PJPProject;
using System.Globalization;

Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

var inputFile = "input.txt";
var outputFile = "output.txt";
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
    // traverse tree and generate isntructions
    var visitor = new TreeVisitor();
    visitor.Visit(tree);
    visitor.Dump(outputFile);
    Logger.Log(LogLevel.INFO, "COMPILE ERRORS:", ConsoleColor.White);
    ErrorList.Dump();

    // create VM and read instruction
    VirtualMachine vm = new();
    vm.ReadFile(outputFile);
    Logger.Log(LogLevel.INFO, "GENERATED CODE:", ConsoleColor.White);
    vm.Dump();
    Logger.Log(LogLevel.INFO, "OUTPUT:", ConsoleColor.White);
    vm.Run();
}
