// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// Input file <Dom\Dot.y - 10.12.2021 22:06:07>

// options: no-lines

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using Gapotchenko.FX.Data.Dot.ParserToolkit;

namespace Gapotchenko.FX.Data.Dot.Dom
{
internal enum DotTokens {
    error=127,EOF=128,DIGRAPH=129,GRAPH=130,ARROW=131,SUBGRAPH=132,
    NODE=133,EDGE=134,ID=135};

internal partial struct DotValueType
{
    public DotToken token;
    public DotNode entity;
    public SeparatedDotNodeList<DotNode> separatedSyntaxList;
    public DotNodeList<DotAttributeNode> attributeSyntaxList;
    public DotNodeList<DotAttributeListNode> attributeListSyntaxList;
    public DotNodeList<DotStatementNode> statementSyntaxList;
}
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal partial class DotParser: ShiftReduceParser<DotValueType, LexLocation>
{
#pragma warning disable 649
  private static Dictionary<int, string>? aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[44];
  private static State[] states = new State[69];
  private static string[] nonTerms = new string[] {
      "graph", "graphType", "graphName", "id", "subgraph", "stmt", "node_stmt", 
      "edge_stmt", "endpoint", "stmts", "node_id", "attr_stmt", "opt_attr_list", 
      "avPair", "stmt_list", "edgeRHS", "a_list", "attr_list", "$accept", };

  static DotParser() {
    states[0] = new State(new int[]{129,63,130,64,135,16},new int[]{-1,1,-2,3,-4,65});
    states[1] = new State(new int[]{128,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{135,16,123,-8},new int[]{-3,4,-4,62});
    states[4] = new State(new int[]{123,6},new int[]{-10,5});
    states[5] = new State(-2);
    states[6] = new State(new int[]{135,16,123,6,132,48,130,56,133,58,134,60,125,-9},new int[]{-15,7,-6,9,-4,13,-7,21,-11,22,-8,38,-9,39,-5,54,-10,47,-12,55});
    states[7] = new State(new int[]{125,8});
    states[8] = new State(-4);
    states[9] = new State(new int[]{59,11,135,16,123,6,132,48,130,56,133,58,134,60,125,-9},new int[]{-15,10,-6,9,-4,13,-7,21,-11,22,-8,38,-9,39,-5,54,-10,47,-12,55});
    states[10] = new State(-10);
    states[11] = new State(new int[]{135,16,123,6,132,48,130,56,133,58,134,60,125,-9},new int[]{-15,12,-6,9,-4,13,-7,21,-11,22,-8,38,-9,39,-5,54,-10,47,-12,55});
    states[12] = new State(-11);
    states[13] = new State(new int[]{61,14,58,17,91,-40,59,-40,135,-40,123,-40,132,-40,130,-40,133,-40,134,-40,125,-40,131,-40});
    states[14] = new State(new int[]{135,16},new int[]{-4,15});
    states[15] = new State(-12);
    states[16] = new State(-43);
    states[17] = new State(new int[]{135,16},new int[]{-4,18});
    states[18] = new State(new int[]{58,19,91,-41,59,-41,135,-41,123,-41,132,-41,130,-41,133,-41,134,-41,125,-41,131,-41});
    states[19] = new State(new int[]{135,16},new int[]{-4,20});
    states[20] = new State(-42);
    states[21] = new State(-13);
    states[22] = new State(new int[]{91,25,131,-19,59,-30,135,-30,123,-30,132,-30,130,-30,133,-30,134,-30,125,-30},new int[]{-13,23,-18,24});
    states[23] = new State(-17);
    states[24] = new State(-31);
    states[25] = new State(new int[]{93,26,135,16},new int[]{-17,27,-14,29,-4,33});
    states[26] = new State(-32);
    states[27] = new State(new int[]{93,28});
    states[28] = new State(-33);
    states[29] = new State(new int[]{44,31,59,36,135,16,93,-34},new int[]{-17,30,-14,29,-4,33});
    states[30] = new State(-35);
    states[31] = new State(new int[]{135,16},new int[]{-17,32,-14,29,-4,33});
    states[32] = new State(-36);
    states[33] = new State(new int[]{61,34,44,-39,59,-39,135,-39,93,-39});
    states[34] = new State(new int[]{135,16},new int[]{-4,35});
    states[35] = new State(-38);
    states[36] = new State(new int[]{135,16},new int[]{-17,37,-14,29,-4,33});
    states[37] = new State(-37);
    states[38] = new State(-14);
    states[39] = new State(new int[]{131,52},new int[]{-16,40});
    states[40] = new State(new int[]{131,42,91,25,59,-30,135,-30,123,-30,132,-30,130,-30,133,-30,134,-30,125,-30},new int[]{-13,41,-18,24});
    states[41] = new State(-18);
    states[42] = new State(new int[]{135,16,123,6,132,48},new int[]{-9,43,-11,44,-4,45,-5,46,-10,47});
    states[43] = new State(-22);
    states[44] = new State(-19);
    states[45] = new State(new int[]{58,17,131,-40,91,-40,59,-40,135,-40,123,-40,132,-40,130,-40,133,-40,134,-40,125,-40});
    states[46] = new State(-20);
    states[47] = new State(-23);
    states[48] = new State(new int[]{123,6,135,16,59,-26,132,-26,130,-26,133,-26,134,-26,125,-26,131,-26,91,-26},new int[]{-10,49,-4,50});
    states[49] = new State(-24);
    states[50] = new State(new int[]{123,6},new int[]{-10,51});
    states[51] = new State(-25);
    states[52] = new State(new int[]{135,16,123,6,132,48},new int[]{-9,53,-11,44,-4,45,-5,46,-10,47});
    states[53] = new State(-21);
    states[54] = new State(new int[]{131,-20,59,-16,135,-16,123,-16,132,-16,130,-16,133,-16,134,-16,125,-16});
    states[55] = new State(-15);
    states[56] = new State(new int[]{91,25},new int[]{-18,57});
    states[57] = new State(-27);
    states[58] = new State(new int[]{91,25},new int[]{-18,59});
    states[59] = new State(-28);
    states[60] = new State(new int[]{91,25},new int[]{-18,61});
    states[61] = new State(-29);
    states[62] = new State(-7);
    states[63] = new State(-5);
    states[64] = new State(-6);
    states[65] = new State(new int[]{129,63,130,64},new int[]{-2,66});
    states[66] = new State(new int[]{135,16,123,-8},new int[]{-3,67,-4,62});
    states[67] = new State(new int[]{123,6},new int[]{-10,68});
    states[68] = new State(-3);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-19, new int[]{-1,128});
    rules[2] = new Rule(-1, new int[]{-2,-3,-10});
    rules[3] = new Rule(-1, new int[]{-4,-2,-3,-10});
    rules[4] = new Rule(-10, new int[]{123,-15,125});
    rules[5] = new Rule(-2, new int[]{129});
    rules[6] = new Rule(-2, new int[]{130});
    rules[7] = new Rule(-3, new int[]{-4});
    rules[8] = new Rule(-3, new int[]{});
    rules[9] = new Rule(-15, new int[]{});
    rules[10] = new Rule(-15, new int[]{-6,-15});
    rules[11] = new Rule(-15, new int[]{-6,59,-15});
    rules[12] = new Rule(-6, new int[]{-4,61,-4});
    rules[13] = new Rule(-6, new int[]{-7});
    rules[14] = new Rule(-6, new int[]{-8});
    rules[15] = new Rule(-6, new int[]{-12});
    rules[16] = new Rule(-6, new int[]{-5});
    rules[17] = new Rule(-7, new int[]{-11,-13});
    rules[18] = new Rule(-8, new int[]{-9,-16,-13});
    rules[19] = new Rule(-9, new int[]{-11});
    rules[20] = new Rule(-9, new int[]{-5});
    rules[21] = new Rule(-16, new int[]{131,-9});
    rules[22] = new Rule(-16, new int[]{-16,131,-9});
    rules[23] = new Rule(-5, new int[]{-10});
    rules[24] = new Rule(-5, new int[]{132,-10});
    rules[25] = new Rule(-5, new int[]{132,-4,-10});
    rules[26] = new Rule(-5, new int[]{132});
    rules[27] = new Rule(-12, new int[]{130,-18});
    rules[28] = new Rule(-12, new int[]{133,-18});
    rules[29] = new Rule(-12, new int[]{134,-18});
    rules[30] = new Rule(-13, new int[]{});
    rules[31] = new Rule(-13, new int[]{-18});
    rules[32] = new Rule(-18, new int[]{91,93});
    rules[33] = new Rule(-18, new int[]{91,-17,93});
    rules[34] = new Rule(-17, new int[]{-14});
    rules[35] = new Rule(-17, new int[]{-14,-17});
    rules[36] = new Rule(-17, new int[]{-14,44,-17});
    rules[37] = new Rule(-17, new int[]{-14,59,-17});
    rules[38] = new Rule(-14, new int[]{-4,61,-4});
    rules[39] = new Rule(-14, new int[]{-4});
    rules[40] = new Rule(-11, new int[]{-4});
    rules[41] = new Rule(-11, new int[]{-4,58,-4});
    rules[42] = new Rule(-11, new int[]{-4,58,-4,58,-4});
    rules[43] = new Rule(-4, new int[]{135});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)DotTokens.error, (int)DotTokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 2: // graph -> graphType, graphName, stmts
{ Root = CreateGraphSyntax(default, ValueStack[ValueStack.Depth-3].token, ValueStack[ValueStack.Depth-2].token, (DotStatementListNode)ValueStack[ValueStack.Depth-1].entity); }
        break;
      case 3: // graph -> id, graphType, graphName, stmts
{ Root = CreateGraphSyntax(ValueStack[ValueStack.Depth-4].token, ValueStack[ValueStack.Depth-3].token, ValueStack[ValueStack.Depth-2].token, (DotStatementListNode)ValueStack[ValueStack.Depth-1].entity); }
        break;
      case 4: // stmts -> '{', stmt_list, '}'
{ CurrentSemanticValue.entity = CreateStatementListSyntax(CreateToken(ValueStack[ValueStack.Depth-3]), ValueStack[ValueStack.Depth-2].statementSyntaxList, CreateToken(ValueStack[ValueStack.Depth-1])); }
        break;
      case 5: // graphType -> DIGRAPH
{ CurrentSemanticValue.token = ValueStack[ValueStack.Depth-1].token; }
        break;
      case 6: // graphType -> GRAPH
{ CurrentSemanticValue.token = ValueStack[ValueStack.Depth-1].token; }
        break;
      case 7: // graphName -> id
{ CurrentSemanticValue.token = ValueStack[ValueStack.Depth-1].token; }
        break;
      case 8: // graphName -> /* empty */
{ }
        break;
      case 9: // stmt_list -> /* empty */
{ CurrentSemanticValue.statementSyntaxList = new(); }
        break;
      case 10: // stmt_list -> stmt, stmt_list
{ Prepend(ValueStack[ValueStack.Depth-1].statementSyntaxList, (DotStatementNode)ValueStack[ValueStack.Depth-2].entity); CurrentSemanticValue.statementSyntaxList = ValueStack[ValueStack.Depth-1].statementSyntaxList; }
        break;
      case 11: // stmt_list -> stmt, ';', stmt_list
{ var statement = (DotStatementNode)ValueStack[ValueStack.Depth-3].entity;
                                 statement.SemicolonToken = CreateToken(ValueStack[ValueStack.Depth-2]); 
                                 Prepend(ValueStack[ValueStack.Depth-1].statementSyntaxList, (DotStatementNode)ValueStack[ValueStack.Depth-3].entity);
                                 CurrentSemanticValue.statementSyntaxList = ValueStack[ValueStack.Depth-1].statementSyntaxList; }
        break;
      case 12: // stmt -> id, '=', id
{ CurrentSemanticValue.entity = CreateAliasSyntax(ValueStack[ValueStack.Depth-3].token, CreateToken(ValueStack[ValueStack.Depth-2]), ValueStack[ValueStack.Depth-1].token); }
        break;
      case 13: // stmt -> node_stmt
{ CurrentSemanticValue.entity = ValueStack[ValueStack.Depth-1].entity; }
        break;
      case 14: // stmt -> edge_stmt
{ CurrentSemanticValue.entity = ValueStack[ValueStack.Depth-1].entity; }
        break;
      case 15: // stmt -> attr_stmt
{ CurrentSemanticValue.entity = ValueStack[ValueStack.Depth-1].entity; }
        break;
      case 16: // stmt -> subgraph
{ CurrentSemanticValue.entity = ValueStack[ValueStack.Depth-1].entity; }
        break;
      case 17: // node_stmt -> node_id, opt_attr_list
{ CurrentSemanticValue.entity = CreateVertexSyntax((DotVertexIdentifierNode)ValueStack[ValueStack.Depth-2].entity, ValueStack[ValueStack.Depth-1].attributeListSyntaxList); }
        break;
      case 18: // edge_stmt -> endpoint, edgeRHS, opt_attr_list
{ Prepend(ValueStack[ValueStack.Depth-2].separatedSyntaxList, ValueStack[ValueStack.Depth-3].entity); CurrentSemanticValue.entity = CreateEdgeSyntax(ValueStack[ValueStack.Depth-2].separatedSyntaxList, ValueStack[ValueStack.Depth-1].attributeListSyntaxList);}
        break;
      case 19: // endpoint -> node_id
{ CurrentSemanticValue.entity = ValueStack[ValueStack.Depth-1].entity; }
        break;
      case 20: // endpoint -> subgraph
{ CurrentSemanticValue.entity = ValueStack[ValueStack.Depth-1].entity; }
        break;
      case 21: // edgeRHS -> ARROW, endpoint
{ CurrentSemanticValue.separatedSyntaxList = new SeparatedDotNodeList<DotNode>() { ValueStack[ValueStack.Depth-2].token, ValueStack[ValueStack.Depth-1].entity }; }
        break;
      case 22: // edgeRHS -> edgeRHS, ARROW, endpoint
{ ValueStack[ValueStack.Depth-3].separatedSyntaxList.Add(ValueStack[ValueStack.Depth-2].token);
                                      ValueStack[ValueStack.Depth-3].separatedSyntaxList.Add(ValueStack[ValueStack.Depth-1].entity);
                                      CurrentSemanticValue.separatedSyntaxList = ValueStack[ValueStack.Depth-3].separatedSyntaxList; }
        break;
      case 23: // subgraph -> stmts
{ CurrentSemanticValue.entity = CreateGraphSyntax(default, default, default, (DotStatementListNode)ValueStack[ValueStack.Depth-1].entity); }
        break;
      case 24: // subgraph -> SUBGRAPH, stmts
{ CurrentSemanticValue.entity = CreateGraphSyntax(default, ValueStack[ValueStack.Depth-2].token, default, (DotStatementListNode)ValueStack[ValueStack.Depth-1].entity); }
        break;
      case 25: // subgraph -> SUBGRAPH, id, stmts
{ CurrentSemanticValue.entity = CreateGraphSyntax(default, ValueStack[ValueStack.Depth-3].token, ValueStack[ValueStack.Depth-2].token, (DotStatementListNode)ValueStack[ValueStack.Depth-1].entity); }
        break;
      case 26: // subgraph -> SUBGRAPH
{ CurrentSemanticValue.entity = CreateGraphSyntax(default, ValueStack[ValueStack.Depth-1].token, default, default); }
        break;
      case 27: // attr_stmt -> GRAPH, attr_list
{ CurrentSemanticValue.entity = CreateAttachedAttributesSyntax(ValueStack[ValueStack.Depth-2].token, ValueStack[ValueStack.Depth-1].attributeListSyntaxList); }
        break;
      case 28: // attr_stmt -> NODE, attr_list
{ CurrentSemanticValue.entity = CreateAttachedAttributesSyntax(ValueStack[ValueStack.Depth-2].token, ValueStack[ValueStack.Depth-1].attributeListSyntaxList); }
        break;
      case 29: // attr_stmt -> EDGE, attr_list
{ CurrentSemanticValue.entity = CreateAttachedAttributesSyntax(ValueStack[ValueStack.Depth-2].token, ValueStack[ValueStack.Depth-1].attributeListSyntaxList); }
        break;
      case 30: // opt_attr_list -> /* empty */
{ }
        break;
      case 31: // opt_attr_list -> attr_list
{ CurrentSemanticValue.attributeListSyntaxList = ValueStack[ValueStack.Depth-1].attributeListSyntaxList; }
        break;
      case 32: // attr_list -> '[', ']'
{ CurrentSemanticValue.attributeListSyntaxList = CreateAttributeListSyntaxList(CreateToken(ValueStack[ValueStack.Depth-2]), default, CreateToken(ValueStack[ValueStack.Depth-1])); }
        break;
      case 33: // attr_list -> '[', a_list, ']'
{ CurrentSemanticValue.attributeListSyntaxList = CreateAttributeListSyntaxList(CreateToken(ValueStack[ValueStack.Depth-3]), ValueStack[ValueStack.Depth-2].attributeSyntaxList, CreateToken(ValueStack[ValueStack.Depth-1])); }
        break;
      case 34: // a_list -> avPair
{ CurrentSemanticValue.attributeSyntaxList = new(); CurrentSemanticValue.attributeSyntaxList.Add((DotAttributeNode)ValueStack[ValueStack.Depth-1].entity); }
        break;
      case 35: // a_list -> avPair, a_list
{ CurrentSemanticValue.attributeSyntaxList = ValueStack[ValueStack.Depth-1].attributeSyntaxList; Prepend(ValueStack[ValueStack.Depth-1].attributeSyntaxList, (DotAttributeNode)ValueStack[ValueStack.Depth-2].entity); }
        break;
      case 36: // a_list -> avPair, ',', a_list
{ CurrentSemanticValue.attributeSyntaxList = ValueStack[ValueStack.Depth-1].attributeSyntaxList;
                                var attr = (DotAttributeNode)ValueStack[ValueStack.Depth-3].entity;
                                attr.SemicolonOrCommaToken = CreateToken(ValueStack[ValueStack.Depth-2]); 
                                Prepend(ValueStack[ValueStack.Depth-1].attributeSyntaxList, attr); }
        break;
      case 37: // a_list -> avPair, ';', a_list
{ CurrentSemanticValue.attributeSyntaxList = ValueStack[ValueStack.Depth-1].attributeSyntaxList; 
                                var attr = (DotAttributeNode)ValueStack[ValueStack.Depth-3].entity;
                                attr.SemicolonOrCommaToken = CreateToken(ValueStack[ValueStack.Depth-2]); 
                                Prepend(ValueStack[ValueStack.Depth-1].attributeSyntaxList, attr); }
        break;
      case 38: // avPair -> id, '=', id
{ CurrentSemanticValue.entity = CreateAttributeSyntax(ValueStack[ValueStack.Depth-3].token, CreateToken(ValueStack[ValueStack.Depth-2]), ValueStack[ValueStack.Depth-1].token, default); }
        break;
      case 39: // avPair -> id
{ CurrentSemanticValue.entity = CreateAttributeSyntax(ValueStack[ValueStack.Depth-1].token, default, default, default); }
        break;
      case 40: // node_id -> id
{ CurrentSemanticValue.entity = CreateVertexIdentifierSyntax(ValueStack[ValueStack.Depth-1].token, default, default, default, default); }
        break;
      case 41: // node_id -> id, ':', id
{ CurrentSemanticValue.entity = CreateVertexIdentifierSyntax(ValueStack[ValueStack.Depth-3].token, CreateToken(ValueStack[ValueStack.Depth-2]), ValueStack[ValueStack.Depth-1].token, default, default); }
        break;
      case 42: // node_id -> id, ':', id, ':', id
{ CurrentSemanticValue.entity = CreateVertexIdentifierSyntax(ValueStack[ValueStack.Depth-5].token, CreateToken(ValueStack[ValueStack.Depth-4]), ValueStack[ValueStack.Depth-3].token, CreateToken(ValueStack[ValueStack.Depth-2]), ValueStack[ValueStack.Depth-1].token); }
        break;
      case 43: // id -> ID
{ CurrentSemanticValue.token = ValueStack[ValueStack.Depth-1].token; }
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((DotTokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((DotTokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }


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
}
}
