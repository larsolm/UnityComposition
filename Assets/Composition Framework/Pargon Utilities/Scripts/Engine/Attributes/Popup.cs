using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class PopupAttribute : PropertyAttribute
	{
		public List<int> IntValues { get; private set; }
		public List<float> FloatValues { get; private set; }
		public List<string> Options { get; private set; }

		public PopupAttribute(string[] options)
		{
			Options = options.ToList();
		}

		public PopupAttribute(int[] values, string[] options = null)
		{
			IntValues = values.ToList();
			Options = options?.ToList();
		}

		public PopupAttribute(float[] values, string[] options = null)
		{
			FloatValues = values.ToList();
			Options = options?.ToList();
		}
	}
}