namespace PiRhoSoft.PargonUtilities.Engine
{
	public class ConditionalAttribute : PropertyTraitAttribute
	{
		public string Property { get; private set; }
		public string Method { get; private set; }

		public string StringValue { get; private set; }
		public int IntValue { get; private set; }
		public float FloatValue { get; private set; }
		public bool BoolValue { get; private set; }

		public ConditionalAttribute(string property, string value, bool whenEqual = true) : this(property, whenEqual)
		{
			StringValue = value;
		}

		public ConditionalAttribute(string property, int value, bool whenEqual = true) : this(property, whenEqual)
		{
			IntValue = value;
		}

		public ConditionalAttribute(string property, float value, bool whenEqual = true) : this(property, whenEqual)
		{
			FloatValue = value;
		}

		public ConditionalAttribute(string property, bool value) : base(int.MaxValue)
		{
			Property = property;
			BoolValue = value;
		}

		public ConditionalAttribute(string method) : base(int.MaxValue)
		{
			Method = method;
		}
	}
}
