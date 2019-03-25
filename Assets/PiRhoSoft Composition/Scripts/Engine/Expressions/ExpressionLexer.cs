using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ExpressionTokenizeException : Exception
	{
		public int Location;

		public ExpressionTokenizeException(int location, string error) : base(error) => Location = location;
		public ExpressionTokenizeException(int location, string errorFormat, params object[] arguments) : this(location, string.Format(errorFormat, arguments)) { }
	}

	public static class ExpressionLexer
	{
		private const string _invalidTokenException = "invalid character '{0}'";

		private const char _lookupOpen = '[';
		private const char _lookupClose = ']';
		private const char _stringOpen = '\"';
		private const char _stringClose = '\"';
		private const char _groupOpen = '(';
		private const char _groupClose = ')';
		private const char _separator = ',';
		private const char _colorOpen = '#';

		private const string _trueLiteral = "true";
		private const string _falseLiteral = "false";
		private const string _nullLiteral = "null";

		private const string _sentinelCharacters = ";\n";
		private const string _operatorCharacters = "+-!^*/%<=>&|?:";
		private const string _hexCharacters = "0123456789ABCDEFabcdef";

		private static bool IsSentinelCharacter(char c) => _sentinelCharacters.IndexOf(c) >= 0;
		private static bool IsIdentifierStartCharacter(char c) => char.IsLetter(c) || c == '_';
		private static bool IsIdentifierCharacter(char c) => char.IsLetterOrDigit(c) || c == '_' || c == VariableReference.Separator || c == VariableReference.LookupOpen || c == VariableReference.LookupClose;
		private static bool IsHexCharacter(char c) => _hexCharacters.IndexOf(c) >= 0;
		private static bool IsOperatorCharacter(char c) => _operatorCharacters.IndexOf(c) >= 0;

		public static List<ExpressionToken> Tokenize(string input)
		{
			var tokens = new List<ExpressionToken>();
			var whitespace = string.Empty;
			var start = 0;

			while (start < input.Length)
			{
				var c = input[start];

				if (char.IsWhiteSpace(c) && !IsSentinelCharacter(c))
				{
					whitespace += c;
					start++;
				}
				else
				{
					if (IsSentinelCharacter(c)) AddSentinel(tokens, input, start, ref start);
					else if (char.IsDigit(c)) AddInteger(tokens, input, start, ref start, whitespace);
					else if (c == _stringOpen) AddString(tokens, input, start, ref start, _stringClose);
					else if (IsIdentifierStartCharacter(c)) AddIdentifier(tokens, input, start, ref start, whitespace);
					else if (c == _lookupOpen) AddStartLookup(tokens, input, start, ref start);
					else if (c == _lookupClose) AddEndLookup(tokens, input, start, ref start);
					else if (c == _groupOpen) AddStartGroup(tokens, input, start, ref start);
					else if (c == _groupClose) AddEndGroup(tokens, input, start, ref start);
					else if (c == _separator) AddSeparator(tokens, input, start, ref start);
					else if (c == _colorOpen) AddColor(tokens, input, start, ref start);
					else if (IsOperatorCharacter(c)) AddOperator(tokens, input, start, ref start);
					else throw new ExpressionTokenizeException(start, _invalidTokenException, c);

					whitespace = string.Empty;
				}
			}

			return tokens;
		}

		private static int SkipInteger(string input, int start)
		{
			while (start < input.Length && char.IsDigit(input[start]))
				++start;

			return start;
		}

		private static int SkipIdentifier(string input, int start)
		{
			while (start < input.Length && IsIdentifierCharacter(input[start]))
				++start;

			return start;
		}

		private static int SkipOperator(string input, int start)
		{
			while (start < input.Length && IsOperatorCharacter(input[start]))
				++start;

			return start;
		}

		private static int SkipHex(string input, int start)
		{
			while (start < input.Length && IsHexCharacter(input[start]))
				++start;

			return start;
		}

		private static void AddInteger(List<ExpressionToken> tokens, string input, int start, ref int end, string whitespace)
		{
			end = SkipInteger(input, end + 1);

			if (end < input.Length && char.IsLetter(input[end]))
			{
				AddIdentifier(tokens, input, start, ref end, whitespace);
			}
			else if (end < input.Length && input[end] == '.')
			{
				AddNumber(tokens, input, start, ref end, whitespace);
			}
			else
			{
				var text = input.Substring(start, end - start);
				var integer = int.Parse(text);

				AddToken(tokens, new ExpressionToken { Location = start, Type = ExpressionTokenType.Integer, Text = text, Integer = integer, Number = integer }, whitespace);
			}
		}

		private static void AddNumber(List<ExpressionToken> tokens, string input, int start, ref int end, string whitespace)
		{
			end = SkipInteger(input, end + 1);

			var text = input.Substring(start, end - start);
			var number = float.Parse(text);

			AddToken(tokens, new ExpressionToken { Location = start, Type = ExpressionTokenType.Number, Text = text, Integer = (int)number, Number = number }, whitespace);
		}

		private static void AddString(List<ExpressionToken> tokens, string input, int start, ref int end, char delimiter)
		{
			end++;

			while (end < input.Length && input[end] != delimiter)
				end++;

			var text = input.Substring(start + 1, end - start - 1);
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.String, Text = text });

			end++;
		}

		private static void AddIdentifier(List<ExpressionToken> tokens, string input, int start, ref int end, string whitespace)
		{
			end = SkipIdentifier(input, end + 1);

			var text = input.Substring(start, end - start);
			var type = ExpressionTokenType.Identifier;

			if (end < input.Length && input[end] == _groupOpen)
			{
				++end;
				type = ExpressionTokenType.Command;
			}

			switch (text)
			{
				case _trueLiteral: AddToken(tokens, new ExpressionToken { Location = start, Type = ExpressionTokenType.Boolean, Text = text, Integer = 1, Number = 1.0f }, whitespace); break;
				case _falseLiteral: AddToken(tokens, new ExpressionToken { Location = start, Type = ExpressionTokenType.Boolean, Text = text, Integer = 0, Number = 0.0f }, whitespace); break;
				case _nullLiteral: AddToken(tokens, new ExpressionToken { Location = start, Type = ExpressionTokenType.Null, Text = text, Integer = 0, Number = 0.0f }, whitespace); break;
				default: AddToken(tokens, new ExpressionToken { Location = start, Type = type, Text = text }, whitespace); break;
			}
		}

		private static void AddStartLookup(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.StartLookup });
			end = start + 1;
		}

		private static void AddEndLookup(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.EndLookup });
			end = start + 1;
		}

		private static void AddStartGroup(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.StartGroup });
			end = start + 1;
		}

		private static void AddEndGroup(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.EndGroup });
			end = start + 1;
		}

		private static void AddSeparator(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.Separator });
			end = start + 1;
		}

		private static void AddSentinel(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			if (tokens.Count > 0 && tokens[tokens.Count - 1].Type != ExpressionTokenType.Sentinel)
				tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.Sentinel });

			end = start + 1;
		}

		private static void AddColor(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			end = SkipHex(input, end + 1);

			if ((end - start) % 2 == 0)
				throw new ExpressionTokenizeException(start, _invalidTokenException);

			var r = GetHexValue(input, start + 1, end);
			var g = GetHexValue(input, start + 3, end);
			var b = GetHexValue(input, start + 5, end);
			var a = GetHexValue(input, start + 7, end);

			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.Color, Color = new Color(r, g, b, a) });

			end++;
		}

		private static float GetHexValue(string input, int start, int end)
		{
			if (start + 2 > end)
				return 1.0f;

			var text = input.Substring(start, 2);
			var value = byte.Parse(text, System.Globalization.NumberStyles.AllowHexSpecifier);
			return value / 255.0f;
		}

		private static void AddOperator(List<ExpressionToken> tokens, string input, int start, ref int end)
		{
			end = SkipOperator(input, end + 1);
			var text = input.Substring(start, end - start);
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.Operator, Text = text });
		}

		private static void AddToken(List<ExpressionToken> tokens, ExpressionToken token, string whitespace)
		{
			if (tokens.Count > 0)
			{
				var previous = tokens[tokens.Count - 1];

				if (previous.Type == ExpressionTokenType.Boolean || previous.Type == ExpressionTokenType.Integer || previous.Type == ExpressionTokenType.Number || previous.Type == ExpressionTokenType.Null || previous.Type == ExpressionTokenType.Identifier)
				{
					if (token.Type == ExpressionTokenType.Boolean || token.Type == ExpressionTokenType.Integer || token.Type == ExpressionTokenType.Number || token.Type == ExpressionTokenType.Null || token.Type == ExpressionTokenType.Identifier)
					{
						previous.Text = previous.Text + whitespace + token.Text;
						previous.Type = ExpressionTokenType.Identifier;
						return;
					}
					else if (token.Type == ExpressionTokenType.Command)
					{
						previous.Text = previous.Text + whitespace + token.Text;
						previous.Type = ExpressionTokenType.Command;
						return;
					}
				}
			}

			tokens.Add(token);
		}
	}
}
