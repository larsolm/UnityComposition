using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public struct Variable
	{
		public string Name { get; private set; }
		public VariableValue Value { get; private set; }

		public static Variable Empty => Create(string.Empty, VariableValue.Empty);

		public static Variable Create(string name, VariableValue value)
		{
			return new Variable
			{
				Name = name,
				Value = value
			};
		}
	}
}
