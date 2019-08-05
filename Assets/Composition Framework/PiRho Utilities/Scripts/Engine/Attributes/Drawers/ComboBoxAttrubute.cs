using System.Collections.Generic;
using System.Linq;

namespace PiRhoSoft.Utilities
{
	public class ComboBoxAttribute : PropertyTraitAttribute
	{
		public List<string> Options { get; private set; }

		public ComboBoxAttribute(string[] options) : base(FieldPhase, 0)
		{
			Options = options.ToList();
		}
	}
}