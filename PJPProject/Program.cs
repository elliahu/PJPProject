using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PJPProject;

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
    var result = new TreeVisitor().Visit(tree);
    Console.WriteLine(result);


}