using System;
using System.Collections.Generic;

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

		private const char _sentinelCharacter = ';';
		private const string _operatorCharacters = "+-!^*/%<=>&|?:";

		public static List<ExpressionToken> Tokenize(string input)
		{
			var tokens = new List<ExpressionToken>();
			var whitespace = "";
			var start = 0;

			while (start < input.Length)
			{
				var c = input[start];

				if (char.IsWhiteSpace(c))
				{
					whitespace += c;
					start++;
				}
				else
				{
					if (c == _sentinelCharacter) AddSentinel(tokens, input, start, ref start);
					else if (char.IsDigit(c)) AddInteger(tokens, input, start, ref start, whitespace);
					else if (c == '\"' || c == '\'') AddString(tokens, input, start, ref start, c);
					else if (IsIdentifierStartCharacter(c)) AddIdentifier(tokens, input, start, ref start, whitespace);
					else if (c == '(') AddStartGroup(tokens, input, start, ref start);
					else if (c == ')') AddEndGroup(tokens, input, start, ref start);
					else if (c == ',') AddSeparator(tokens, input, start, ref start);
					else if (IsOperatorCharacter(c)) AddOperator(tokens, input, start, ref start);
					else throw new ExpressionTokenizeException(start, _invalidTokenException, c);

					whitespace = "";
				}
			}

			return tokens;
		}

		private static bool IsIdentifierStartCharacter(char c)
		{
			return char.IsLetter(c) || c == '_';
		}

		private static bool IsIdentifierCharacter(char c)
		{
			return char.IsLetterOrDigit(c) || c == '_' || c == '[' || c == ']' || c == '.';
		}

		private static bool IsOperatorCharacter(char c)
		{
			return _operatorCharacters.IndexOf(c) >= 0;
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

			if (end < input.Length && input[end] == '(')
			{
				++end;
				type = ExpressionTokenType.Command;
			}

			var keyword = text.ToLowerInvariant();

			switch (keyword)
			{
				case "true": AddToken(tokens, new ExpressionToken { Location = start, Type = ExpressionTokenType.Boolean, Text = text, Integer = 1, Number = 1.0f }, whitespace); break;
				case "false": AddToken(tokens, new ExpressionToken { Location = start, Type = ExpressionTokenType.Boolean, Text = text, Integer = 0, Number = 0.0f }, whitespace); break;
				default: AddToken(tokens, new ExpressionToken { Location = start, Type = type, Text = text }, whitespace); break;
			}
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
			tokens.Add(new ExpressionToken { Location = start, Type = ExpressionTokenType.Sentinel });
			end = start + 1;
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

				if (previous.Type == ExpressionTokenType.Boolean || previous.Type == ExpressionTokenType.Integer || previous.Type == ExpressionTokenType.Number || previous.Type == ExpressionTokenType.Identifier)
				{
					if (token.Type == ExpressionTokenType.Boolean || token.Type == ExpressionTokenType.Integer || token.Type == ExpressionTokenType.Number || token.Type == ExpressionTokenType.Identifier)
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
