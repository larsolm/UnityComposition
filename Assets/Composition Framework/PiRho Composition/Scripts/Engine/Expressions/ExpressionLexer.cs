using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	public static class ExpressionLexer
	{
		internal const string NullConstant = "null";
		internal const string TrueConstant = "true";
		internal const string FalseConstant = "false";
		internal const string PiConstant = "PI";
		internal const string Deg2RadConstant = "Deg2Rad";
		internal const string Rad2DegConstant = "Rad2Deg";

		internal const string TernaryOperator = "?";
		internal const string CastOperator = VariableReference.Cast;
		internal const string TestOperator = "is";

		internal const char StringSymbol = StringVariableHandler.Symbol;
		internal const char ColorSymbol = ColorVariableHandler.Symbol;
		internal const char MemberAccessSymbol = VariableReference.Separator;
		internal const char LookupOpenSymbol = VariableReference.LookupOpen;
		internal const char LookupCloseSymbol = VariableReference.LookupClose;
		internal const char GroupOpenSymbol = '(';
		internal const char GroupCloseSymbol = ')';
		internal const char SeparatorSymbol = ',';
		internal const char AlternationSymbol = ':';

		private const string _sentinelCharacters = ";\n";
		private const string _operatorCharacters = "+-!^*/%<=>&|?.";
		private const string _hexCharacters = "0123456789ABCDEFabcdef";

		private const string _duplicateKeywordError = "(CELDK) Failed to add keyword '{0}': a keyword with the same text has already been added";
		private const string _duplicateConstantError = "(CELDL) Failed to add constant '{0}': a constant with the same text has already been added";

		public static List<ExpressionToken> Tokenize(string input)
		{
			var tokens = new List<ExpressionToken>();
			var start = 0;

			while (start < input.Length)
			{
				var c = input[start];

				if (IsSentinelCharacter(c)) AddSentinel(tokens, input, start, ref start);
				else if (char.IsWhiteSpace(c)) start++;
				else if (IsIdentifierStartCharacter(c)) AddIdentifier(tokens, input, start, ref start);
				else if (IsOperatorCharacter(c)) AddOperator(tokens, input, start, ref start);
				else if (char.IsDigit(c)) AddNumber(tokens, input, start, ref start);
				else if (c == StringSymbol) AddString(tokens, input, start, ref start, StringSymbol);
				else if (c == ColorSymbol) AddColor(tokens, input, start, ref start);
				else if (c == LookupOpenSymbol) AddSingleToken(tokens, ExpressionTokenType.StartLookup, ref start);
				else if (c == LookupCloseSymbol) AddSingleToken(tokens, ExpressionTokenType.EndLookup, ref start);
				else if (c == GroupOpenSymbol) AddSingleToken(tokens, ExpressionTokenType.StartGroup, ref start);
				else if (c == GroupCloseSymbol) AddSingleToken(tokens, ExpressionTokenType.EndGroup, ref start);
				else if (c == SeparatorSymbol) AddSingleToken(tokens, ExpressionTokenType.Separator, ref start);
				else if (c == AlternationSymbol) AddSingleToken(tokens, ExpressionTokenType.Alternation, ref start);
				else AddUnknown(tokens, input, start, ref start);
			}

			return tokens;
		}

		#region Reserved Words
		
		private static HashSet<string> _keywords = new HashSet<string>();
		private static Dictionary<string, VariableValue> _constants = new Dictionary<string, VariableValue>();
		private static Dictionary<string, VariableType> _types = new Dictionary<string, VariableType>();

		static ExpressionLexer()
		{
			AddKeyword(CastOperator);
			AddKeyword(TestOperator);

			AddConstant(NullConstant, VariableValue.Create((Object)null));
			AddConstant(TrueConstant, VariableValue.Create(true));
			AddConstant(FalseConstant, VariableValue.Create(false));
			AddConstant(PiConstant, VariableValue.Create(Mathf.PI));
			AddConstant(Deg2RadConstant, VariableValue.Create(Mathf.Deg2Rad));
			AddConstant(Rad2DegConstant, VariableValue.Create(Mathf.Rad2Deg));

			_types = Enum.GetValues(typeof(VariableType))
				.Cast<VariableType>()
				.ToDictionary(e => e.ToString(), e => e);
		}

		public static void AddKeyword(string text)
		{
			if (!_keywords.Contains(text))
				_keywords.Add(text);
			else
				Debug.LogErrorFormat(_duplicateKeywordError, text);
		}

		public static void AddConstant(string text, VariableValue value)
		{
			if (!_constants.ContainsKey(text))
				_constants.Add(text, value);
			else
				Debug.LogErrorFormat(_duplicateConstantError, text);
		}

		public static VariableValue GetConstant(string text)
		{
			return _constants.TryGetValue(text, out var value) ? value : VariableValue.Empty;
		}

		internal static VariableType GetType(string text)
		{
			return _types.TryGetValue(text, out var value) ? value : VariableType.Empty;
		}

		#endregion

		#region Sentinel

		private static bool IsSentinelCharacter(char c) => _sentinelCharacters.IndexOf(c) >= 0;

		private static int SkipSentinel(string input, int start)
		{
			while (start < input.Length && IsSentinelCharacter(input[start]))
				++start;

			return start;
		}

		private static void AddSentinel(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			end = SkipSentinel(input, end + 1);
			AddToken(tokens, ExpressionTokenType.Sentinel, start, end, 0);
		}

		#endregion

		#region Identifiers

		private static bool IsIdentifierStartCharacter(char c) => char.IsLetter(c) || c == '_';
		private static bool IsIdentifierCharacter(char c) => char.IsLetterOrDigit(c) || c == '_';

		private static int SkipIdentifier(string input, int start)
		{
			while (start < input.Length && IsIdentifierCharacter(input[start]))
				++start;

			return start;
		}

		private static void AddIdentifier(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			end = SkipIdentifier(input, end + 1);

			var text = input.Substring(start, end - start);
			var type = GetIdentifierType(text);

			if (type == ExpressionTokenType.Identifier || type == ExpressionTokenType.Type)
			{
				if (end < input.Length && input[end] == GroupOpenSymbol)
					AddMergeableToken(tokens, ExpressionTokenType.Command, start, end++);
				else
					AddMergeableToken(tokens, type, start, end);
			}
			else
			{
				AddToken(tokens, type, start, end, 0);
			}
		}

		private static ExpressionTokenType GetIdentifierType(string text)
		{
			if (_keywords.Contains(text))
				return ExpressionTokenType.Operator;
			else if (_constants.ContainsKey(text))
				return ExpressionTokenType.Constant;
			else if (_types.ContainsKey(text))
				return ExpressionTokenType.Type;
			else
				return ExpressionTokenType.Identifier;
		}

		#endregion

		#region Operators

		private static bool IsOperatorCharacter(char c) => _operatorCharacters.IndexOf(c) >= 0;

		private static int SkipOperator(string input, int start)
		{
			while (start < input.Length && IsOperatorCharacter(input[start]))
				++start;

			return start;
		}

		private static void AddOperator(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			end = SkipOperator(input, end + 1);
			AddToken(tokens, ExpressionTokenType.Operator, start, end, 0);
		}

		#endregion

		#region Literals

		private static bool IsHexCharacter(char c) => _hexCharacters.IndexOf(c) >= 0;

		private static int SkipInt(string input, int start)
		{
			while (start < input.Length && char.IsDigit(input[start]))
				++start;

			return start;
		}

		private static void AddNumber(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			end = SkipInt(input, end + 1);

			// checks the character after the . so something like "Object 2.property" is not interpreted as a float

			if (end < (input.Length - 1) && input[end] == MemberAccessSymbol && char.IsDigit(input[end + 1]))
			{
				end = SkipInt(input, end + 1);
				AddMergeableToken(tokens, ExpressionTokenType.Float, start, end);
			}
			else
			{
				AddMergeableToken(tokens, ExpressionTokenType.Int, start, end);
			}
		}

		private static int SkipHex(string input, int start)
		{
			while (start < input.Length && IsHexCharacter(input[start]))
				++start;

			return start;
		}

		private static void AddColor(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			end = SkipHex(input, end + 1);
			AddToken(tokens, ExpressionTokenType.Color, start, end, 1);
		}

		private static int SkipTo(string input, int start, char delimiter)
		{
			while (start < input.Length && input[start] != delimiter)
				++start;

			return start;
		}

		private static void AddString(List<ExpressionToken> tokens, string input, int start, ref int end, char delimiter)
		{
			end = SkipTo(input, end + 1, delimiter);
			AddToken(tokens, ExpressionTokenType.String, start, end, 1);
			end++;
		}

		#endregion

		#region Unknown

		private static void AddUnknown(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			end = start + 1;

			if (tokens.Count > 0 && tokens[tokens.Count - 1].Type == ExpressionTokenType.Unknown)
				MergeToken(tokens, ExpressionTokenType.Unknown, end);
			else
				AddToken(tokens, ExpressionTokenType.Unknown, start, end, 0);
		}

		#endregion

		#region Tokens

		private static void AddToken(List<ExpressionToken> tokens, ExpressionTokenType type, int start, int end, int offset)
		{
			tokens.Add(new ExpressionToken { Location = start, Type = type, Start = start + offset, End = end });
		}

		private static void AddSingleToken(List<ExpressionToken> tokens, ExpressionTokenType type, ref int start)
		{
			tokens.Add(new ExpressionToken { Location = start, Type = type, Start = start, End = start + 1 });
			++start;
		}

		private static void MergeToken(List<ExpressionToken> tokens, ExpressionTokenType type, int end)
		{
			var previous = tokens[tokens.Count - 1];
			previous.Type = type;
			previous.End = end;
		}

		private static bool IsMergeable(ExpressionTokenType type)
		{
			return type == ExpressionTokenType.Int || type == ExpressionTokenType.Float || type == ExpressionTokenType.Identifier || type == ExpressionTokenType.Type;
		}

		private static void AddMergeableToken(List<ExpressionToken> tokens, ExpressionTokenType type, int start, int end)
		{
			// This handles merging names separated by whitespace so "Thing 2" becomes a single Identifier token
			// instead of an Identifier followed by an Int.

			if (tokens.Count > 0 && IsMergeable(tokens[tokens.Count - 1].Type))
			{
				if (IsMergeable(type))
					MergeToken(tokens, ExpressionTokenType.Identifier, end);
				else if (type == ExpressionTokenType.Command)
					MergeToken(tokens, ExpressionTokenType.Command, end);
				else
					AddToken(tokens, type, start, end, 0);
			}
			else
			{
				AddToken(tokens, type, start, end, 0);
			}
		}

		#endregion
	}
}
