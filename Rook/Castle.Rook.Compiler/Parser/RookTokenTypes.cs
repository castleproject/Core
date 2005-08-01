// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// $ANTLR 2.7.5 (20050128): "lang4.g" -> "RookLexer.cs"$

	using System.Text;
	using System.Collections;

	using Castle.Rook.Compiler.AST;
	using Castle.Rook.Compiler.Services;

namespace Castle.Rook.Compiler.Parser
{
	public class RookTokenTypes
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int CLASS = 4;
		public const int DO = 5;
		public const int END = 6;
		public const int DEF = 7;
		public const int OPERATOR = 8;
		public const int BEGIN = 9;
		public const int WHILE = 10;
		public const int TYPE = 11;
		public const int METHOD_CALL = 12;
		public const int SUPER_CTOR_CALL = 13;
		public const int POST_INC = 14;
		public const int POST_DEC = 15;
		public const int EXPR = 16;
		public const int ELIST = 17;
		public const int INDEX_OP = 18;
		public const int UNARY_MINUS = 19;
		public const int UNARY_PLUS = 20;
		public const int TYPECAST = 21;
		public const int ARRAY_DECLARATOR = 22;
		public const int NUM_INT = 23;
		public const int NUM_DOUBLE = 24;
		public const int NUM_FLOAT = 25;
		public const int NUM_LONG = 26;
		public const int STATEMENT_END = 27;
		public const int IDENT = 28;
		public const int DOT = 29;
		public const int COLONCOLON = 30;
		public const int LITERAL_as = 31;
		public const int STATICIDENT = 32;
		public const int INSTIDENT = 33;
		public const int SEMI = 34;
		public const int LITERAL_namespace = 35;
		public const int SL = 36;
		public const int LITERAL_self = 37;
		public const int LITERAL_override = 38;
		public const int LITERAL_abstract = 39;
		public const int LITERAL_new = 40;
		public const int LITERAL_virtual = 41;
		public const int LITERAL_public = 42;
		public const int LITERAL_private = 43;
		public const int LITERAL_protected = 44;
		public const int LITERAL_internal = 45;
		public const int COMMA = 46;
		public const int ASSIGN = 47;
		public const int LPAREN = 48;
		public const int RPAREN = 49;
		public const int LITERAL_require = 50;
		public const int LTHAN = 51;
		public const int STAR = 52;
		public const int BAND = 53;
		public const int PLUS_ASSIGN = 54;
		public const int MINUS_ASSIGN = 55;
		public const int STAR_ASSIGN = 56;
		public const int DIV_ASSIGN = 57;
		public const int MOD_ASSIGN = 58;
		public const int BAND_ASSIGN = 59;
		public const int BOR_ASSIGN = 60;
		public const int BXOR_ASSIGN = 61;
		public const int LITERAL_if = 62;
		public const int LITERAL_unless = 63;
		public const int LITERAL_until = 64;
		public const int LITERAL_redo = 65;
		public const int LITERAL_break = 66;
		public const int LITERAL_next = 67;
		public const int LITERAL_retry = 68;
		public const int LITERAL_lambda = 69;
		public const int LCURLY = 70;
		public const int RCURLY = 71;
		public const int LITERAL_raise = 72;
		public const int LITERAL_yield = 73;
		public const int BOR = 74;
		public const int LITERAL_or = 75;
		public const int LITERAL_and = 76;
		public const int LITERAL_not = 77;
		public const int LNOT = 78;
		public const int GT = 79;
		public const int EQUAL = 80;
		public const int GE = 81;
		public const int LE = 82;
		public const int NOT_EQUAL = 83;
		public const int LITERAL_is = 84;
		public const int BXOR = 85;
		public const int PLUS = 86;
		public const int MINUS = 87;
		public const int SLASH = 88;
		public const int PERCENT = 89;
		public const int BNOT = 90;
		public const int LBRACK = 91;
		public const int RBRACK = 92;
		public const int LITERAL_base = 93;
		public const int SYMBOL = 94;
		public const int STRING_LITERAL = 95;
		public const int CHAR_LITERAL = 96;
		// "nil?" = 97
		public const int DOTDOT = 98;
		public const int DOTDOTDOT = 99;
		public const int MAPASSIGN = 100;
		public const int QUESTION = 101;
		public const int DIV = 102;
		public const int INC = 103;
		public const int DEC = 104;
		public const int MOD = 105;
		public const int SR = 106;
		public const int SR_ASSIGN = 107;
		public const int BSR = 108;
		public const int BSR_ASSIGN = 109;
		public const int SL_ASSIGN = 110;
		public const int LOR = 111;
		public const int LAND = 112;
		public const int COLON = 113;
		public const int NEWLINE = 114;
		public const int SL_NEWLINE = 115;
		public const int SL_COMMENT = 116;
		public const int WS = 117;
		public const int ESC = 118;
		public const int HEX_DIGIT = 119;
		public const int VOCAB = 120;
		public const int NUMBER = 121;
		public const int Int = 122;
		public const int NonZeroDigit = 123;
		public const int FloatTrailer = 124;
		public const int Exponent = 125;
		public const int CONTINUED_LINE = 126;
		
	}
}
