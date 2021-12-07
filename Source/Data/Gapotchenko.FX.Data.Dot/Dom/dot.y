%using Gapotchenko.FX.Data.Dot.ParserToolkit;
%namespace Gapotchenko.FX.Data.Dot.Dom
%visibility internal
%parsertype DotParser
%tokentype DotTokens
%YYSTYPE DotValueType
%partial

%start graph

%union {
    public DotToken token;
    public DotNode entity;
    public SeparatedDotNodeList<DotNode> separatedSyntaxList;
    public DotNodeList<DotAttributeNode> attributeSyntaxList;
    public DotNodeList<DotAttributeListNode> attributeListSyntaxList;
    public DotNodeList<DotStatementNode> statementSyntaxList;
}

%token <token> DIGRAPH GRAPH ARROW SUBGRAPH NODE EDGE ID

%type <token> graphType graphName id
%type <entity> graph subgraph stmt stmt node_stmt edge_stmt endpoint stmts node_id
%type <entity> attr_stmt opt_attr_list avPair
%type <statementSyntaxList> stmt_list
%type <separatedSyntaxList> edgeRHS
%type <attributeSyntaxList> a_list
%type <attributeListSyntaxList> attr_list opt_attr_list

%left '{'

%%

graph     : graphType graphName stmts    { Root = CreateGraphSyntax(default, $1, $2, (DotStatementListNode)$3); }
          | id graphType graphName stmts { Root = CreateGraphSyntax($1, $2, $3, (DotStatementListNode)$4); }
          ;
          
stmts     : '{' stmt_list '}' { $$ = CreateStatementListSyntax(CreateToken($1), $2, CreateToken($3)); }
          ;

graphType : DIGRAPH { $$ = $1; }
          | GRAPH   { $$ = $1; }
          ;
 
graphName : id { $$ = $1; } 
          |    { }
          ;

stmt_list : { $$ = new(); }
          | stmt stmt_list { Prepend($2, (DotStatementNode)$1); $$ = $2; }
          | stmt ';' stmt_list { var statement = (DotStatementNode)$1;
                                 statement.SemicolonToken = CreateToken($2); 
                                 Prepend($3, (DotStatementNode)$1);
                                 $$ = $3; }
          ;

stmt      : id '=' id { $$ = CreateAliasSyntax($1, CreateToken($2), $3); } 
          | node_stmt { $$ = $1; }
          | edge_stmt { $$ = $1; }
          | attr_stmt { $$ = $1; }
          | subgraph  { $$ = $1; }
          ;

node_stmt : node_id opt_attr_list { $$ = CreateVertexSyntax((DotVertexIdentifierNode)$1, $2); }
          ;

edge_stmt : endpoint edgeRHS opt_attr_list { Prepend($2, $1); $$ = CreateEdgeSyntax($2, $3);} 
          ;

endpoint  : node_id  { $$ = $1; }
          | subgraph { $$ = $1; }
		  ;

edgeRHS   : ARROW endpoint          { $$ = new SeparatedDotNodeList<DotNode>() { $1, $2 }; }
          | edgeRHS ARROW endpoint  { $1.Add($2);
                                      $1.Add($3);
                                      $$ = $1; }
          ;

subgraph  : stmts              { $$ = CreateGraphSyntax(default, default, default, (DotStatementListNode)$1); }
          | SUBGRAPH stmts     { $$ = CreateGraphSyntax(default, $1, default, (DotStatementListNode)$2); }
          | SUBGRAPH id stmts  { $$ = CreateGraphSyntax(default, $1, $2, (DotStatementListNode)$3); }
          | SUBGRAPH           { $$ = CreateGraphSyntax(default, $1, default, default); }
          ;

attr_stmt : GRAPH attr_list { $$ = CreateAttachedAttributesSyntax($1, $2); }
          | NODE attr_list  { $$ = CreateAttachedAttributesSyntax($1, $2); }
          | EDGE attr_list  { $$ = CreateAttachedAttributesSyntax($1, $2); }
          ;

