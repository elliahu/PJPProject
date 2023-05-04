grammar PJPProject;

/** The start rule; begin parsing here. */
program: stat+ EOF;


stat: block  stat?
        | statement stat?
        | whileLoop stat?
        | condition stat?
        ;

whileLoop                                                                           
    : WHILE_KEYWORD '(' expr ')' block      #whileBlockBlock
    | WHILE_KEYWORD '(' expr ')' statement  #whileBlockStatement
    ;

condition                                                                           
    : IF_KEYWORD '(' expr ')' block                                 #conditionBlock
    | IF_KEYWORD '(' expr ')' block ELSE_KEYWORD block              #conditionBlockElseBlock
    | IF_KEYWORD '(' expr ')' block ELSE_KEYWORD statement          #conditionBlockElseStatement
    |IF_KEYWORD '(' expr ')' statement                              #conditionStatement
    | IF_KEYWORD '(' expr ')' statement  ELSE_KEYWORD block         #conditionStatementElseBlock
    | IF_KEYWORD '(' expr ')' statement  ELSE_KEYWORD statement     #conditionStatementElseStatement
    ;

block
    : '{' statement+ '}'                    #codeBlock      
    ;

statement
    : primitiveType IDENTIFIER (',' IDENTIFIER)* (';')+ # declaration
    | expr (';')+                                       # emptyStmt
    | WRITE_KEYWORD expr (',' expr)*   (';')+           # writeExpr
    | READ_KEYWORD IDENTIFIER (',' IDENTIFIER)*  (';')+ # readExpr
    | COMMENT                                           # comment
    ;

expr: expr op=(MUL|DIV|MOD) expr                # mulDiv
    | expr op=(ADD|SUB|CONCAT) expr             # addSub
    | expr op=(LT|GT|EQ|NOTEQ) expr             # compare
    | expr op=(AND|OR) expr                     # andOr
    | NOT expr                                  # not
    | SUB expr                                  # uminus
    | STRING                                    # string
    | INT                                       # int
    | IDENTIFIER                                # id
    | FLOAT                                     # float
    | BOOL                                      # bool
    | '(' expr ')'                              # parens
    | <assoc=right> IDENTIFIER '=' expr         # assignment
    ;

primitiveType
    : type=INT_KEYWORD
    | type=FLOAT_KEYWORD
    | type=STRING_KEYWORD
    | type=BOOL_KEYWORD
    ;


INT_KEYWORD : 'int';
FLOAT_KEYWORD : 'float';
STRING_KEYWORD : 'string';
BOOL_KEYWORD : 'bool';
WRITE_KEYWORD : 'write';
READ_KEYWORD : 'read';
WHILE_KEYWORD : 'while';
IF_KEYWORD : 'if' ;
ELSE_KEYWORD : 'else' ;
SEMI:               ';';
COMMA:              ',';
MUL : '*' ; 
DIV : '/' ;
ADD : '+' ;
SUB : '-' ;
MOD : '%' ;
GT : '>' ;
LT : '<' ;
EQ : '==' ;
AND : '&&' ;
OR : '||' ;
NOT : '!' ;
NOTEQ: '!=' ;
CONCAT : '.' ;
STRING :  ('"' [a-zA-Z0-9(){}<>,._!?:/*+%=; ]* '-'? [a-zA-Z0-9(){}<>,._!?:/*+%=; ]* '"') | '""';
BOOL : ('true'|'false');
IDENTIFIER : [a-zA-Z] [a-zA-Z0-9]* ; 
FLOAT : [0-9]+'.'[0-9]+ ;
INT : [0-9]+ ; 
WS : [ \t\r\n]+ -> skip ; // toss out whitespace
COMMENT: '//' [a-zA-Z0-9.()"*-+/%,; ]+;