using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class ScenePickerAttribute : PropertyAttribute
	{
		public string CreateMethod { get; private set; }

		public ScenePickerAttribute(string createMethod)
		{
			CreateMethod = createMethod;
		}
	}
}
