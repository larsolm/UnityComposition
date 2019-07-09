using UnityEngine;

namespace PiRhoSoft.Utilities.Engine
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
