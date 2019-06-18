using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class DropdownAttribute : PropertyAttribute
	{
		public List<string> Options { get; private set; }
		public List<int> IntValues { get; private set; }
		public List<float> FloatValues { get; private set; }

		public DropdownAttribute(string[] options)
		{
			Options = options.ToList();
		}

		public DropdownAttribute(string[] options, int[] values) : this(options)
		{
			IntValues = values.ToList();
		}

		public DropdownAttribute(string[] options, float[] values) : this(options)
		{
			FloatValues = values.ToList();
		}
	}
}
