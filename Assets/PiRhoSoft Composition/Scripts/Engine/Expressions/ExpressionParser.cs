using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ExpressionParseException : Exception
	{
		public int Location;

		public ExpressionParseException(int location, string error) : base(error) => Location = location;
		public ExpressionParseException(int location, string errorFormat, params object[] arguments) : this(location, string.Format(errorFormat, arguments)) { }
	}

	public class ExpressionParser
	{
		private const string _duplicateCommandError = "(CEPDC) Failed to add Command '{0}': a Command with that name has already been added";
		private const string _duplicateInfixOperatorError = "(CEPDIO) Failed to add infix operator '{0}': an infix operator with that symbol has already been added";
		private const string _duplicatePrefixOperatorError = "(CEPDPO) Failed to add prefix operator '{0}': a prefix operator with that symbol has already been added";
		private const string _missingCommandError = "(CEPMC) Failed to remove Command '{0}': a Command with that name has not been added";
		private const string _endOfInputException = "Expression is incomplete";
		private const string _invalidInfixOperatorException = "{0} is not a valid infix operator";
		private const string _invalidPrefixOperatorException = "{0} is not a valid prefix operator";
		private const string _invalidToken1Exception = "{0} expected";
		private const string _invalidToken3Exception = "{0}, {1}, or {2} expected";

		private static Dictionary<string, Command> _commands = new Dictionary<string, Command>();
		private static Dictionary<string, OperatorCreator> _prefixOperators = new Dictionary<string, OperatorCreator>();
		private static Dictionary<string, OperatorCreator> _infixOperators = new Dictionary<string, OperatorCreator>();
		private static Dictionary<string, OperatorPrecedence> _precedences = new Dictionary<string, OperatorPrecedence>();
		
		private List<ExpressionToken> _tokens;
		private int _index;

		public static void AddCommand(string name, Command command)
		{
			var n = name.ToLowerInvariant();

			if (!_commands.ContainsKey(n))
				_commands.Add(n, command);
			else
				Debug.LogErrorFormat(_duplicateCommandError, name);
		}

		public static void RemoveCommand(string name)
		{
			var n = name.ToLowerInvariant();

			if (!_commands.Remove(n))
				Debug.LogErrorFormat(_missingCommandError, name);
		}

		public static Command GetCommand(string name)
		{
			return _commands.TryGetValue(name.ToLowerInvariant(), out Command command) ? command : null;
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

		public static void AddTernaryOperator(string symbol, OperatorPrecedence precedence)
		{
			_precedences.Add(symbol, precedence);
		}

		public static List<Operation> Parse(List<ExpressionToken> tokens)
		{
			var parser = new ExpressionParser(tokens);
			var expressions = new List<Operation>();

			while (parser._index < parser._tokens.Count)
			{
				var expression = parser.Parse(0);
				expressions.Add(expression);

				if (parser._index < parser._tokens.Count)
					parser.SkipNextToken(ExpressionTokenType.Sentinel);
			}

			return expressions;
		}

		private abstract class OperatorCreator
		{
			public abstract Operation Create();
		}

		private class OperatorCreator<OperatorType> : OperatorCreator where OperatorType : Operation, new()
		{
			public override Operation Create() => new OperatorType();
		}

		private static Operation CreatePrefixOperator(ExpressionToken token, Operation right)
		{
			if (_prefixOperators.TryGetValue(token.Text, out OperatorCreator creator))
			{
				var op = creator.Create() as PrefixOperation;
				op.Symbol = token.Text;
				op.Right = right;

				return op;
			}
			else
			{
				throw new ExpressionParseException(token.Location, _invalidPrefixOperatorException, token.Text);
			}
		}

		private static Operation CreateInfixOperator(Operation left, ExpressionToken token, Operation right)
		{
			if (_infixOperators.TryGetValue(token.Text, out OperatorCreator creator))
			{
				var op = creator.Create() as InfixOperation;
				op.Left = left;
				op.Symbol = token.Text;
				op.Right = right;
				return op;
			}
			else
			{
				throw new ExpressionParseException(token.Location, _invalidInfixOperatorException, token.Text);
			}
		}

		private static Operation CreateTernaryOperator(Operation left, Operation trueBranch, Operation falseBranch)
		{
			return new TernaryOperator { Condition = left, TrueBranch = trueBranch, FalseBranch = falseBranch };
		}

		private ExpressionParser(List<ExpressionToken> tokens)
		{
			_tokens = tokens;
			_index = 0;
		}

		private Operation Parse(int precedence)
		{
			var expression = ParsePrefix();

			while (_index < _tokens.Count && precedence < GetPrecedence(_tokens[_index]))
				expression = ParseInfix(expression);

			return expression;
		}

		private Operation ParsePrefix()
		{
			var token = TakeNextToken();

			switch (token.Type)
			{
				case ExpressionTokenType.Boolean: return new LiteralOperation(VariableValue.Create(token.Integer == 0 ? false : true));
				case ExpressionTokenType.Integer: return new LiteralOperation(VariableValue.Create(token.Integer));
				case ExpressionTokenType.Number: return new LiteralOperation(VariableValue.Create(token.Number));
				case ExpressionTokenType.String: return new LiteralOperation(VariableValue.Create(token.Text));
				case ExpressionTokenType.Null: return new LiteralOperation(VariableValue.Create(VariableType.Null));
				case ExpressionTokenType.Identifier: return new LookupOperation(token.Text);
				case ExpressionTokenType.StartGroup: return ParseGroup();
				case ExpressionTokenType.Command: return ParseCommand(token);
				case ExpressionTokenType.Operator: return ParsePrefixOperator(token);
				default: throw new ExpressionParseException(token.Location, _invalidToken3Exception, ExpressionTokenType.Command, ExpressionTokenType.Operator, ExpressionTokenType.Identifier);
			}
		}

		private Operation ParseInfix(Operation left)
		{
			var token = TakeNextToken();

			if (token.Type == ExpressionTokenType.Operator)
			{
				if (token.Text == "?")
					return ParseTernaryOperator(token, left);
				else
					return ParseInfixOperator(token, left);
			}
			else
			{
				throw new ExpressionParseException(token.Location, _invalidToken1Exception, ExpressionTokenType.Operator);
			}
		}

		private Operation ParseGroup()
		{
			var expression = Parse(0);
			SkipNextToken(ExpressionTokenType.EndGroup);
			return expression;
		}

		private Operation ParseCommand(ExpressionToken token)
		{
			var nextToken = ViewNextToken();
			var parameters = new List<Operation>();

			while (nextToken.Type != ExpressionTokenType.EndGroup)
			{
				var parameter = Parse(0);
				parameters.Add(parameter);
				nextToken = ViewNextToken();

				if (nextToken.Type == ExpressionTokenType.Separator)
				{
					TakeNextToken();
					nextToken = ViewNextToken();
				}
			}

			SkipNextToken(ExpressionTokenType.EndGroup);
			return new CommandOperation(token.Text, parameters);
		}

		private Operation ParsePrefixOperator(ExpressionToken token)
		{
			var right = Parse(5);
			return CreatePrefixOperator(token, right);
		}

		private Operation ParseInfixOperator(ExpressionToken token, Operation left)
		{
			var precedence = GetAssociativePrecedence(token);
			var right = Parse(precedence);
			return CreateInfixOperator(left, token, right);
		}

		private Operation ParseTernaryOperator(ExpressionToken token, Operation left)
		{
			var precedence = GetAssociativePrecedence(token);
			var trueBranch = Parse(precedence);

			SkipNextToken(ExpressionTokenType.Operator, ":");

			var falseBranch = Parse(precedence);

			return CreateTernaryOperator(left, trueBranch, falseBranch);
		}

		private ExpressionToken TakeNextToken()
		{
			if (_index < _tokens.Count)
				return _tokens[_index++];
			else
				throw new ExpressionParseException(0, _endOfInputException);
		}

		private ExpressionToken ViewNextToken()
		{
			if (_index < _tokens.Count)
				return _tokens[_index];
			else
				throw new ExpressionParseException(0, _endOfInputException);
		}

		private void SkipNextToken(ExpressionTokenType type)
		{
			var token = TakeNextToken();

			if (token.Type != type)
				throw new ExpressionParseException(token.Location, _invalidToken1Exception, type);
		}

		private void SkipNextToken(ExpressionTokenType type, string text)
		{
			var token = TakeNextToken();

			if (token.Type != type || token.Text != text)
				throw new ExpressionParseException(token.Location, _invalidToken1Exception, text);
		}

		private int GetPrecedence(ExpressionToken token)
		{
			switch (token.Type)
			{
				case ExpressionTokenType.Sentinel: return int.MinValue;
				case ExpressionTokenType.Boolean: return int.MaxValue - 1;
				case ExpressionTokenType.Integer: return int.MaxValue - 1;
				case ExpressionTokenType.Number: return int.MaxValue - 1;
				case ExpressionTokenType.Identifier: return int.MaxValue - 1;
				case ExpressionTokenType.Command: return int.MaxValue - 1;
				case ExpressionTokenType.StartGroup: return int.MaxValue;
				case ExpressionTokenType.EndGroup: return 0;
				case ExpressionTokenType.Separator: return 0;
			}

			OperatorPrecedence precedence;
			return _precedences.TryGetValue(token.Text, out precedence) ? precedence.Value : 0;
		}

		private int GetAssociativePrecedence(ExpressionToken token)
		{
			OperatorPrecedence precedence;
			return _precedences.TryGetValue(token.Text, out precedence) ? precedence.AssociativeValue : 0;
		}

		static ExpressionParser()
		{
			AddPrefixOperator<NegateOperator>("-");
			AddPrefixOperator<InvertOperator>("!");

			AddInfixOperator<AddOperator>("+", OperatorPrecedence.Addition);
			AddInfixOperator<SubtractOperator>("-", OperatorPrecedence.Addition);
			AddInfixOperator<MultiplyOperator>("*", OperatorPrecedence.Multiplication);
			AddInfixOperator<DivideOperator>("/", OperatorPrecedence.Multiplication);
			AddInfixOperator<ModuloOperator>("%", OperatorPrecedence.Multiplication);
			AddInfixOperator<ExponentOperator>("^", OperatorPrecedence.Exponentiation);
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

			AddTernaryOperator("?", OperatorPrecedence.Ternary);

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

			AddCommand("PI", new ConstantCommand(VariableValue.Create(Mathf.PI)));
			AddCommand("Deg2Rad", new ConstantCommand(VariableValue.Create(Mathf.Deg2Rad)));
			AddCommand("Rad2Deg", new ConstantCommand(VariableValue.Create(Mathf.Rad2Deg)));
		}
	}
}
