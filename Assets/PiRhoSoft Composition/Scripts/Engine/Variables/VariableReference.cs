using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class VariableReference
	{
		public const string Cast = "as";
		public const char Separator = '.';
		public const char LookupOpen = '[';
		public const char LookupClose = ']';

		[SerializeField] private string _variable = string.Empty;
		[SerializeField] private List<VariableToken> _tokens = new List<VariableToken>();

		public bool IsValid => string.IsNullOrEmpty(_variable) || _tokens.Count > 0;
		public bool IsAssigned => _tokens.Count > 0;
		public string StoreName => IsAssigned ? (_tokens.Count > 1 ? _tokens[0].Text : string.Empty) : string.Empty;
		public string RootName => IsAssigned ? (_tokens.Count > 1 ? _tokens[1].Text : _tokens[0].Text) : string.Empty;

		public string Variable
		{
			get
			{
				return _variable;
			}
			set
			{
				_variable = value;
				_tokens = Parse(value);
			}
		}

		public override string ToString()
		{
			return _variable;
		}

		#region Lookup

		public VariableValue GetValue(IVariableStore variables)
		{
			var value = IsAssigned ? VariableValue.Create(variables) : VariableValue.Empty;

			foreach (var token in _tokens)
			{
				switch (token.Type)
				{
					case VariableTokenType.Name: value = VariableHandler.Lookup(value, VariableValue.Create(token.Text)); break;
					case VariableTokenType.Number: value = VariableHandler.Lookup(value, VariableValue.Create(int.Parse(token.Text))); break;
					case VariableTokenType.Type: value = VariableHandler.Cast(value, token.Text); break;
				}

				if (value.IsEmpty)
					break;
			}

			return value;
		}

		#endregion

		#region Assignment

		public SetVariableResult SetValue(IVariableStore variables, VariableValue value)
		{
			if (IsAssigned)
			{
				var owner = VariableValue.Create(variables);
				return SetValue_(ref owner, value, 0);
			}
			else
			{
				return SetVariableResult.NotFound;
			}
		}

		private SetVariableResult SetValue_(ref VariableValue owner, VariableValue value, int index)
		{
			var token = _tokens[index];

			if (index == _tokens.Count - 1)
			{
				switch (token.Type)
				{
					case VariableTokenType.Name:
					{
						return VariableHandler.Apply(ref owner, VariableValue.Create(token.Text), value);
					}
					case VariableTokenType.Number:
					{
						if (int.TryParse(token.Text, out var number))
							return VariableHandler.Apply(ref owner, VariableValue.Create(number), value);

						break;
					}
					case VariableTokenType.Type:
					{
						return SetVariableResult.ReadOnly;
					}
				}
			}
			else
			{
				var lookup = VariableValue.Empty;
				var child = VariableValue.Empty;

				switch (token.Type)
				{
					case VariableTokenType.Name:
					{
						lookup = VariableValue.Create(token.Text);
						child = VariableHandler.Lookup(owner, lookup);
						break;
					}
					case VariableTokenType.Number:
					{
						if (int.TryParse(token.Text, out var number))
						{
							lookup = VariableValue.Create(number);
							child = VariableHandler.Lookup(owner, lookup);
						}

						break;
					}
					case VariableTokenType.Type:
					{
						child = VariableHandler.Cast(owner, token.Text);
						break;
					}
				}

				if (!child.IsEmpty)
				{
					// if a value is set on a struct (Vector3.x for instance), the variable value needs to be
					// reassigned to its owner

					var result = SetValue_(ref child, value, index + 1);

					if (result == SetVariableResult.Success && !child.HasReference)
						result = VariableHandler.Apply(ref owner, lookup, child);

					return result;
				}
			}

			return SetVariableResult.NotFound;
		}

		#endregion

		#region Parsing

		public enum VariableTokenType
		{
			Name,
			Number,
			Type
		}

		[Serializable]
		public class VariableToken
		{
			public VariableTokenType Type;
			public string Text;
		}

		private static List<VariableToken> Parse(string variable)
		{
			var parsed = new List<VariableToken>();
			var tokens = ExpressionLexer.Tokenize(variable);
			var state = ExpressionTokenType.Identifier;

			foreach (var token in tokens)
			{
				var text = variable.Substring(token.Start, token.End - token.Start);

				if (!CheckState(state, token))
					return new List<VariableToken>();

				switch (token.Type)
				{
					case ExpressionTokenType.Identifier:
					{
						parsed.Add(new VariableToken { Type = state == ExpressionTokenType.Command ? VariableTokenType.Type : VariableTokenType.Name, Text = text });
						state = ExpressionTokenType.Operator;
						break;
					}
					case ExpressionTokenType.Int:
					{
						parsed.Add(new VariableToken { Type = VariableTokenType.Number, Text = text });
						state = ExpressionTokenType.EndLookup;
						break;
					}
					case ExpressionTokenType.Operator:
					{
						if (text == Cast)
							state = ExpressionTokenType.Command;
						else if (text.Length == 1 && text[0] == Separator)
							state = ExpressionTokenType.Identifier;
						else
							return new List<VariableToken>();

						break;
					}
					case ExpressionTokenType.StartLookup:
					{
						state = ExpressionTokenType.Int;
						break;
					}
					case ExpressionTokenType.EndLookup:
					{
						state = ExpressionTokenType.Operator;
						break;
					}
					default:
					{
						return new List<VariableToken>();
					}
				}
			}

			return state == ExpressionTokenType.Operator ? parsed : new List<VariableToken>();
		}

		private static bool CheckState(ExpressionTokenType state, ExpressionToken token)
		{
			switch (state)
			{
				case ExpressionTokenType.Identifier: return token.Type == ExpressionTokenType.Identifier;
				case ExpressionTokenType.Command: return token.Type == ExpressionTokenType.Identifier;
				case ExpressionTokenType.Int: return token.Type == ExpressionTokenType.Int;
				case ExpressionTokenType.Operator: return token.Type == ExpressionTokenType.Operator || token.Type == ExpressionTokenType.StartLookup;
				case ExpressionTokenType.EndLookup: return token.Type == ExpressionTokenType.EndLookup;
				default: return false;
			}
		}

		#endregion
	}
}
