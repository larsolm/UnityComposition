namespace PiRhoSoft.PargonUtilities.Engine
{
	public class ConditionalAttribute : PropertyTraitAttribute
	{
		public string Property { get; private set; }
		public string Method { get; private set; }

		public string StringValue { get; private set; }
		public int IntValue { get; private set; }
		public float FloatValue { get; private set; }
		public bool Invert { get; private set; }

		public ConditionalAttribute(string property, string value, bool invert) : this(property, invert)
		{
			StringValue = value;
		}

		public ConditionalAttribute(string property, int value, bool invert) : this(property, invert)
		{
			IntValue = value;
		}

		public ConditionalAttribute(string property, float value, bool invert) : this(property, invert)
		{
			FloatValue = value;
		}

		public ConditionalAttribute(string property, bool invert) : base(int.MaxValue)
		{
			Property = property;
			Invert = invert;
		}

		public ConditionalAttribute(string method) : base(int.MaxValue)
		{
			Method = method;
		}
	}
}
