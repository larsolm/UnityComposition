﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public class VariableReferenceAttribute : Attribute
	{
		public VariableType Type { get; private set; }
		public VariableConstraint Constraint { get; private set; }

		public VariableReferenceAttribute(VariableType type) => Type = type;
		public VariableReferenceAttribute(int minimum, int maximum) { Type = VariableType.Int; Constraint = new IntConstraint(minimum, maximum); }
		public VariableReferenceAttribute(bool noMinimum, int maximum) { Type = VariableType.Int; Constraint = new IntConstraint(null, maximum); }
		public VariableReferenceAttribute(int minimum, bool noMaximum) { Type = VariableType.Int; Constraint = new IntConstraint(minimum, null); }
		public VariableReferenceAttribute(float minimum, float maximum) { Type = VariableType.Float; Constraint = new FloatConstraint(minimum, maximum); }
		public VariableReferenceAttribute(bool noMinimum, float maximum) { Type = VariableType.Float; Constraint = new FloatConstraint(null, maximum); }
		public VariableReferenceAttribute(float minimum, bool noMaximum) { Type = VariableType.Float; Constraint = new FloatConstraint(minimum, null); }
		public VariableReferenceAttribute(string[] values) { Type = VariableType.String; Constraint = new StringConstraint(new List<string>(values)); }

		public VariableReferenceAttribute(Type type)
		{
			if (Variable.IsValidEnumType(type))
				Constraint = new EnumConstraint(type);
			else
				Constraint = new ObjectConstraint(type);
		}

		public VariableDefinition GetDefinition(string name)
		{
			if (Constraint != null)
				return new VariableDefinition(name, Constraint);
			else
				return new VariableDefinition(name, Type);
		}
	}

	[Serializable]
	public class VariableReference
	{
		public const string Cast = "as";
		public const char Separator = '.';
		public const char LookupOpen = '[';
		public const char LookupClose = ']';

		[SerializeField] protected string _variable = string.Empty;
		[SerializeField] private List<VariableToken> _tokens = new List<VariableToken>();

		public bool IsValid => string.IsNullOrEmpty(_variable) || _tokens.Count > 0;
		public bool IsAssigned => _tokens.Count > 0;
		public string StoreName => IsAssigned ? (_tokens.Count > 1 ? _tokens[0].Text : string.Empty) : string.Empty;
		public string RootName => IsAssigned ? (_tokens.Count > 1 ? _tokens[1].Text : _tokens[0].Text) : string.Empty;
		public bool UsesStore(string storeName) => IsAssigned && StoreName == storeName;

		public List<VariableToken> Tokens
		{
			get
			{
				return _tokens;
			}
			set
			{
				_tokens = value;
				_variable = Format(value);
			}
		}

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
			return Format(_tokens);
		}

		public VariableDefinition GetDefinition() => new VariableDefinition(RootName);

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

		public static string Format(List<VariableToken> tokens)
		{
			var text = string.Empty;

			for (var i = 0; i < tokens.Count; i++)
			{
				var token = tokens[i];
				text += token.Text;

				if (token.Type == VariableTokenType.Number)
					text += LookupClose;

				if (i < tokens.Count - 1)
				{
					var nextToken = tokens[i + 1];

					switch (nextToken.Type)
					{
						case VariableTokenType.Name: text += Separator; break;
						case VariableTokenType.Number: text += LookupOpen; break;
						case VariableTokenType.Type: text += $" {Cast} "; break;
					}
				}
			}

			return text;
		}

		public static List<VariableToken> Parse(string variable)
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

	[Serializable]
	public class VariableLookupReference : VariableReference
	{
		public Variable GetValue(IVariableCollection variables)
		{
			var value = IsAssigned ? Composition.Variable.Object(variables) : Composition.Variable.Empty;

			foreach (var token in Tokens)
			{
				switch (token.Type)
				{
					case VariableTokenType.Name: value = VariableHandler.Lookup(value, Composition.Variable.String(token.Text)); break;
					case VariableTokenType.Number: value = VariableHandler.Lookup(value, Composition.Variable.Int(int.Parse(token.Text))); break;
					case VariableTokenType.Type: value = VariableHandler.Cast(value, token.Text); break;
				}

				if (value.IsEmpty)
					break;
			}

			return value;
		}
	}

	[Serializable]
	public class VariableAssignmentReference : VariableReference
	{
		public SetVariableResult SetValue(IVariableCollection variables, Variable value)
		{
			if (IsAssigned)
			{
				var owner = Composition.Variable.Object(variables);
				return SetValue_(ref owner, value, 0);
			}
			else
			{
				return SetVariableResult.NotFound;
			}
		}

		private SetVariableResult SetValue_(ref Variable owner, Variable value, int index)
		{
			var token = Tokens[index];

			if (index == Tokens.Count - 1)
			{
				switch (token.Type)
				{
					case VariableTokenType.Name:
					{
						return VariableHandler.Apply(ref owner, Composition.Variable.String(token.Text), value);
					}
					case VariableTokenType.Number:
					{
						if (int.TryParse(token.Text, out var number))
							return VariableHandler.Apply(ref owner, Composition.Variable.Int(number), value);

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
				var lookup = Composition.Variable.Empty;
				var child = Composition.Variable.Empty;

				switch (token.Type)
				{
					case VariableTokenType.Name:
					{
						lookup = Composition.Variable.String(token.Text);
						child = VariableHandler.Lookup(owner, lookup);
						break;
					}
					case VariableTokenType.Number:
					{
						if (int.TryParse(token.Text, out var number))
						{
							lookup = Composition.Variable.Int(number);
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

					if (result == SetVariableResult.Success && child.IsValueType)
						result = VariableHandler.Apply(ref owner, lookup, child);

					return result;
				}
			}

			return SetVariableResult.NotFound;
		}
	}
}