opt_attr_list :       { }
          | attr_list { $$ = $1; }
          ;

attr_list : '[' ']'        { $$ = CreateAttributeListSyntaxList(CreateToken($1), default, CreateToken($2)); }
          | '[' a_list ']' { $$ = CreateAttributeListSyntaxList(CreateToken($1), $2, CreateToken($3)); }
          ;

a_list    : avPair            { $$ = new(); $$.Add((DotAttributeNode)$1); }
          | avPair a_list     { $$ = $2; Prepend($2, (DotAttributeNode)$1); }
          | avPair ',' a_list { $$ = $3;
                                var attr = (DotAttributeNode)$1;
                                attr.SemicolonOrCommaToken = CreateToken($2); 
                                Prepend($3, attr); }
          | avPair ';' a_list { $$ = $3; 
                                var attr = (DotAttributeNode)$1;
                                attr.SemicolonOrCommaToken = CreateToken($2); 
                                Prepend($3, attr); }
          ;

avPair    : id '=' id { $$ = CreateAttributeSyntax($1, CreateToken($2), $3, default); }
          | id        { $$ = CreateAttributeSyntax($1, default, default, default); }
          ;

node_id   : id               { $$ = CreateVertexIdentifierSyntax($1, default, default, default, default); }
          | id ':' id        { $$ = CreateVertexIdentifierSyntax($1, CreateToken($2), $3, default, default); }
          | id ':' id ':' id { $$ = CreateVertexIdentifierSyntax($1, CreateToken($2), $3, CreateToken($4), $5); }
          ;
          
id        : ID          { $$ = $1; }
          | '"' ID '"'  { $$ = $2; 
                          AppendLeadingTrivia($2, $1);
                          PrependTrailingTrivia($2, $3); }
          ;

%%

/*

port : ':' ID [ ':' compass_pt ] 
     | ':' compass_pt 
subgraph : [ subgraph [ ID ] ] '{' stmt_list '}' 
compass_pt : (n | ne | e | se | s | sw | w | nw | c | _) 

The keywords node, edge, graph, digraph, subgraph, and strict are case-independent. 
Note also that the allowed compass point values are not keywords,
so these strings can be used elsewhere as ordinary identifiers and, conversely, 
the parser will actually accept any identifier. 

An ID is one of the following: 

Any string of alphabetic ([a-zA-Z\200-\377]) characters, underscores ('_') or digits ([0-9]), not beginning with a digit; 
a numeral [-]?(.[0-9]+ | [0-9]+(.[0-9]*)? ); 
any double-quoted string ("...") possibly containing escaped quotes (\")1; 
an HTML string (<...>). 
An ID is just a string; the lack of quote characters in the first two forms is just for simplicity. There is no semantic difference between abc_2 and "abc_2", or between 2.34 and "2.34". Obviously, to use a keyword as an ID, it must be quoted. Note that, in HTML strings, angle brackets must occur in matched pairs, and unescaped newlines are allowed. In addition, the content must be legal XML, so that the special XML escape sequences for ", &, <, and > may be necessary in order to embed these characters in attribute values or raw text. 
Both quoted strings and HTML strings are scanned as a unit, so any embedded comments will be treated as part of the strings. 

An edgeop is -> in directed graphs and -- in undirected graphs. 

An a_list clause of the form ID is equivalent to ID=true. 

The language supports C++-style comments: and //. In addition, a line beginning with a '#' character is considered a line output from a C preprocessor (e.g., # 34 to indicate line 34 ) and discarded. 

Semicolons aid readability but are not required except in the rare case that a named subgraph with no body immediately preceeds an anonymous subgraph, since the precedence rules cause this sequence to be parsed as a subgraph with a heading and a body. Also, any amount of whitespace may be inserted between terminals. 

As another aid for readability, dot allows single logical lines to span multiple physical lines using the standard C convention of a backslash immediately preceding a newline character. In addition, double-quoted strings can be concatenated using a '+' operator. As HTML strings can contain newline characters, they do not support the concatenation operator. 

*/