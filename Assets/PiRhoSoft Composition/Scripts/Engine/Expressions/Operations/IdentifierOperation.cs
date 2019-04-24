﻿using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	internal class IdentifierOperation : Operation, IAssignableOperation
	{
		private const string _invalidLookupException = "unable to find variable '{0}' on '{1}'";
		private const string _invalidCastException = "unable to cast variable '{0}' to type '{1}'";

		private const string _missingAssignmentException = "unable to assign '{0}' to missing variable '{1}' on '{2}'";
		private const string _readOnlyAssignmentException = "unable to assign '{0}' to read only variable '{1}' on '{2}'";
		private const string _mismatchedAssignmentException = "unable to assign '{0}' of type {1} to the variable '{2}' on '{3}'";

		public string Name { get; private set; }

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			Name = parser.GetText(token);
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
			builder.Append(Name);
		}

		public VariableValue GetValue(VariableValue owner)
		{
			var value = VariableHandler.Lookup(owner, VariableValue.Create(Name));

			if (value.IsEmpty)
				throw new ExpressionEvaluationException(_invalidLookupException, Name, owner);

			return value;
		}

		public SetVariableResult SetValue(ref VariableValue owner, VariableValue value)
		{
			var result = VariableHandler.Apply(ref owner, VariableValue.Create(Name), value);

			switch (result)
			{
				case SetVariableResult.NotFound: throw new ExpressionEvaluationException(_missingAssignmentException, value, Name, owner);
				case SetVariableResult.ReadOnly: throw new ExpressionEvaluationException(_readOnlyAssignmentException, value, Name, owner);
				case SetVariableResult.TypeMismatch: throw new ExpressionEvaluationException(_mismatchedAssignmentException, value, value.Type, Name, owner);
			}

			return result;
		}

		public VariableValue Cast(VariableValue owner)
		{
			var cast = VariableHandler.Cast(owner, Name);

			if (cast.IsEmpty)
				throw new ExpressionEvaluationException(_invalidCastException, owner, Name);

			return cast;
		}

		public VariableValue Test(VariableValue owner)
		{
			var result = VariableHandler.Test(owner, VariableValue.Create(Name));
			return VariableValue.Create(result);
		}
	}
}
