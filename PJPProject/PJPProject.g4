grammar PJPProject;

/** The start rule; begin parsing here. */
program: statement+ ;

statement
    : primitiveType IDENTIFIER (',' IDENTIFIER)* ';' # declaration
    | expr ';'                                       # emptyStmt
    ;

expr: expr op=(MUL|DIV) expr                # mulDiv
    | expr op=(ADD|SUB) expr                # addSub
    | INT                                   # int
    | IDENTIFIER                            # id
    | FLOAT                                 # float
    | STRING                                # string
    | BOOL                                  # bool
    | '(' expr ')'                          # parens
    | <assoc=right> IDENTIFIER '=' expr     # assignment
    | WRITE_KEYWORD expr (',' expr)*        # writeExpr
    ;

primitiveType
    : type=INT_KEYWORD
    | type=FLOAT_KEYWORD
    | type=STRING_KEYWORD
    | tpye=BOOL_KEYWORD
    ;


INT_KEYWORD : 'int';
FLOAT_KEYWORD : 'float';
STRING_KEYWORD : 'string';
BOOL_KEYWORD : 'bool';
WRITE_KEYWORD : 'write';
SEMI:               ';';
COMMA:              ',';
MUL : '*' ; 
DIV : '/' ;
ADD : '+' ;
SUB : '-' ;
STRING : '"'.*'"' ;
BOOL : ('true'|'false');
IDENTIFIER : [a-zA-Z] [a-zA-Z0-9]* ; 
FLOAT : [0-9]+'.'[0-9]+ ;
INT : [0-9]+ ; 
WS : [ \t\r\n]+ -> skip ; // toss out whitespace