using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public class IdentifierOperation : Operation, IAssignableOperation
	{
		private const string _invalidLookupException = "unable to find variable '{0}' on '{1}'";
		private const string _invalidCastException = "unable to cast variable '{0}' to type '{1}'";

		private const string _missingAssignmentException = "unable to assign '{0}' to missing variable '{1}' on '{2}'";
		private const string _readOnlyAssignmentException = "unable to assign '{0}' to read only variable '{1}' on '{2}'";
		private const string _mismatchedAssignmentException = "unable to assign '{0}' of type {1} to the variable '{2}' on '{3}'";

		private string _name;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			_name = parser.GetText(token);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			return GetValue(VariableValue.Create(variables));
		}

		public SetVariableResult SetValue(IVariableStore variables, VariableValue value)
		{
			var owner = VariableValue.Create(variables);
			return SetValue(ref owner, value);
		}

		public override void ToString(StringBuilder builder)
		{
			builder.Append(_name);
		}

		public VariableValue GetValue(VariableValue owner)
		{
			var value = VariableHandler.Lookup(owner, VariableValue.Create(_name));

			if (value.IsEmpty)
				throw new ExpressionEvaluationException(_invalidLookupException, _name, owner);

			return value;
		}

		public SetVariableResult SetValue(ref VariableValue owner, VariableValue value)
		{
			var result = VariableHandler.Apply(ref owner, VariableValue.Create(_name), value);

			switch (result)
			{
				case SetVariableResult.NotFound: throw new ExpressionEvaluationException(_missingAssignmentException, value, _name, owner);
				case SetVariableResult.ReadOnly: throw new ExpressionEvaluationException(_readOnlyAssignmentException, value, _name, owner);
				case SetVariableResult.TypeMismatch: throw new ExpressionEvaluationException(_mismatchedAssignmentException, value, value.Type, _name, owner);
			}

			return result;
		}

		public VariableValue Cast(VariableValue owner)
		{
			var cast = VariableHandler.Cast(owner, _name);

			if (cast.IsEmpty)
				throw new ExpressionEvaluationException(_invalidCastException, owner, _name);

			return cast;
		}

		public VariableValue Test(VariableValue owner)
		{
			var result = VariableHandler.Test(owner, VariableValue.Create(_name));
			return VariableValue.Create(result);
		}
	}
}
