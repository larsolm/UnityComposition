using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ExpressionParseException : Exception
	{
		public ExpressionToken Token;

		public ExpressionParseException(ExpressionToken token, string error) : base(error) => Token = token;
		public ExpressionParseException(ExpressionToken token, string errorFormat, params object[] arguments) : this(token, string.Format(errorFormat, arguments)) { }
	}

	public class ExpressionParser
	{
		private const string _duplicateCommandError = "(CEPDC) Failed to add Command '{0}': a Command with the same name has already been added";
		private const string _duplicatePrefixOperatorError = "(CEPDPO) Failed to add prefix operator '{0}': a prefix operator with the same symbol has already been added";
		private const string _duplicateInfixOperatorError = "(CEPDIO) Failed to add infix operator '{0}': an infix operator with the same symbol has already been added";

		private const string _invalidStatementException = "a statement cannot begin with '{0}'";
		private const string _invalidConstantException = "'{0}' is not a valid constant";
		private const string _invalidOperatorException = "'{0}' is not a valid operator";
		private const string _invalidTokenException = "expected '{0}' instead of '{1}'";
		private const string _invalidEndException = "expression is incomplete";

		private static Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();
		private static Dictionary<string, OperatorCreator> _prefixOperators = new Dictionary<string, OperatorCreator>();
		private static Dictionary<string, OperatorCreator> _infixOperators = new Dictionary<string, OperatorCreator>();
		private static Dictionary<string, OperatorPrecedence> _precedences = new Dictionary<string, OperatorPrecedence>();
		
		private string _input;
		private List<ExpressionToken> _tokens;
		private int _index;

		public static List<Operation> Parse(string input, List<ExpressionToken> tokens)
		{
			var parser = new ExpressionParser { _input = input, _tokens = tokens, _index = 0 };
			var expressions = new List<Operation>();

			while (parser._index < parser._tokens.Count)
			{
				var expression = parser.ParseLeft(OperatorPrecedence.Default);
				expressions.Add(expression);

				if (parser._index < parser._tokens.Count)
					parser.SkipToken(ExpressionTokenType.Sentinel, ";");
			}

			return expressions;
		}

		#region Commands

		public static void AddCommand(string name, ICommand command)
		{
			if (!_commands.ContainsKey(name))
				_commands.Add(name, command);
			else
				Debug.LogErrorFormat(_duplicateCommandError, name);
		}

		public static void RemoveCommand(string name)
		{
			_commands.Remove(name);
		}

		public static ICommand GetCommand(string name)
		{
			return _commands.TryGetValue(name, out ICommand command) ? command : null;
		}

		#endregion

		#region Operators

		private abstract class OperatorCreator
		{
			public abstract Operation Create();
		}

		private class OperatorCreator<OperatorType> : OperatorCreator where OperatorType : Operation, new()
		{
			public override Operation Create() => new OperatorType();
		}

		public static void AddPrefixOperator<OperatorType>(string symbol) where OperatorType : PrefixOperation, new()
		{
			if (!_prefixOperators.ContainsKey(symbol))
				_prefixOperators.Add(symbol, new OperatorCreator<OperatorType>());
			else
				Debug.LogErrorFormat(_duplicatePrefixOperatorError, symbol);
		}

		public static void AddInfixOperator<OperatorType>(string symbol, OperatorPrecedence precedence) where OperatorType : InfixOperation, new()
		{
			if (!_infixOperators.ContainsKey(symbol))
			{
				_infixOperators.Add(symbol, new OperatorCreator<OperatorType>());
				_precedences.Add(symbol, precedence);
			}
			else
			{
				Debug.LogErrorFormat(_duplicateInfixOperatorError, symbol);
			}
		}

		private Operation CreatePrefixOperator(ExpressionToken token)
		{
			var op = GetText(token);

			if (_prefixOperators.TryGetValue(op, out var creator))
				return creator.Create();
			else
				throw new ExpressionParseException(token, _invalidOperatorException, op);
		}

		private InfixOperation CreateInfixOperator(ExpressionToken token)
		{
			var op = GetText(token);

			if (_infixOperators.TryGetValue(op, out var creator))
				return creator.Create() as InfixOperation;
			else
				throw new ExpressionParseException(token, _invalidOperatorException, op);
		}

		#endregion

		#region Parsing

		public Operation ParseLeft(OperatorPrecedence precedence)
		{
			return Parse(precedence.Value);
		}

		public Operation ParseRight(OperatorPrecedence precedence)
		{
			return Parse(precedence.AssociativeValue);
		}

		private Operation Parse(int precedence)
		{
			var operation = ParsePrefix();

			while (_index < _tokens.Count && precedence < GetPrecedence(_tokens[_index]))
				operation = ParseInfix(operation);

			return operation;
		}

		private int GetPrecedence(ExpressionToken token)
		{
			switch (token.Type)
			{
				case ExpressionTokenType.Sentinel: return int.MinValue;
				case ExpressionTokenType.Constant: return int.MaxValue - 1;
				case ExpressionTokenType.Type: return int.MaxValue - 1;
				case ExpressionTokenType.Int: return int.MaxValue - 1;
				case ExpressionTokenType.Float: return int.MaxValue - 1;
				case ExpressionTokenType.String: return int.MaxValue - 1;
				case ExpressionTokenType.Color: return int.MaxValue - 1;
				case ExpressionTokenType.Identifier: return int.MaxValue - 1;
				case ExpressionTokenType.Command: return int.MaxValue - 1;
				case ExpressionTokenType.StartLookup: return OperatorPrecedence.MemberAccess.Value;
				case ExpressionTokenType.EndLookup: return 0;
				case ExpressionTokenType.StartGroup: return int.MaxValue;
				case ExpressionTokenType.EndGroup: return 0;
				case ExpressionTokenType.Separator: return 0;
				case ExpressionTokenType.Alternation: return OperatorPrecedence.Ternary.AssociativeValue;
				case ExpressionTokenType.Unknown: return 0;
			}

			var text = GetText(token);
			return _precedences.TryGetValue(text, out var precedence) ? precedence.Value : 0;
		}

		private Operation ParsePrefix()
		{
			var token = TakeToken();
			var operation = CreatePrefixOperation(token);

			operation.Parse(this, token);

			return operation;
		}

		private Operation ParseInfix(Operation left)
		{
			var token = TakeToken();
			var operation = CreateInfixOperation(token);

			operation.Setup(left);
			operation.Parse(this, token);

			return operation;
		}

		private Operation CreatePrefixOperation(ExpressionToken token)
		{
			switch (token.Type)
			{
				case ExpressionTokenType.Constant: return CreateConstantOperation(token);
				case ExpressionTokenType.Type: return CreateTypeOperation(token);
				case ExpressionTokenType.Int: return new LiteralOperation();
				case ExpressionTokenType.Float: return new LiteralOperation();
				case ExpressionTokenType.String: return new LiteralOperation();
				case ExpressionTokenType.Color: return new LiteralOperation();
				case ExpressionTokenType.Identifier: return new IdentifierOperation();
				case ExpressionTokenType.Command: return new CommandOperation();
				case ExpressionTokenType.Operator: return CreatePrefixOperator(token);
				case ExpressionTokenType.StartGroup: return new GroupOperation();
			}

			throw new ExpressionParseException(token, _invalidStatementException, GetText(token));
		}

		private InfixOperation CreateInfixOperation(ExpressionToken token)
		{
			switch (token.Type)
			{
				case ExpressionTokenType.Operator: return CreateInfixOperator(token);
				case ExpressionTokenType.StartLookup: return new LookupOperator();
				default: throw new ExpressionParseException(token, _invalidOperatorException, GetText(token));
			}
		}

		private ConstantOperation CreateConstantOperation(ExpressionToken token)
		{
			var text = GetText(token);
			var value = ExpressionLexer.GetConstant(GetText(token));

			if (value.IsEmpty)
				throw new ExpressionParseException(token, _invalidConstantException, text);

			return new ConstantOperation(value);
		}

		private TypeOperation CreateTypeOperation(ExpressionToken token)
		{
			var text = GetText(token);
			var type = ExpressionLexer.GetType(GetText(token));

			return new TypeOperation(type);
		}

		#endregion

		#region Tokens

		public string GetText(ExpressionToken token)
		{
			return _input.Substring(token.Start, token.End - token.Start);
		}

		public bool HasText(ExpressionToken token, string text)
		{
			var length = token.End - token.Start;

			if (length != text.Length)
				return false;

			for (var i = 0; i < length; i++)
			{
				if (text[i] != _input[token.Start + i])
					return false;
			}

			return true;
		}

		public bool HasToken(ExpressionTokenType type)
		{
			return _index < _tokens.Count && _tokens[_index].Type == type;
		}

		public void SkipToken(ExpressionTokenType type, string expected)
		{
			var token = TakeToken();

			if (token.Type != type)
			{
				var text = GetText(token);
				throw new ExpressionParseException(token, _invalidTokenException, expected, text);
			}
		}

		private ExpressionToken TakeToken()
		{
			if (_index < _tokens.Count)
				return _tokens[_index++];
			else
				throw new ExpressionParseException(_tokens[_tokens.Count - 1], _invalidEndException);
		}

		#endregion

		#region Built In Operators and Commands

		static ExpressionParser()
		{
			AddPrefixOperator<NegateOperator>("-");
			AddInfixOperator<AddOperator>("+", OperatorPrecedence.Addition);
			AddInfixOperator<SubtractOperator>("-", OperatorPrecedence.Addition);
			AddInfixOperator<MultiplyOperator>("*", OperatorPrecedence.Multiplication);
			AddInfixOperator<DivideOperator>("/", OperatorPrecedence.Multiplication);
			AddInfixOperator<ModuloOperator>("%", OperatorPrecedence.Multiplication);
			AddInfixOperator<ExponentOperator>("^", OperatorPrecedence.Exponentiation);

			AddPrefixOperator<IncrementOperator>("++");
			AddPrefixOperator<DecrementOperator>("--");
			AddInfixOperator<PostIncrementOperator>("++", OperatorPrecedence.Postfix);
			AddInfixOperator<PostDecrementOperator>("--", OperatorPrecedence.Postfix);

			AddPrefixOperator<InvertOperator>("!");
			AddInfixOperator<AndOperator>("&&", OperatorPrecedence.And);
			AddInfixOperator<OrOperator>("||", OperatorPrecedence.Or);

			AddInfixOperator<EqualOperator>("==", OperatorPrecedence.Equality);
			AddInfixOperator<InequalOperator>("!=", OperatorPrecedence.Equality);
			AddInfixOperator<LessOperator>("<", OperatorPrecedence.Comparison);
			AddInfixOperator<GreaterOperator>(">", OperatorPrecedence.Comparison);
			AddInfixOperator<LessOrEqualOperator>("<=", OperatorPrecedence.Comparison);
			AddInfixOperator<GreaterOrEqualOperator>(">=", OperatorPrecedence.Comparison);

			AddInfixOperator<AssignOperator>("=", OperatorPrecedence.Assignment);
			AddInfixOperator<AddAssignOperator>("+=", OperatorPrecedence.Assignment);
			AddInfixOperator<SubtractAssignOperator>("-=", OperatorPrecedence.Assignment);
			AddInfixOperator<MultiplyAssignOperator>("*=", OperatorPrecedence.Assignment);
			AddInfixOperator<DivideAssignOperator>("/=", OperatorPrecedence.Assignment);
			AddInfixOperator<ModuloAssignOperator>("%=", OperatorPrecedence.Assignment);
			AddInfixOperator<ExponentAssignOperator>("^=", OperatorPrecedence.Assignment);
			AddInfixOperator<AndAssignOperator>("&=", OperatorPrecedence.Assignment);
			AddInfixOperator<OrAssignOperator>("|=", OperatorPrecedence.Assignment);

			AddInfixOperator<AccessOperator>(".", OperatorPrecedence.MemberAccess);
			AddInfixOperator<LookupOperator>("[", OperatorPrecedence.MemberAccess);
			AddInfixOperator<CastOperator>(ExpressionLexer.CastOperator, OperatorPrecedence.MemberAccess);
			AddInfixOperator<TestOperator>(ExpressionLexer.TestOperator, OperatorPrecedence.Comparison);

			AddInfixOperator<TernaryOperator>(ExpressionLexer.TernaryOperator, OperatorPrecedence.Ternary);

			AddCommand("Abs", new AbsCommand());
			AddCommand("Acos", new AcosCommand());
			AddCommand("Asin", new AsinCommand());
			AddCommand("Atan", new AtanCommand());
			AddCommand("Ceiling", new CeilingCommand());
			AddCommand("Clamp", new ClampCommand());
			AddCommand("Cos", new CosCommand());
			AddCommand("Floor", new FloorCommand());
			AddCommand("Lerp", new LerpCommand());
			AddCommand("Log", new LogCommand());
			AddCommand("Max", new MaxCommand());
			AddCommand("Min", new MinCommand());
			AddCommand("Pow", new PowCommand());
			AddCommand("Random", new RandomCommand());
			AddCommand("Round", new RoundCommand());
			AddCommand("Sign", new SignCommand());
			AddCommand("Sin", new SinCommand());
			AddCommand("Sqrt", new SqrtCommand());
			AddCommand("Tan", new TanCommand());
			AddCommand("Truncate", new TruncateCommand());

			AddCommand("Vector2", new Vector2Command());
			AddCommand("Vector2Int", new Vector2IntCommand());
			AddCommand("Vector3", new Vector3Command());
			AddCommand("Vector3Int", new Vector3IntCommand());
			AddCommand("Vector4", new Vector4Command());
			AddCommand("Quaternion", new QuaternionCommand());
			AddCommand("Rect", new RectCommand());
			AddCommand("RectInt", new RectIntCommand());
			AddCommand("Bounds", new BoundsCommand());
			AddCommand("BoundsInt", new BoundsIntCommand());
			AddCommand("Color", new ColorCommand());
			AddCommand("List", new ListCommand());
			AddCommand("Store", new StoreCommand());

			AddCommand("Time", new TimeCommand());
			AddCommand("Realtime", new RealtimeCommand());
			AddCommand("UnscaledTime", new UnscaledTimeCommand());
		}

		#endregion
	}
}
