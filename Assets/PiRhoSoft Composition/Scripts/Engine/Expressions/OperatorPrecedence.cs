using System.Collections.Generic;
using System.Text;

namespace PiRhoSoft.CompositionEngine
{
	public struct OperatorPrecedence
	{
		public static OperatorPrecedence Assignment = RightAssociative(10);
		public static OperatorPrecedence Or = LeftAssociative(20);
		public static OperatorPrecedence And = LeftAssociative(30);
		public static OperatorPrecedence Ternary = RightAssociative(40);
		public static OperatorPrecedence Equality = LeftAssociative(50);
		public static OperatorPrecedence Comparison = LeftAssociative(60);
		public static OperatorPrecedence Addition = LeftAssociative(70);
		public static OperatorPrecedence Multiplication = LeftAssociative(80);
		public static OperatorPrecedence Exponentiation = RightAssociative(90);

		public static OperatorPrecedence LeftAssociative(int value)
		{
			return new OperatorPrecedence { Value = value, AssociativeValue = value };
		}

		public static OperatorPrecedence RightAssociative(int value)
		{
			return new OperatorPrecedence { Value = value, AssociativeValue = value - 1 };
		}

		public int Value { get; private set; }
		public int AssociativeValue { get; private set; }
	}
}
