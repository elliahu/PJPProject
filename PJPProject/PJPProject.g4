grammar PJPProject;

/** The start rule; begin parsing here. */
program
    : (statement+ | whileLoop | condition)+ EOF                                        #stmt
    ;

whileLoop
    : WHILE_KEYWORD '(' expr ')' (block|statement) 
    ;

condition
    : IF_KEYWORD '(' expr ')' (block|statement)  else?
    ;

else
    : ELSE_KEYWORD  (block|statement)  
    ;

block
    : '{' statement+ '}' 
    ;

statement
    : primitiveType IDENTIFIER (',' IDENTIFIER)* (';')+ # declaration
    | expr (';')+                                       # emptyStmt
    ;

expr: expr op=(MUL|DIV|MOD) expr                # mulDiv
    | expr op=(ADD|SUB|CONCAT) expr             # addSub
    | expr op=(LT|GT|EQ) expr                   # compare
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
    | WRITE_KEYWORD expr (',' expr)*            # writeExpr
    | READ_KEYWORD IDENTIFIER (',' IDENTIFIER)* # readExpr
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
CONCAT : '.' ;
STRING :  ('"' [a-zA-Z0-9(){}<>,.-_!?:/*+%=; ]+ '"') | '""';
BOOL : ('true'|'false');
IDENTIFIER : [a-zA-Z] [a-zA-Z0-9]* ; 
FLOAT : [0-9]+'.'[0-9]+ ;
INT : [0-9]+ ; 
WS : [ \t\r\n]+ -> skip ; // toss out whitespace