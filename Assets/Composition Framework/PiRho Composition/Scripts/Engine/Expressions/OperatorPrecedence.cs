namespace PiRhoSoft.Composition.Engine
{
	public struct OperatorPrecedence
	{
		public static OperatorPrecedence Default = LeftAssociative(0);
		public static OperatorPrecedence Assignment = RightAssociative(10);
		public static OperatorPrecedence Ternary = RightAssociative(20);
		public static OperatorPrecedence Or = LeftAssociative(30);
		public static OperatorPrecedence And = LeftAssociative(40);
		public static OperatorPrecedence Equality = LeftAssociative(50);
		public static OperatorPrecedence Comparison = LeftAssociative(60);
		public static OperatorPrecedence Addition = LeftAssociative(70);
		public static OperatorPrecedence Multiplication = LeftAssociative(80);
		public static OperatorPrecedence Exponentiation = RightAssociative(90);
		public static OperatorPrecedence Prefix = LeftAssociative(100);
		public static OperatorPrecedence Postfix = LeftAssociative(110);
		public static OperatorPrecedence MemberAccess = LeftAssociative(200);

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
